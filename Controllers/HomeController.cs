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
        public DocumentTable documentTable = new();
        private readonly BlobService _blobService;

        

		private readonly ILogger<HomeController> _logger;
		private readonly IConfiguration _configuration;

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//
		public HomeController(IConfiguration configuration,BlobService blobService, ILogger<HomeController> logger)
		{
			_logger = logger;
			_blobService = blobService;
			_configuration = configuration;
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
            _configuration.GetValue<string>("AzureSQLDatabase");

			List<ClaimTable> claims = claimTable.GetAllClaims();
            
            List<DocumentTable> documents = documentTable.GetAllDocuments();
			Task<List<string>> docName = _blobService.GetBlobsAsync("claim-documents");

            // Pass the products and userID to the view
            ViewData["Claims"] = claims;
            ViewData["Documents"] = documents;
            ViewData["docNames"] = docName;

			// Return the view with the products
			return View(claims);

		}

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

        public IActionResult CoordAndManagPage()
		{
            // Retrieve all products from the database
            List<ClaimTable> claims = claimTable.GetAllClaims();
            List<DocumentTable> documents = documentTable.GetAllDocuments();
            Task<List<string>> docName = _blobService.GetBlobsAsync("claim-documents");

            // Pass the products and userID to the view
            ViewData["Claims"] = claims;
            ViewData["Documents"] = documents;
            ViewData["docNames"] = docName;

            return View(claims);
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Action method to download a file
		public async Task<IActionResult> DownloadFile(string fileName)
        {
            var stream = await _blobService.DownloadBlobAsync("claim-documents", fileName);
            return File(stream, "application/octet-stream", fileName);
        }

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// POST method to add a new customer profile
		[HttpPost]
        public async Task<IActionResult> AddClaim(ClaimTable claim)
        {
	        DateTime localDate = DateTime.Now;

	        // Server-side validation: Ensure HoursWorked and HourlyRate are positive
	        if (claim.HoursWorked <= 0)
	        {
		        ModelState.AddModelError(string.Empty, "Hours Worked must be greater than zero.");
		        return View("LecturerPage"); // Return the user to the Create Claim view with the error
	        }

	        if (claim.HourlyRate <= 0)
	        {
		        ModelState.AddModelError(string.Empty, "Hourly Rate must be greater than zero.");
		        return View("LecturerPage"); // Return the user to the Create Claim view with the error
	        }


			claim.dateTime = localDate;
			claim.status = "Pending";
            claim.TotalAmount = claim.HourlyRate * claim.HoursWorked;
            claimTable.CreateClaim(claim);


            return RedirectToAction("LecturerPage");
        }

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		[HttpPost]
        public async Task<IActionResult> UpdateApprovalStatus(int claimID, string status)
        {
	        // Retrieve all products from the database
	        List<ClaimTable> claims = claimTable.GetAllClaims();
	        List<DocumentTable> documents = documentTable.GetAllDocuments();

	        // Server-side validation: Check if the ClaimID exists
	        var claim = claimTable.GetClaimById(claimID); // Assuming GetClaimById is implemented in claimTable
	        if (claim == null)
	        {
		        ViewData["Claims"] = claims;
		        ViewData["Documents"] = documents;

		        ModelState.AddModelError(string.Empty, "Invalid Claim ID. Please ensure the claim exists before uploading documents.");
		        // Instead of redirecting, return the view with the current model
		        return View("CoordAndManagPage"); // Adjust this to your actual view name and model
	        }
			// Update the product's availability in the database
			claimTable.UpdateStatus(claimID, status);
            // Redirect to the MyWork action
            return RedirectToAction("CoordAndManagPage");
        }

        //같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// POST method to upload an image to blob storage
		[HttpPost]
        public async Task<IActionResult> UploadDocument(DocumentTable document ,IFormFile file)
        {
	        DateTime localDate = DateTime.Now;

	        // Retrieve all products from the database
	        List<ClaimTable> claims = claimTable.GetAllClaims();
	        List<DocumentTable> documents = documentTable.GetAllDocuments();

			// Server-side validation: Check if the ClaimID exists
			var claim = claimTable.GetClaimById(document.ClaimID); // Assuming GetClaimById is implemented in claimTable
	        if (claim == null)
	        {
		        ViewData["Claims"] = claims;
		        ViewData["Documents"] = documents;

				ModelState.AddModelError(string.Empty, "Invalid Claim ID. Please ensure the claim exists before uploading documents.");
		        // Instead of redirecting, return the view with the current model
		        return View("LecturerPage"); // Adjust this to your actual view name and model
	        }

			if (file != null)
	        {
		        using var stream = file.OpenReadStream();
		        await _blobService.UploadBlobAsync("claim-documents", file.FileName, stream); // Upload image to blob storage

		        document.DocumentURL = await _blobService.GetBlobUrlAsync("claim-documents", file.FileName);
                document.dateTime = localDate;
                document.DocumentName = file.FileName;
		        documentTable.NewDocument(document);
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
