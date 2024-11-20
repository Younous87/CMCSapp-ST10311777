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

		// Initialize claim and document tables to interact with claims and documents data
		public ClaimTable claimTable = new();
		public DocumentTable documentTable = new();

		// Service for blob storage
		private readonly BlobService _blobService;

        private readonly ClaimVerificationService _claimVerificationService;

        // Logger service for logging errors, information, and warnings
        private readonly ILogger<HomeController> _logger;

		// Configuration service for accessing app settings
		private readonly IConfiguration _configuration;

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Constructor to inject services
		public HomeController(ClaimVerificationService claimVerificationService, IConfiguration configuration, BlobService blobService, ILogger<HomeController> logger)
		{
			_logger = logger;
			_blobService = blobService;
			_configuration = configuration;
            _claimVerificationService = claimVerificationService;
        }

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Default action for home page
		public IActionResult Index()
		{
			return View();
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Default action for privacy page
		public IActionResult Privacy()
		{
			return View();
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Action for LecturerPage to display claims and documents for the lecturer
		[HttpGet]
		public async Task<IActionResult> LecturerPage()
		{
			// Retrieve all claims and documents from the database
			List<ClaimTable> claims = claimTable.GetAllClaims();

			List<DocumentTable> documents = documentTable.GetAllDocuments();

			// Fetch blob names from blob storage asynchronously
			Task<List<string>> docName = _blobService.GetBlobsAsync("claim-documents");

			// Pass claims, documents, and blob names to the view using ViewData
			ViewData["Claims"] = claims;
			ViewData["Documents"] = documents;
			ViewData["docNames"] = docName;

			// Return view displaying claims
			return View(claims);
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Action for CoordAndManagPage for coordinators and managers to view claims and documents
		public async Task<IActionResult> CoordAndManagPage()
		{
			// Retrieve all claims and documents from the database
			List<ClaimTable> claims = claimTable.GetAllClaims();
			List<DocumentTable> documents = documentTable.GetAllDocuments();

			// Fetch blob names from blob storage asynchronously
			Task<List<string>> docName = _blobService.GetBlobsAsync("claim-documents");

			// Pass claims, documents, and blob names to the view using ViewData
			ViewData["Claims"] = claims;
			ViewData["Documents"] = documents;
			ViewData["docNames"] = docName;

			// Return view displaying claim
			return View(claims);
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Action method to download a file from blob storage
		public async Task<IActionResult> DownloadFile(string fileName)
		{
			// Download the file from blob storage
			var stream = await _blobService.DownloadBlobAsync("claim-documents", fileName);

			// Return the file as a download
			return File(stream, "application/octet-stream", fileName);
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// POST action to add a new claim 
		[HttpPost]
		public async Task<IActionResult> AddClaim(ClaimTable claim)
		{
			DateTime localDate = DateTime.Now;

            List<ClaimTable> claims = claimTable.GetAllClaims();

            List<DocumentTable> documents = documentTable.GetAllDocuments();

            // Fetch blob names from blob storage asynchronously
            Task<List<string>> docName = _blobService.GetBlobsAsync("claim-documents");

            // Pass claims, documents, and blob names to the view using ViewData
            ViewData["Claims"] = claims;
            ViewData["Documents"] = documents;
            ViewData["docNames"] = docName;

            // Use the verification service to validate the claim
            var (isValid, validationErrors) = _claimVerificationService.VerifyClaim(claim);


            if (!isValid)
            {
                // Add all validation errors to ModelState
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View("LecturerPage");
            }

            // Set the current date and status of the claim
            claim.claimDate = localDate;
            //claim.status = "Pending";

            // Determine the initial status using auto-approval logic
            claim.status = _claimVerificationService.DetermineAutoApprovalStatus(claim);

            // Calculate the total claim amount based on hours worked and hourly rate
            claim.totalAmount = claim.hourlyRate * claim.hoursWorked;

			// Insert the claim into the database
			claimTable.CreateClaim(claim);

			// Redirect to the LecturerPage after adding the claim
			return RedirectToAction("LecturerPage");
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// POST action to update the approval status of a claim 
		[HttpPost]
		public async Task<IActionResult> UpdateApprovalStatus(int claimID, string status)
		{
			// Retrieve all claims and documents from the database
			List<ClaimTable> claims = claimTable.GetAllClaims();
			List<DocumentTable> documents = documentTable.GetAllDocuments();

			// Check if the claim exists in the database
			var claim = claimTable.GetClaimById(claimID);

			if (claim == null)
			{
				// If claim doesn't exist, return the view with an error message
				ViewData["Claims"] = claims;
				ViewData["Documents"] = documents;
				ModelState.AddModelError(string.Empty, "Invalid Claim ID. Please ensure the claim exists before uploading documents.");
				return View("CoordAndManagPage");
			}

			// Update the claim's status in the database
			claimTable.UpdateStatus(claimID, status);

			// Redirect to the CoordAndManagPage after updating the claim status
			return RedirectToAction("CoordAndManagPage");
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// POST action to upload a document to blob storage and link it to a claim
		[HttpPost]
		public async Task<IActionResult> UploadDocument(DocumentTable document, IFormFile file)
		{
			DateTime localDate = DateTime.Now;

			// Retrieve all claims and documents from the database
			List<ClaimTable> claims = claimTable.GetAllClaims();
			List<DocumentTable> documents = documentTable.GetAllDocuments();

			// Check if the claimID exists before allowing document upload
			var claim = claimTable.GetClaimById(document.ClaimID);
			if (claim == null)
			{
				// If claim doesn't exist, return the view with an error message
				ViewData["Claims"] = claims;
				ViewData["Documents"] = documents;
				ModelState.AddModelError(string.Empty, "Invalid Claim ID. Please ensure the claim exists before uploading documents.");
				return View("LecturerPage");
			}

			// If a file is selected, upload it to blob storage
			if (file != null)
			{
				using var stream = file.OpenReadStream();
				await _blobService.UploadBlobAsync("claim-documents", file.FileName, stream); // Upload file to blob storage

				// Save document details to the database
				document.DocumentURL = await _blobService.GetBlobUrlAsync("claim-documents", file.FileName);
				document.dateTime = localDate;
				document.DocumentName = file.FileName;
				documentTable.NewDocument(document);
			}

			// Redirect to the LecturerPage after uploading the document
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
