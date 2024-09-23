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
                PhoneNo = "(226)998-9806",
                Email = "tj@abc.com",
                DepartmentId = 100,
            };
            Assert.True(await dao.Add(newEmployee) > 0);
        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            EmployeeDAO dao = new();
            Employee? employeeForUpdate = await dao.GetByEmail("tj@abc.com");

            if (employeeForUpdate != null)
            {
                string oldPhoneNo = employeeForUpdate.PhoneNo;
                string newPhoneNo = oldPhoneNo == "226-998-9806" ? "555-555-5555" : "226-998-9806";
                employeeForUpdate!.PhoneNo = newPhoneNo;
            }
            Assert.True(await dao.Update(employeeForUpdate!) == 1);     //1 indicates number of rows updated
        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            EmployeeDAO dao = new();
            Employee? employeeToDelete = await dao.GetById(1);
            Assert.True(await dao.Delete(employeeToDelete.Id!) == 1);
        }
    }
}