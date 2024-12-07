using HelpdeskDAL;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace HelpdeskWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        [HttpGet("{description}")]
        public async Task<IActionResult> GetByDescription(string description)
        {
            try
            {
                ProblemViewModel viewmodel = new() { Description = description };
                await viewmodel.GetByDescription();
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
        public async Task<ActionResult> Put([FromBody] ProblemViewModel viewmodel)
        {
            try
            {
                int retVal = await viewmodel.Update();
                return retVal switch
                {
                    1 => Ok(new { msg = "Problem " + viewmodel.Description + " updated!" }),
                    -1 => Ok(new { msg = "Problem " + viewmodel.Description + " not updated!" }),
                    -2 => Ok(new { msg = "Data is stale for " + viewmodel.Description + ", Problem not updated!" }),
                    _ => Ok(new { msg = "Problem " + viewmodel.Description + " not updated!" }),
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
                ProblemViewModel viewmodel = new();
                List<ProblemViewModel> allProblems = await viewmodel.GetAll();
                return Ok(allProblems);
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
