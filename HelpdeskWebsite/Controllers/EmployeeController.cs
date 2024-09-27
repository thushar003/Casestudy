using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using HelpdeskViewModels;

namespace HelpdeskWebsite.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class EmployeeController : ControllerBase
        {
            [HttpGet("{lastname}")]
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
        }
}
