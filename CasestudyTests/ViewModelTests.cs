using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpdeskViewModels;

namespace CasestudyTests
{
    public class ViewModelTests
    {
        [Fact]
        public async Task Employee_GetByIdTest()
        {
            EmployeeViewModel vm = new() { Id = 1003 };
            await vm.GetById();
        }

        [Fact]
        public async Task Employee_GetByEmailTest()
        {
            EmployeeViewModel vm = new() { Email = "some@abc.com" };
            await vm.GetByEmail();
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {
            List<EmployeeViewModel> allEmployeeVms;
            EmployeeViewModel vm = new();
            allEmployeeVms = await vm.GetAll();
            Assert.True(allEmployeeVms.Count > 0);
        }

        [Fact]
        public async Task Employee_AddTest()
        {
            EmployeeViewModel vm;
            vm = new()
            {
                Title = "Mr.",
                Firstname = "Thushar",
                Lastname = "Joji",
                Email = "tj@abc.com",
                Phoneno = "(777) 777-7777",
                DepartmentId = 100
            };
            await vm.Add();
            Assert.True(vm.Id > 0);
        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            EmployeeViewModel vm = new() { Phoneno = "(777) 777-7777" };
            await vm.GetByPhoneNumber(); // Employee just added in Add test
            vm.Email = vm.Email == "some@abc.com" ? "tj@abc.com" : "some@abc.com";
            // will be -1 if failed 0 if no data changed, 1 if succcessful
            Assert.True(await vm.Update() == 1);
        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            EmployeeViewModel vm = new() { Phoneno = "(777) 777-7777" };
            await vm.GetByPhoneNumber(); // Employee just added
            Assert.True(await vm.Delete() == 1); // 1 employee deleted
        }

        [Fact]
        public async Task Employee_GetByPhoneNumberTest()
        {
            EmployeeViewModel vm = new() { Phoneno = "(777) 777-7777" };
            await vm.GetByPhoneNumber();
            Assert.NotNull(vm.Phoneno);
        }
    }
}