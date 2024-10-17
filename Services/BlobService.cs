using Azure;
using Azure.Storage.Blobs;

namespace CMCSapp_ST10311777.Services
{
    public class BlobService
    {
	    //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		private readonly BlobServiceClient _blobServiceClient;

		//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		public BlobService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration["AzureStorage:ConnectionString"]);
        }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		public async Task UploadBlobAsync(string containerName, string blobName, Stream content)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();
                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.UploadAsync(content, true);
            }
            catch (RequestFailedException ex)
            {
                // Handle specific Azure storage-related errors
                throw new Exception("Error uploading blob: " + ex.Message);
            }
            catch (Exception ex)
            {
                // General exception handling
                throw new Exception("An error occurred while uploading the blob: " + ex.Message);
            }
        }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		public async Task<List<string>> GetBlobsAsync(string containerName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobs = new List<string>();

                await foreach (var blob in containerClient.GetBlobsAsync())
                {
                    blobs.Add(blob.Name);
                }

                return blobs;
            }
            catch (RequestFailedException ex)
            {
                throw new Exception("Error retrieving blobs: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving blobs: " + ex.Message);
            }
        }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		public async Task<string> GetBlobUrlAsync(string containerName, string blobName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                return blobClient.Uri.AbsoluteUri;
            }
            catch (RequestFailedException ex)
            {
                throw new Exception("Error retrieving blob URL: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the blob URL: " + ex.Message);
            }
        }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//

		public async Task<Stream> DownloadBlobAsync(string containerName, string blobName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                var download = await blobClient.DownloadAsync();
                var memoryStream = new MemoryStream();
                await download.Value.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                return memoryStream;
            }
            catch (RequestFailedException ex)
            {
                throw new Exception("Error downloading blob: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while downloading the blob: " + ex.Message);
            }
        }

        //°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//
	}
}//°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°...ooo000 END OF FILE 000ooo...°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°//


