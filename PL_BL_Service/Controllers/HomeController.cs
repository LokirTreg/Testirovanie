using Microsoft.AspNetCore.Mvc;
using PL_BL_Service.BL;
using PL_BL_Service.Models;
using System.Diagnostics;

namespace PL_BL_Service.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBusinessService _businessService;

        public HomeController(IBusinessService businessService)
        {
            _businessService = businessService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
