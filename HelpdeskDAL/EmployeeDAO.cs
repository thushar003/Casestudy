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
    }
}
