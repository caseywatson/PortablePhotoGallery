using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PortablePhotoGallery.Azure.Interfaces;
using PortablePhotoGallery.Shared.Interfaces;

namespace PortablePhotoGallery.Azure.Repositories
{
    public class AzureBlobRepository : IBlobRepository
    {
        private readonly Lazy<CloudBlobClient> lazyCloudBlobClient;
        private readonly Lazy<CloudStorageAccount> lazyCloudStorageAccount;
        private readonly IAzureStorageConfiguration storageConfiguration;

        public AzureBlobRepository(IAzureStorageConfiguration storageConfiguration)
        {
            this.storageConfiguration = storageConfiguration;

            lazyCloudStorageAccount = new Lazy<CloudStorageAccount>(CreateCloudStorageAccount);
            lazyCloudBlobClient = new Lazy<CloudBlobClient>(CreateCloudBlobClient);
        }

        public async Task<Stream> DownloadBlobAsync(string containerName, string blobName)
        {
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException(nameof(containerName));

            if (string.IsNullOrEmpty(blobName))
                throw new ArgumentNullException(nameof(blobName));

            var outputStream = new MemoryStream();
            var container = lazyCloudBlobClient.Value.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(blobName);

            await blob.DownloadToStreamAsync(outputStream);

            outputStream.Position = 0;

            return outputStream;
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream blobStream)
        {
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException(nameof(containerName));

            if (string.IsNullOrEmpty(blobName))
                throw new ArgumentNullException(nameof(blobName));

            if ((blobStream == null) || (blobStream.Length == 0))
                throw new ArgumentNullException(nameof(blobStream));

            if (blobStream.CanSeek)
                blobStream.Position = 0;

            var container = lazyCloudBlobClient.Value.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(blobName);

            await container.CreateIfNotExistsAsync();
            await blob.UploadFromStreamAsync(blobStream);
        }

        private CloudStorageAccount CreateCloudStorageAccount()
        {
            return CloudStorageAccount.Parse(storageConfiguration.StorageConnectionString);
        }

        private CloudBlobClient CreateCloudBlobClient()
        {
            return lazyCloudStorageAccount.Value.CreateCloudBlobClient();
        }
    }
}