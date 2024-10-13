using CMCSapp_ST10311777.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CMCSapp_ST10311777.Services;

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
        public ClaimTable claimTable = new();
        private readonly BlobService _blobService;

		private readonly ILogger<HomeController> _logger;

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//
        public HomeController(BlobService blobService, ILogger<HomeController> logger)
		{
			_logger = logger;
			_blobService = blobService;
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

        [HttpGet]
        public IActionResult LecturerPage()
		{

            // Retrieve all products from the database
            List<ClaimTable> claims = claimTable.GetAllClaims();

            // Pass the products and userID to the view
            ViewData["Claims"] = claims;

            // Return the view with the products
            return View(claims);

		}

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

        public IActionResult CoordAndManagPage()
		{
            // Retrieve all products from the database
            List<ClaimTable> claims = claimTable.GetAllClaims();

            // Pass the products and userID to the view
            ViewData["Claims"] = claims;

            return View(claims);
		}

        // POST method to add a new customer profile
        [HttpPost]
        public async Task<IActionResult> AddClaim(ClaimTable claim)
        {

            claim.status = "Pending";
            claim.TotalAmount = claim.HourlyRate * claim.HoursWorked;
            claimTable.CreateClaim(claim);


            return RedirectToAction("LecturerPage");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateApprovalStatus(int claimID, string status)
        {
            // Update the product's availability in the database
            claimTable.UpdateStatus(claimID, status);
            // Redirect to the MyWork action
            return RedirectToAction("CoordAndManagPage");
        }

        // POST method to upload an image to blob storage
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
	        if (file != null)
	        {
		        using var stream = file.OpenReadStream();
		        await _blobService.UploadBlobAsync("claim-documents", file.FileName, stream); // Upload image to blob storage
	        }
	        return RedirectToAction("LecturerPage");
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
