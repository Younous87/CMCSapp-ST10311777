using Azure.Storage.Blobs;

namespace CMCSapp_ST10311777.Services
{
	public class BlobService
	{
		private readonly BlobServiceClient _blobServiceClient;

		public BlobService(IConfiguration configuration)
		{
			_blobServiceClient = new BlobServiceClient(configuration["AzureStorage:ConnectionString"]);
		}

		public async Task UploadBlobAsync(string containerName, string blobName, Stream content)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
			await containerClient.CreateIfNotExistsAsync();
			var blobClient = containerClient.GetBlobClient(blobName);
			await blobClient.UploadAsync(content, true);
		}

		public async Task<List<string>> GetBlobsAsync(string containerName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
			var blobs = new List<string>();

			await foreach (var blob in containerClient.GetBlobsAsync())
			{
				blobs.Add(blob.Name);
			}

			return blobs;
		}

		public async Task<string> GetBlobUrlAsync(string containerName, string blobName)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
			var blobClient = containerClient.GetBlobClient(blobName);
			return blobClient.Uri.AbsoluteUri;
		}
	}
}


