using Azure;
using Azure.Storage.Blobs;

namespace CMCSapp_ST10311777.Services
{
	public class BlobService
	{
		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Interact with Azure Blob Storage.
		private readonly BlobServiceClient _blobServiceClient;

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Constructor that initializes the BlobServiceClient 
		public BlobService(IConfiguration configuration)
		{
			// Retrieve the connection string for Azure Storage from configuration settings.
			_blobServiceClient = new BlobServiceClient(configuration["AzureStorage:ConnectionString"]);
		}

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to upload a blob to a specified container asynchronously.
		public async Task UploadBlobAsync(string containerName, string blobName, Stream content)
		{
			try
			{
				// Get the BlobContainerClient for the specified container.
				var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

				// Create the container if it doesn't already exist.
				await containerClient.CreateIfNotExistsAsync();

				// Get a reference to the blob client for the specified blob name.
				var blobClient = containerClient.GetBlobClient(blobName);

				// Upload the content stream to the blob.
				await blobClient.UploadAsync(content, true);
			}
			catch (RequestFailedException ex)
			{
				// Handle specific Azure storage-related errors.
				throw new Exception("Error uploading blob: " + ex.Message);
			}
			catch (Exception ex)
			{
				// General exception handling for any other errors.
				throw new Exception("An error occurred while uploading the blob: " + ex.Message);
			}
		}

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to retrieve a list of all blobs in a specified container asynchronously.
		public async Task<List<string>> GetBlobsAsync(string containerName)
		{
			try
			{
				// Get the BlobContainerClient for the specified container.
				var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
				// Create a list to hold the blob names.
				var blobs = new List<string>();

				// Enumerate through the blobs in the container and add their names to the list.
				await foreach (var blob in containerClient.GetBlobsAsync())
				{
					// Add the name of the blob to the list.
					blobs.Add(blob.Name);
				}

				return blobs; // Return the list of blob names.
			}
			catch (RequestFailedException ex)
			{
				// Handle specific Azure storage-related errors.
				throw new Exception("Error retrieving blobs: " + ex.Message);
			}
			catch (Exception ex)
			{
				// General exception handling for any other errors.
				throw new Exception("An error occurred while retrieving blobs: " + ex.Message);
			}
		}

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to get the URL of a specified blob asynchronously.
		public async Task<string> GetBlobUrlAsync(string containerName, string blobName)
		{
			try
			{
				// Get the BlobContainerClient for the specified container.
				var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

				// Get a reference to the blob client for the specified blob name.
				var blobClient = containerClient.GetBlobClient(blobName);

				// Return the absolute URI of the blob.
				return blobClient.Uri.AbsoluteUri;
			}
			catch (RequestFailedException ex)
			{
				// Handle specific Azure storage-related errors.
				throw new Exception("Error retrieving blob URL: " + ex.Message);
			}
			catch (Exception ex)
			{
				// General exception handling for any other errors.
				throw new Exception("An error occurred while retrieving the blob URL: " + ex.Message);
			}
		}

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		// Method to download a specified blob as a stream asynchronously.
		public async Task<Stream> DownloadBlobAsync(string containerName, string blobName)
		{
			try
			{
				// Get the BlobContainerClient for the specified container.
				var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
				// Get a reference to the blob client for the specified blob name.
				var blobClient = containerClient.GetBlobClient(blobName);

				// Download the blob's content as a stream.
				var download = await blobClient.DownloadAsync();
				var memoryStream = new MemoryStream();
				await download.Value.Content.CopyToAsync(memoryStream); // Copy the blob content to the memory stream.
				memoryStream.Position = 0; // Reset the stream position to the beginning.

				return memoryStream; // Return the memory stream containing the blob's content.
			}
			catch (RequestFailedException ex)
			{
				// Handle specific Azure storage-related errors.
				throw new Exception("Error downloading blob: " + ex.Message);
			}
			catch (Exception ex)
			{
				// General exception handling for any other errors.
				throw new Exception("An error occurred while downloading the blob: " + ex.Message);
			}
		}

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//
	}

}//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°...ooo000 END OF FILE 000ooo...°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//


