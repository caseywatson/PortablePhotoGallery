using System.Configuration;
using PortablePhotoGallery.Aws.Interfaces;

namespace PortablePhotoGallery.Aws.Configuration
{
    public class LocalAwsConfiguration : IAwsConfiguration
    {
        public LocalAwsConfiguration()
        {
            AwsAccessKeyId = GetAwsAccessKeyId();
            AwsSecretAccessKey = GetAwsSecretAccessKey();
            AwsRegionName = GetAwsRegionName();
        }

        public string AwsAccessKeyId { get; }
        public string AwsSecretAccessKey { get; }
        public string AwsRegionName { get; }

        private string GetAwsAccessKeyId()
        {
            const string appSettingKey = nameof(AwsAccessKeyId);
            var accessKeyId = ConfigurationManager.AppSettings[appSettingKey];

            if (string.IsNullOrEmpty(accessKeyId))
                throw new ConfigurationErrorsException($"[{appSettingKey}] not configured.");

            return accessKeyId;
        }

        private string GetAwsSecretAccessKey()
        {
            const string appSettingKey = nameof(AwsSecretAccessKey);
            var secretAccessKey = ConfigurationManager.AppSettings[appSettingKey];

            if (string.IsNullOrEmpty(secretAccessKey))
                throw new ConfigurationErrorsException($"[{appSettingKey}] not configured.");

            return secretAccessKey;
        }

        private string GetAwsRegionName()
        {
            const string appSettingKey = nameof(AwsRegionName);
            var regionName = ConfigurationManager.AppSettings[appSettingKey];

            if (string.IsNullOrEmpty(regionName))
                throw new ConfigurationErrorsException($"[{appSettingKey}] not configured.");

            return regionName;
        }
    }
}