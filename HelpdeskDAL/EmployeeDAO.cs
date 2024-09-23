using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace HelpdeskDAL
{
    public class EmployeeDAO
    {
        public async Task<Employee> GetByEmail(string email)
        {
            Employee? selectedEmployee;
            try
            {
                HelpdeskContext _db = new();
                selectedEmployee = await _db.Employees.FirstOrDefaultAsync(emp => emp.Email == email);
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
                HelpdeskContext _db = new();
                selectedEmployee = await _db.Employees.FirstOrDefaultAsync(emp => emp.PhoneNo == phoneNum);
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
                HelpdeskContext _db = new();
                selectedEmployee = await _db.Employees.FindAsync(id);
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
                HelpdeskContext _db = new();
                allEmployees = await _db.Employees.ToListAsync();
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
                HelpdeskContext _db = new();
                await _db.Employees.AddAsync(newEmployee);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return newEmployee.Id;
        }

        public async Task<int> Update(Employee updatedEmployee)
        {
            int employeeUpdated = -1;
            try
            {
                HelpdeskContext _db = new();
                Employee? currentEmployee =
                    await _db.Employees.FirstOrDefaultAsync(emp => emp.Id == updatedEmployee.Id);
                _db.Entry(currentEmployee!).CurrentValues.SetValues(updatedEmployee);
                employeeUpdated = await _db.SaveChangesAsync();     //should return a value of 1
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return employeeUpdated;
        }
        public async Task<int> Delete(int? id)
        {
            int employeesDeleted = -1;
            try
            {
                HelpdeskContext _db = new();
                Employee? selectedEmployee =
                    await _db.Employees.FirstOrDefaultAsync(emp => emp.Id == id);
                _db.Employees.Remove(selectedEmployee);
                employeesDeleted = await _db.SaveChangesAsync();        //returns number of rows removed
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
