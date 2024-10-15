﻿using HelpdeskDAL;
using HelpdeskDAL;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

public class HelpdeskRepository<T> : IRepository<T> where T : HelpdeskEntity
{
    readonly private HelpdeskContext _db;
    public HelpdeskRepository()
    {
        _db = new HelpdeskContext();
    }
    public async Task<List<T>> GetAll()
    {
        return await _db.Set<T>().ToListAsync();
    }
    public async Task<List<T>> GetSome(Expression<Func<T, bool>> match)
    {
        return await _db.Set<T>().Where(match).ToListAsync();
    }
    public async Task<T?> GetOne(Expression<Func<T, bool>> match)
    {
        return await _db.Set<T>().FirstOrDefaultAsync(match);
    }
    public async Task<T> Add(T entity)
    {
        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
    public async Task<UpdateStatus> Update(T updatedEntity)
    {
        UpdateStatus operationStatus = UpdateStatus.Failed;
        try
        {
            T? currentEntity = await GetOne(ent => ent.Id == updatedEntity.Id);
            _db.Entry(currentEntity!).OriginalValues["Timer"] = updatedEntity.Timer;
            _db.Entry(currentEntity!).CurrentValues.SetValues(updatedEntity);
            if (await _db.SaveChangesAsync() == 1) // should throw exception if stale;
                operationStatus = UpdateStatus.Ok;
        }
        catch (DbUpdateConcurrencyException dbx)
        {
            operationStatus = UpdateStatus.Stale;
            Console.WriteLine("Problem in " + MethodBase.GetCurrentMethod()!.Name + dbx.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Problem in " + MethodBase.GetCurrentMethod()!.Name + ex.Message);
        }
        return operationStatus;
    }
    public async Task<int> Delete(int id)
    {
        T? currentEntity = await GetOne(ent => ent.Id == id);
        _db.Set<T>().Remove(currentEntity!);
        return _db.SaveChanges();
    }
}