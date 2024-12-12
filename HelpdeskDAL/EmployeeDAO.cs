/*
 * File: EmployeeDAO.cs
 * @author: Thushar Joseph Joji, 1190586
 */

using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace HelpdeskDAL
{
    public class EmployeeDAO
    {
        readonly IRepository<Employee> _repo;
        public EmployeeDAO()
        {
            _repo = new HelpdeskRepository<Employee>();
        }
        public async Task<Employee> GetByEmail(string email)
        {
            Employee? selectedEmployee;
            try
            {
                //HelpdeskContext _db = new();
                selectedEmployee = await _repo.GetOne(emp => emp.Email == email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return selectedEmployee;
        }

        public async Task<Employee> GetByLastname(string lastname)
        {
            Employee? selectedEmployee;
            try
            {
                selectedEmployee = await _repo.GetOne(emp => emp.LastName == lastname);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return selectedEmployee;
        }

        public async Task<Employee> GetByPhoneNumber(string phoneNum)
        {
            Employee? selectedEmployee;
            try
            {
                //HelpdeskContext _db = new();
                selectedEmployee = await _repo.GetOne(emp => emp.PhoneNo == phoneNum);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return selectedEmployee;
        }

        public async Task<Employee> GetById(int id)
        {
            Employee? selectedEmployee;
            try
            {
                //HelpdeskContext _db = new();
                selectedEmployee = await _repo.GetOne(emp => emp.Id == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return selectedEmployee;
        }

        public async Task<List<Employee>> GetAll()
        {
            List<Employee> allEmployees;
            try
            {
                //HelpdeskContext _db = new();
                allEmployees = await _repo.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return allEmployees;
        }

        public async Task<int> Add(Employee newEmployee)
        {
            try
            {
                //HelpdeskContext _db = new();
                /*await _db.Employees.AddAsync(newEmployee);
                await _db.SaveChangesAsync();*/
                await _repo.Add(newEmployee);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return newEmployee.Id;
        }

        public async Task<UpdateStatus> Update(Employee updatedEmployee)
        {
            UpdateStatus status = UpdateStatus.Failed;
            try
            {
                //HelpdeskContext _db = new();
                /*Employee? currentEmployee =
                    await _repo.GetOne(emp => emp.Id == updatedEmployee.Id);
                _repo.Entry(currentEmployee!).CurrentValues.SetValues(updatedEmployee);
                employeeUpdated = await _db.SaveChangesAsync();  */   //should return a value of 1
                //updatedEmployee = await _repo.GetOne(emp => emp.Id == updatedEmployee.Id);
                status = await _repo.Update(updatedEmployee);
            }
            catch (DbUpdateConcurrencyException)
            {
                status = UpdateStatus.Stale;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return status;
        }
        public async Task<int> Delete(int? id)
        {
            int employeesDeleted = -1;
            try
            {
                /*HelpdeskContext _db = new();
                Employee? selectedEmployee =
                    await _db.Employees.FirstOrDefaultAsync(emp => emp.Id == id);
                _db.Employees.Remove(selectedEmployee);*/
                employeesDeleted = await _repo.Delete((int)id!);        //returns number of rows removed
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return employeesDeleted;
        }
    }
}
