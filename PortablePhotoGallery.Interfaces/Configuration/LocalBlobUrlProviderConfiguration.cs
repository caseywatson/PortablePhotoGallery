using System;
using System.Configuration;
using PortablePhotoGallery.Shared.Interfaces;

namespace PortablePhotoGallery.Shared.Configuration
{
    public class LocalBlobUrlProviderConfiguration : IBlobUrlProviderConfiguration
    {
        public LocalBlobUrlProviderConfiguration()
        {
            BlobUrlExpiryInterval = GetBlobUrlExpiryInterval();
        }

        public TimeSpan BlobUrlExpiryInterval { get; }

        private TimeSpan GetBlobUrlExpiryInterval()
        {
            var cfgValue = ConfigurationManager.AppSettings[nameof(BlobUrlExpiryInterval)];

            if (string.IsNullOrEmpty(cfgValue) == false)
            {
                TimeSpan tempTimeSpan;

                if (TimeSpan.TryParse(cfgValue, out tempTimeSpan) == false)
                {
                    throw new ConfigurationErrorsException(
                        $"[{nameof(BlobUrlExpiryInterval)}] configuration is invalid. " +
                        $"[{cfgValue}] can not be converted to a [TimeSpan].");
                }

                return tempTimeSpan;
            }

            return TimeSpan.FromHours(1);
        }
    }
}