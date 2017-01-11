using System.Configuration;
using PortablePhotoGallery.Azure.Interfaces;

namespace PortablePhotoGallery.Azure.Configuration
{
    public class LocalAzureStorageConfiguration : IAzureStorageConfiguration
    {
        public LocalAzureStorageConfiguration()
        {
            StorageConnectionString = GetStorageConnectionString();
        }

        public string StorageConnectionString { get; }

        private string GetStorageConnectionString()
        {
            const string connectionStringKey = nameof(StorageConnectionString);
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringKey];

            if (string.IsNullOrEmpty(connectionString?.ConnectionString))
                throw new ConfigurationErrorsException($"[{connectionStringKey}] not configured.");

            return connectionString.ConnectionString;
        }
    }
}