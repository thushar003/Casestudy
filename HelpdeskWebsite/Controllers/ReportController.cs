﻿/*
 * File: ReportController.cs
 * @author: Thushar Joseph Joji, 1190586
 */

using HelpdeskWebsite.Reports;
using Microsoft.AspNetCore.Mvc;
namespace HelpdeskWebsite.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public ReportController(IWebHostEnvironment env)
        {
            _env = env;
        }
        [Route("api/employeereport")]
        [HttpGet]
        public IActionResult GetEmployeeReport()
        {
            EmployeeReport emp = new();
            emp.GenerateReport(_env.WebRootPath);
            return Ok(new { msg = "Report Generated" });
        }
    }
}