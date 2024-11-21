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
		public LecturerTable lecturerTable = new();

		// Service for blob storage
		private readonly BlobService _blobService;

        private readonly ClaimVerificationService _claimVerificationService;

        private readonly ClaimProcessingService _claimProcessingService;

        // Logger service for logging errors, information, and warnings
        private readonly ILogger<HomeController> _logger;

		// Configuration service for accessing app settings
		private readonly IConfiguration _configuration;

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Constructor to inject services
		public HomeController(ClaimProcessingService claimProcessingService, ClaimVerificationService claimVerificationService, IConfiguration configuration, BlobService blobService, ILogger<HomeController> logger)
		{
			_logger = logger;
			_blobService = blobService;
			_configuration = configuration;
            _claimVerificationService = claimVerificationService;
            _claimProcessingService = claimProcessingService;
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

        public async Task<IActionResult> HR()
        {
            // Retrieve all claims and documents from the database

			List<LecturerTable> lecturers = lecturerTable.GetAllLecturers();

            // Pass claims, documents, and blob names to the view using ViewData

			ViewData["Lecturers"] = lecturers;

            return View(lecturers);
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

            // Retrieve all claims and documents
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

                // Return the view with the existing model and error messages
                return View("LecturerPage", claims);
            }

            // Set the current date
            claim.claimDate = localDate;

            // Calculate the total claim amount based on hours worked and hourly rate
            claim.totalAmount = claim.hourlyRate * claim.hoursWorked;

            // Determine the initial status using auto-approval logic AFTER validation
            claim.status = _claimVerificationService.DetermineAutoApprovalStatus(claim);

            // Insert the claim into the database
            claimTable.CreateClaim(claim);

            // Redirect to the LecturerPage after successfully adding the claim
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
                ModelState.Clear();
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
            // Retrieve all claims and documents from the database
            List<ClaimTable> claims = claimTable.GetAllClaims();
            List<DocumentTable> documents = documentTable.GetAllDocuments();

            // Validate input
            if (file == null || file.Length == 0)
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Please select a file to upload.");
                ViewData["Claims"] = claims;
                ViewData["Documents"] = documents;
                return View("LecturerPage");
            }

            // Allowed file extensions
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            // File type validation
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Invalid file type. Only PDF, DOC, DOCX, and XLSX files are allowed.");
                ViewData["Claims"] = claims;
                ViewData["Documents"] = documents;
                return View("LecturerPage");
            }

            // File size validation (2 MB)
            if (file.Length > 2 * 1024 * 1024)
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "File size exceeds 2 MB. Please upload a smaller file.");
                ViewData["Claims"] = claims;
                ViewData["Documents"] = documents;
                return View("LecturerPage");
            }

            // Check if the claimID exists
            var claim = claimTable.GetClaimById(document.ClaimID);
            if (claim == null)
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Invalid Claim ID. Please ensure the claim exists before uploading documents.");
                ViewData["Claims"] = claims;
                ViewData["Documents"] = documents;
                return View("LecturerPage");
            }

            try
            {
                // Generate a unique filename to prevent overwriting
                string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

                // Upload file to blob storage
                using var stream = file.OpenReadStream();
                await _blobService.UploadBlobAsync("claim-documents", uniqueFileName, stream);

                // Save document details to the database
                document.DocumentURL = await _blobService.GetBlobUrlAsync("claim-documents", uniqueFileName);
                document.dateTime = DateTime.Now;
                document.DocumentName = uniqueFileName;
                documentTable.NewDocument(document);

                // Redirect to the LecturerPage after successful upload
                return RedirectToAction("LecturerPage");
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error uploading document");

                ModelState.AddModelError(string.Empty, "An error occurred while uploading the document. Please try again.");
                ViewData["Claims"] = claims;
                ViewData["Documents"] = documents;
                return View("LecturerPage");
            }
        }

        [HttpPost]
        public IActionResult AddLecturer(LecturerTable lecturer)
        {
            try
            {
                // Validate the lecturer object
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Invalid input. Please check the details and try again.");
                    return View("HR"); // Return to the HR page with validation errors
                }

                // Add the lecturer to the database
                int rowsAffected = lecturerTable.NewLecturer(lecturer);

                if (rowsAffected > 0)
                {
                    // Log success
                    _logger.LogInformation("Lecturer added successfully: {LecturerName}", $"{lecturer.LecturerFirstName} {lecturer.LecturerLastName}");

                    // Redirect to the HR page after successfully adding the lecturer
                    return RedirectToAction("HR");
                }
                else
                {
                    // Log failure
                    _logger.LogWarning("No rows affected while adding a lecturer: {LecturerName}", $"{lecturer.LecturerFirstName} {lecturer.LecturerLastName}");
                    ModelState.AddModelError(string.Empty, "Failed to add the lecturer. Please try again.");
                    return View("HR");
                }
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Error occurred while adding a lecturer.");

                // Display an error message
                ModelState.AddModelError(string.Empty, "An error occurred while adding the lecturer. Please try again.");
                return View("HR");
            }
        }

        [HttpPost]
        public IActionResult UpdateLecturer(LecturerTable lecturer)
        {
            List<LecturerTable> lecturers = lecturerTable.GetAllLecturers();

            // Pass claims, documents, and blob names to the view using ViewData

            ViewData["Lecturers"] = lecturers;

            try
            {

                // Validate that at least one field (besides ID) is provided
                if (string.IsNullOrEmpty(lecturer.LecturerFirstName) &&
                    string.IsNullOrEmpty(lecturer.LecturerLastName) &&
                    string.IsNullOrEmpty(lecturer.LecturerEmail) &&
                    string.IsNullOrEmpty(lecturer.LecturerPhone))
                {
                    // Clear any other errors before adding the custom message
                    ModelState.Clear();

                    // Add a custom error message
                    ModelState.AddModelError(string.Empty, "At least one field must be completed to update the lecturer information.");
                    return View("HR"); // Return to HR page with error message
                }

                // Check if the lecturer exists
                var existingLecturer = lecturerTable.GetLecturerById(lecturer.LecturerID);
                if (existingLecturer == null)
                {
                    ModelState.AddModelError(string.Empty, "Lecturer not found. Please check the ID and try again.");
                    return View("HR", lecturers);
                }

                // Update the lecturer in the database
                int rowsAffected = lecturerTable.UpdateLecturerById(
                    lecturer.LecturerID,
                    lecturer.LecturerFirstName,
                    lecturer.LecturerLastName,
                    lecturer.LecturerEmail,
                    lecturer.LecturerPhone
                );

                if (rowsAffected > 0)
                {
                    // Log success
                    _logger.LogInformation("Lecturer updated successfully: {LecturerName}", $"{lecturer.LecturerFirstName} {lecturer.LecturerLastName}");

                    // Redirect to the HR page after successfully updating the lecturer
                    return RedirectToAction("HR");
                }
                else
                {
                    // Log failure
                    _logger.LogWarning("No rows affected while updating lecturer ID: {LecturerId}", lecturer.LecturerID);
                    ModelState.AddModelError(string.Empty, "Failed to update the lecturer. Please try again.");
                    return View("HR", lecturers);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Error occurred while updating a lecturer.");

                // Display an error message
                ModelState.AddModelError(string.Empty, "An error occurred while updating the lecturer. Please try again.");
                return View("HR", lecturers);
            }
        }

        // Generate payment report action
        [HttpGet]
        public IActionResult GeneratePaymentReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                var reportBytes = _claimProcessingService.GeneratePaymentReport(startDate, endDate);
                return File(reportBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"PaymentReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating payment report");
                return BadRequest("Could not generate payment report");
            }
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
