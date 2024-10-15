using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpdeskDAL;

namespace CasestudyTests
{
    public class DAOTests
    {
        [Fact]
        public async Task Employee_GetByEmailTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetByEmail("bs@abc.com");
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_GetByIdTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetById(1);
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_GetByPhoneNumberTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetByPhoneNumber("(555) 555-5555");
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {
            EmployeeDAO dao = new();
            List<Employee> allEmployees = await dao.GetAll();
            Assert.True(allEmployees.Count > 0);
        }

        [Fact]
        public async Task Employee_AddTest()
        {
            EmployeeDAO dao = new();
            Employee newEmployee = new()
            {
                Title = "Mr.",
                FirstName = "Thushar",
                LastName = "Joji",
                PhoneNo = "(226) 998-9806",
                Email = "tj@abc.com",
                DepartmentId = 100,
            };
            Assert.True(await dao.Add(newEmployee) > 0);
        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            EmployeeDAO dao = new();
            Employee? employeeForUpdate = await dao.GetById(11);

            if (employeeForUpdate != null)
            {
                string oldPhoneNo = employeeForUpdate.PhoneNo;
                string newPhoneNo = oldPhoneNo == "226-998-9806" ? "555-555-5555" : "226-998-9806";
                employeeForUpdate!.PhoneNo = newPhoneNo;
            }
            Assert.True(await dao.Update(employeeForUpdate) == UpdateStatus.Ok);     //1 indicates number of rows updated
        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            EmployeeDAO dao = new();
            Employee? employeeToDelete = await dao.GetById(1);
            Assert.True(await dao.Delete(employeeToDelete.Id!) == 1);
        }

        [Fact]
        public async Task Employee_ConcurrencyTest()
        {
            EmployeeDAO dao1 = new();
            EmployeeDAO dao2 = new();
            Employee employeeForUpdate1 = await dao1.GetByEmail("tj@abc.com");
            Employee employeeForUpdate2 = await dao2.GetByEmail("tj@abc.com");
            if (employeeForUpdate1 != null)
            {
                string? oldPhoneNo = employeeForUpdate1.PhoneNo;
                string? newPhoneNo = oldPhoneNo == "519-555-1234" ? "555-555-5555" : "519-555-1234";
                employeeForUpdate1.PhoneNo = newPhoneNo;
                if (await dao1.Update(employeeForUpdate1) == UpdateStatus.Ok)
                {
                    // need to change the phone # to something else
                    employeeForUpdate2.PhoneNo = "666-666-6668";
                    Assert.True(await dao2.Update(employeeForUpdate2) == UpdateStatus.Stale);
                }
                else
                    Assert.True(false); // first update failed
            }
            else
                Assert.True(false); // didn't find employee 1
        }
    }
}