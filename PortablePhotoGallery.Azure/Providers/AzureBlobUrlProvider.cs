using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PortablePhotoGallery.Azure.Interfaces;
using PortablePhotoGallery.Shared.Interfaces;

namespace PortablePhotoGallery.Azure.Providers
{
    public class AzureBlobUrlProvider : IBlobUrlProvider
    {
        private readonly IBlobUrlProviderConfiguration providerConfiguration;
        private readonly IAzureStorageConfiguration storageConfiguration;

        private readonly Lazy<CloudBlobClient> lazyCloudBlobClient;
        private readonly Lazy<CloudStorageAccount> lazyCloudStorageAccount;

        public AzureBlobUrlProvider(IBlobUrlProviderConfiguration providerConfiguration,
                                    IAzureStorageConfiguration storageConfiguration)
        {
            this.providerConfiguration = providerConfiguration;
            this.storageConfiguration = storageConfiguration;

            lazyCloudStorageAccount = new Lazy<CloudStorageAccount>(CreateCloudStorageAccount);
            lazyCloudBlobClient = new Lazy<CloudBlobClient>(CreateCloudBlobClient);
        }

        public string GetBlobUrl(string containerName, string blobName)
        {
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException(nameof(containerName));

            if (string.IsNullOrEmpty(blobName))
                throw new ArgumentNullException(nameof(blobName));

            var container = lazyCloudBlobClient.Value.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(blobName);

            var sasPolicy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.Add(providerConfiguration.BlobUrlExpiryInterval),
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5)
            };

            return (blob.Uri + blob.GetSharedAccessSignature(sasPolicy));
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