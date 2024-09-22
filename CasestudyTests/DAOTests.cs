using System;
using System.Collections.Generic;
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
    }
}