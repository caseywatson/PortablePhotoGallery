using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PortablePhotoGallery.Azure.Entities;
using PortablePhotoGallery.Azure.Extensions;
using PortablePhotoGallery.Azure.Interfaces;
using PortablePhotoGallery.Shared.Interfaces;
using PortablePhotoGallery.Shared.Models;

namespace PortablePhotoGallery.Azure.Providers
{
    public class AzureTableStoragePhotoMetadataProvider : IPhotoMetadataProvider
    {
        private readonly IConfiguration providerConfiguration;
        private readonly IAzureStorageConfiguration storageConfiguration;

        private readonly Lazy<CloudStorageAccount> lazyCloudStorageAccount;
        private readonly Lazy<CloudTableClient> lazyCloudTableClient;

        public AzureTableStoragePhotoMetadataProvider(IConfiguration providerConfiguration,
                                                      IAzureStorageConfiguration storageConfiguration)
        {
            this.providerConfiguration = providerConfiguration;
            this.storageConfiguration = storageConfiguration;

            lazyCloudStorageAccount = new Lazy<CloudStorageAccount>(CreateCloudStorageAccount);
            lazyCloudTableClient = new Lazy<CloudTableClient>(CreateCloudTableClient);
        }

        public async Task<IEnumerable<PhotoMetadata>> GetAllPhotoMetadataAsync()
        {
            var photoMetadataList = new List<PhotoMetadata>();

            var table = lazyCloudTableClient.Value.GetTableReference(
                providerConfiguration.PhotoMetadataTableName);

            if (await table.ExistsAsync())
            {
                var queryResults = table.ExecuteQuery(
                    new TableQuery<PhotoMetadataTableEntity>().WhereInPartition(
                        providerConfiguration.AllPhotoMetadataTablePartitionKey));

                photoMetadataList.AddRange(queryResults.Select(e => e.ToPhotoMetadata()));
            }

            return photoMetadataList;
        }

        public async Task<IEnumerable<PhotoMetadata>> GetAllUserPhotoMetadataAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var photoMetadataList = new List<PhotoMetadata>();

            var table = lazyCloudTableClient.Value.GetTableReference(
                providerConfiguration.PhotoMetadataTableName);

            if (await table.ExistsAsync())
            {
                var queryResults = table.ExecuteQuery(
                    new TableQuery<PhotoMetadataTableEntity>().WhereInPartition(userId));

                photoMetadataList.AddRange(queryResults.Select(e => e.ToPhotoMetadata()));
            }

            return photoMetadataList;
        }

        public async Task<PhotoMetadata> GetPhotoMetadataAsync(string photoId)
        {
            if (string.IsNullOrEmpty(photoId))
                throw new ArgumentNullException(nameof(photoId));

            var table = lazyCloudTableClient.Value.GetTableReference(
                providerConfiguration.PhotoMetadataTableName);

            if (await table.ExistsAsync())
            {
                var retrieveOp = TableOperation.Retrieve(
                    providerConfiguration.AllPhotoMetadataTablePartitionKey, photoId);

                var retrieveResult = await table.ExecuteAsync(retrieveOp);

                if (retrieveResult.Result != null)
                    return (retrieveResult.Result as PhotoMetadata);
            }

            return null;
        }

        private CloudStorageAccount CreateCloudStorageAccount()
        {
            return CloudStorageAccount.Parse(storageConfiguration.StorageConnectionString);
        }

        private CloudTableClient CreateCloudTableClient()
        {
            return lazyCloudStorageAccount.Value.CreateCloudTableClient();
        }

        public interface IConfiguration
        {
            string AllPhotoMetadataTablePartitionKey { get; }
            string PhotoMetadataTableName { get; }
        }

        public class LocalConfiguration : IConfiguration
        {
            public LocalConfiguration()
            {
                AllPhotoMetadataTablePartitionKey = GetAllPhotoMetadataTablePartitionKey();
                PhotoMetadataTableName = GetPhotoMetadataTableName();
            }

            public string AllPhotoMetadataTablePartitionKey { get; }
            public string PhotoMetadataTableName { get; }

            private string GetAllPhotoMetadataTablePartitionKey()
            {
                return (ConfigurationManager.AppSettings[nameof(AllPhotoMetadataTablePartitionKey)] ??
                        "all-photos");
            }

            private string GetPhotoMetadataTableName()
            {
                return (ConfigurationManager.AppSettings[nameof(PhotoMetadataTableName)] ??
                        "photo-metadata");
            }
        }
    }
}