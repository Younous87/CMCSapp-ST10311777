using CMCSapp_ST10311777.Models;
using CMCSapp_ST10311777.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

namespace CMCSappTests
{

	[TestClass]
	// Define the test class for ClaimTable operations
	public class TestClaimTable
	{
		// Field to hold the instance of ClaimTable for testing
		private ClaimTable _claimTable;

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		[TestInitialize]
		// Setup method to initialize the _claimTable object before each test
		public void Setup()
		{
			_claimTable = new ClaimTable();
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		[TestMethod]
		// Test to ensure that a claim is successfully created and inserted into the database
		public void CreateClaim_ShouldInsertClaim()
		{
			// Create a new ClaimTable object with test data
			var claimTable = new ClaimTable
			{
				HoursWorked = 10,  
				HourlyRate = 50,   
				TotalAmount = 500, 
				status = "Pending", 
				dateTime = DateTime.Now 
			};

			var claimDb = new ClaimTable(); // Instance to interact with the database

			// Call the CreateClaim method to insert the new claim
			var result = claimDb.CreateClaim(claimTable);

			// Verify that one row was inserted successfully
			Assert.AreEqual(1, result, "Claim should be created");
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		[TestMethod]
		// Test to ensure that the status of a claim is updated correctly
		public void UpdateStatus_ShouldUpdateClaimStatus()
		{
			// Set up a claimID and the new status to update
			int claimID = 15;  // Sample claimID 
			string newStatus = "Approved";  // New status for the claim

			var claimDb = new ClaimTable(); // Instance to interact with the database

			// Call the UpdateStatus method to update the claim's status
			claimDb.UpdateStatus(claimID, newStatus);

			// Retrieve the updated claim and verify the status has changed
			var updatedClaim = claimDb.GetClaimById(claimID);
			Assert.AreEqual(newStatus, updatedClaim.status,"Claim status should be updated"); // Ensure the new status matches
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		[TestMethod]
		// Test to ensure that all claims are returned from the database
		public void GetAllClaims_ShouldReturnAllClaims()
		{
			// Retrieve all claims using the GetAllClaims method
			List<ClaimTable> claims = _claimTable.GetAllClaims();

			// Verify that the claims list is not null and contains at least one claim
			Assert.IsNotNull(claims, "Claims list should not be null.");
			Assert.IsTrue(claims.Count > 0, "Claims list should contain at least one claim.");
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

	}
	//같같같같같같같같같같같같같같같같같같같같...ooo000 END OF FILE 000ooo...같같같같같같같같같같같같같같같같같같같같//


	[TestClass]
	// Test class for testing DocumentTable operations
	public class DocumentTableTests
	{
		// Test to ensure a new document is inserted successfully into the database
		[TestMethod]
		public void NewDocument_ShouldInsertDocument()
		{
			// Create a new DocumentTable object with sample data
			var document = new DocumentTable
			{
				DocumentURL = "http://example.com/document.pdf",  
				ClaimID = 15,  
				dateTime = DateTime.Now,  
				DocumentName = "test-blob.txt"
			};

			var documentDb = new DocumentTable();  // Instance of DocumentTable for database interaction

			// Call the NewDocument method to insert the document into the database
			int rowsAffected = documentDb.NewDocument(document);

			// Ensure that one row was inserted
			Assert.AreEqual(1, rowsAffected, "NewDocument should insert 1 row in the database.");
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Test to ensure all documents are returned from the database
		[TestMethod]
		public void GetAllDocuments_ShouldReturnDocuments()
		{
			// Create an instance of DocumentTable to retrieve documents from the database
			var documentDb = new DocumentTable();

			// Call the GetAllDocuments method to fetch all documents
			var documents = documentDb.GetAllDocuments();

			// Ensure that the list of documents is not null and contains at least one document
			Assert.IsNotNull(documents, "The documents list should not be null.");
			Assert.IsTrue(documents.Count > 0, "GetAllDocuments should return at least one document.");
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

	}
	//같같같같같같같같같같같같같같같같같같같같...ooo000 END OF FILE 000ooo...같같같같같같같같같같같같같같같같같같같같//


	[TestClass]
	// Test class for testing BlobService operations
	public class BlobServiceTests
	{
		// Fields for BlobService, container name, and test blob name used in the tests
		private BlobService _blobService;
		private string _containerName = "test-container";  
		private string _testBlobName = "test-blob.txt";   

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Setup method executed before each test to initialize BlobService with configuration from appsettings.json
		[TestInitialize]
		public void Setup()
		{
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")  
				.Build();

			_blobService = new BlobService(configuration);  // Initialize BlobService with configuration
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Test to verify that a blob is uploaded successfully
		[TestMethod]
		public async Task UploadBlobAsync_ShouldUploadBlobSuccessfully()
		{
			// Create a MemoryStream with sample content to simulate a file
			var content = new MemoryStream();
			var writer = new StreamWriter(content);
			writer.Write("This is a test blob");  // Write test content to the stream
			writer.Flush();
			content.Position = 0;  // Reset stream position for reading

			// Upload the test blob to the specified container
			await _blobService.UploadBlobAsync(_containerName, _testBlobName, content);

			// Check if the uploaded blob exists in the container
			var blobs = await _blobService.GetBlobsAsync(_containerName);
			Assert.IsTrue(blobs.Contains(_testBlobName), "Blob should be uploaded successfully.");
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Test to verify that all blobs are returned from the container
		[TestMethod]
		public async Task GetBlobsAsync_ShouldReturnBlobList()
		{
			// Retrieve the list of blobs from the specified container
			var blobs = await _blobService.GetBlobsAsync(_containerName);

			// Ensure that the blobs list is not null and contains at least one blob
			Assert.IsNotNull(blobs, "Blob list should not be null.");
			Assert.IsTrue(blobs.Count > 0, "Blob container should contain at least one blob.");
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Test to verify that a valid URL is returned for a specific blob
		[TestMethod]
		public async Task GetBlobUrlAsync_ShouldReturnValidUrl()
		{
			// Get the URL for the specified blob
			var blobUrl = await _blobService.GetBlobUrlAsync(_containerName, _testBlobName);

			// Ensure the URL is not null and is a valid URI
			Assert.IsNotNull(blobUrl, "Blob URL should not be null.");
			Assert.IsTrue(Uri.IsWellFormedUriString(blobUrl, UriKind.Absolute), "Blob URL should be a valid URI.");
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

		// Test to verify that a blob is downloaded successfully
		[TestMethod]
		public async Task DownloadBlobAsync_ShouldDownloadBlobSuccessfully()
		{
			// Download the blob from the specified container
			var stream = await _blobService.DownloadBlobAsync(_containerName, _testBlobName);

			// Ensure the downloaded stream is not null and contains data
			Assert.IsNotNull(stream, "Downloaded blob stream should not be null.");
			Assert.IsTrue(stream.Length > 0, "Downloaded blob should have content.");
		}

		//같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같같//

	}
	//같같같같같같같같같같같같같같같같같같같같...ooo000 END OF FILE 000ooo...같같같같같같같같같같같같같같같같같같같같//
}