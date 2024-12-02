using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpdeskDAL;
using HelpdeskViewModels;
using Xunit.Abstractions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CasestudyTests
{
    public class DAOTests
    {
        private readonly ITestOutputHelper output;
        public DAOTests(ITestOutputHelper output)
        {
            this.output = output;
        }

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

        [Fact]
        public async Task Employee_LoadPicsTest()
        {
            {
                PicsUtility util = new();
                Assert.True(await util.AddEmployeePicsToDb());
            }
        }

        [Fact]
        public async Task Employee_ComprehensiveTest()
        {
            EmployeeDAO dao = new();
            Employee newEmployee = new()
            {
                FirstName = "Joe",
                LastName = "Smith",
                PhoneNo = "(555)555-1234",
                Title = "Mr.",
                DepartmentId = 100,
                Email = "js@abc.com"
            };
            int newEmployeeId = await dao.Add(newEmployee);
            output.WriteLine("New Employee Generated - Id = " + newEmployeeId);
            newEmployee = await dao.GetById(newEmployeeId);
            byte[] oldtimer = newEmployee.Timer!;
            output.WriteLine("New Employee " + newEmployee.Id + " Retrieved");
            newEmployee.PhoneNo = "(555)555-1233";
            if (await dao.Update(newEmployee) == UpdateStatus.Ok)
            {
                output.WriteLine("Employee " + newEmployeeId + " phone# was updated to - " + newEmployee.PhoneNo);
            }
            else
            {
                output.WriteLine("Employee " + newEmployeeId + " phone# was not updated!");
            }
            newEmployee.Timer = oldtimer; // to simulate another user
            newEmployee.PhoneNo = "doesn't matter data is stale now";
            if (await dao.Update(newEmployee) == UpdateStatus.Stale)
            {
                output.WriteLine("Employee " + newEmployeeId + " was not updated due to stale data");
            }
            dao = new();
            await dao.GetById(newEmployeeId);
            if (await dao.Delete(newEmployeeId) == 1)
            {
                output.WriteLine("Employee " + newEmployeeId + " was deleted!");
            }
            else
            {
                output.WriteLine("Employee " + newEmployeeId + " was not deleted");
            }
            // should be null because it was just deleted
            Assert.Null(await dao.GetById(newEmployeeId));
        }

        [Fact]
        public async Task Employee_ComprehensiveVMTest()
        {
            EmployeeViewModel evm = new()
            {
                Title = "Mr.",
                Firstname = "Some",
                Lastname = "Employee",
                Email = "some@abc.com",
                Phoneno = "(777)777-7777",
                DepartmentId = 100 // ensure department id is in Departments table
            };
            await evm.Add();
            output.WriteLine("New Employee Added - Id = " + evm.Id);
            int? id = evm.Id; // need id for delete later
            await evm.GetById();
            output.WriteLine("New Employee " + id + " Retrieved");
            evm.Phoneno = "(555)555-1233";
            if (await evm.Update() == 1)
            {
                output.WriteLine("Employee " + id + " phone# was updated to - " +
                                 evm.Phoneno);
            }
            else
            {
                output.WriteLine("Employee " + id + " phone# was not updated!");
            }
            evm.Phoneno = "Another change that should not work";
            if (await evm.Update() == -2)
            {
                output.WriteLine("Employee " + id + " was not updated due to stale data");
            }
            evm = new EmployeeViewModel
            {
                Id = id
            };
            // need to reset because of concurrency error
            await evm.GetById();
            if (await evm.Delete() == 1)
            {
                output.WriteLine("Employee " + id + " was deleted!");
            }
            else
            {
                output.WriteLine("Employee " + id + " was not deleted");
            }
            // should throw expected exception
            Task<NullReferenceException> ex = Assert.ThrowsAsync<NullReferenceException>(async ()
                => await evm.GetById());
        }

        [Fact]
        public async Task Call_ComprehensiveTest()
        {
            CallDAO dao = new();
            EmployeeDAO empDao = new();
            Call newCall = new()
            {
                EmployeeId = empDao.GetById(11),
                LastName = "Smith",
                PhoneNo = "(555)555-1234",
                Title = "Mr.",
                DepartmentId = 100,
                Email = "js@abc.com"
            };
            int newCallId = await dao.Add(newCall);
            output.WriteLine("New Call Generated - Id = " + newCallId);
            newCall = await dao.GetById(newCallId);
            byte[] oldtimer = newCall.Timer!;
            output.WriteLine("New Call " + newCall.Id + " Retrieved");
            newCall.PhoneNo = "(555)555-1233";
            if (await dao.Update(newCall) == UpdateStatus.Ok)
            {
                output.WriteLine("Call " + newCallId + " phone# was updated to - " + newCall.PhoneNo);
            }
            else
            {
                output.WriteLine("Call " + newCallId + " phone# was not updated!");
            }
            newCall.Timer = oldtimer; // to simulate another user
            newCall.PhoneNo = "doesn't matter data is stale now";
            if (await dao.Update(newCall) == UpdateStatus.Stale)
            {
                output.WriteLine("Call " + newCallId + " was not updated due to stale data");
            }
            dao = new();
            await dao.GetById(newCallId);
            if (await dao.Delete(newCallId) == 1)
            {
                output.WriteLine("Employee " + newCallId + " was deleted!");
            }
            else
            {
                output.WriteLine("Call " + newCallId + " was not deleted");
            }
            // should be null because it was just deleted
            Assert.Null(await dao.GetById(newCallId));
        }

        [Fact]
        public async Task Call_ComprehensiveVMTest()
        {
            EmployeeViewModel evm = new()
            {
                Title = "Mr.",
                Firstname = "Some",
                Lastname = "Employee",
                Email = "some@abc.com",
                Phoneno = "(777)777-7777",
                DepartmentId = 100 // ensure department id is in Departments table
            };
            await evm.Add();
            output.WriteLine("New Employee Added - Id = " + evm.Id);
            int? id = evm.Id; // need id for delete later
            await evm.GetById();
            output.WriteLine("New Employee " + id + " Retrieved");
            evm.Phoneno = "(555)555-1233";
            if (await evm.Update() == 1)
            {
                output.WriteLine("Employee " + id + " phone# was updated to - " +
                                 evm.Phoneno);
            }
            else
            {
                output.WriteLine("Employee " + id + " phone# was not updated!");
            }
            evm.Phoneno = "Another change that should not work";
            if (await evm.Update() == -2)
            {
                output.WriteLine("Employee " + id + " was not updated due to stale data");
            }
            evm = new EmployeeViewModel
            {
                Id = id
            };
            // need to reset because of concurrency error
            await evm.GetById();
            if (await evm.Delete() == 1)
            {
                output.WriteLine("Employee " + id + " was deleted!");
            }
            else
            {
                output.WriteLine("Employee " + id + " was not deleted");
            }
            // should throw expected exception
            Task<NullReferenceException> ex = Assert.ThrowsAsync<NullReferenceException>(async ()
                => await evm.GetById());
        }
    }
}