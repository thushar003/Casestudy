using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HelpdeskViewModels;

namespace HelpdeskWebsite.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class EmployeeController : ControllerBase
        {
            [HttpGet("{email}")]
            public async Task<IActionResult> GetByEmail(string email)
            {
                try
                {
                    EmployeeViewModel viewmodel = new() { Email = email};
                    await viewmodel.GetByEmail();
                    return Ok(viewmodel);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem in " + GetType().Name + " " +
                                    MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
                }
            }

            [HttpPut]
            public async Task<ActionResult> Put([FromBody] EmployeeViewModel viewmodel)
            {
                try
                {
                    int retVal = await viewmodel.Update();
                    return retVal switch
                    {
                        1 => Ok(new { msg = "Employee " + viewmodel.Email + " updated!" }),
                        -1 => Ok(new { msg = "Employee " + viewmodel.Email + " not updated!" }),
                        -2 => Ok(new { msg = "Data is stale for " + viewmodel.Email + ", Employee not updated!" }),
                        _ => Ok(new { msg = "Employee " + viewmodel.Email + " not updated!" }),
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem in " + GetType().Name + " " +
                                    MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
                }
            }

        [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                try
                {
                    EmployeeViewModel viewmodel = new();
                    List<EmployeeViewModel> allEmployees = await viewmodel.GetAll();
                    return Ok(allEmployees);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem in " + GetType().Name + " " +
                                    MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
                }
            }
    }

}
