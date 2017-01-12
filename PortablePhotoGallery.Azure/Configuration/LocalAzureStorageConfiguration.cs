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
            const string key = nameof(StorageConnectionString);
            var connectionString = ConfigurationManager.ConnectionStrings[key];

            if (string.IsNullOrEmpty(connectionString?.ConnectionString))
                throw new ConfigurationErrorsException($"[{key}] not configured.");

            return connectionString.ConnectionString;
        }
    }
}