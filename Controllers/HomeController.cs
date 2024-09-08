using CMCSapp_ST10311777.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
/// <summary>
/// Name:           Y.Houssen
/// Student:        ST10311777
/// Module code:    PROG6212
/// </summary>
/// 
namespace CMCSapp_ST10311777.Controllers
{
	public class HomeController : Controller
	{
        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

        private readonly ILogger<HomeController> _logger;

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//
        public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

        public IActionResult Index()
		{
			return View();
		}

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

        public IActionResult Privacy()
		{
			return View();
		}

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

        public IActionResult LecturerPage()
		{
			return View();
		}

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

        public IActionResult CoordAndManagPage()
		{
			return View();
		}

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//
    }
}//같같같같같같같같같같같같같같같같같같같같...ooo000 END OF FILE 000ooo...같같같같같같같같같같같같같같같같같같같같//
