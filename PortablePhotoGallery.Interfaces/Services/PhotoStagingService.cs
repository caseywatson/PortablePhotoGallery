using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using PortablePhotoGallery.Shared.Interfaces;
using PortablePhotoGallery.Shared.Models;

namespace PortablePhotoGallery.Shared.Services
{
    public class PhotoStagingService : IPhotoStagingService
    {
        private readonly IBlobRepository blobRepository;
        private readonly IBlobUrlProvider blobUrlProvider;
        private readonly IConfiguration configuration;

        public async Task<string> StagePhotoAsync(PhotoMetadata photoMetadata, Stream photoStream)
        {
            if (photoMetadata == null)
                throw new ArgumentNullException(nameof(photoMetadata));

            if ((photoStream == null) || (photoStream.Length == 0))
                throw new ArgumentNullException(nameof(photoStream));

            await blobRepository.UploadBlobAsync(configuration.PhotoStagingContainerName,
                                                 photoMetadata.PhotoId, photoStream);

            return blobUrlProvider.GetBlobUrl(configuration.PhotoStagingContainerName,
                                              photoMetadata.PhotoId);
        }

        public interface IConfiguration
        {
            string PhotoStagingContainerName { get; }
        }

        public class LocalConfiguration : IConfiguration
        {
            public LocalConfiguration()
            {
                PhotoStagingContainerName = GetPhotoStagingContainerName();
            }

            public string PhotoStagingContainerName { get; }

            private string GetPhotoStagingContainerName()
            {
                return (ConfigurationManager.AppSettings[nameof(PhotoStagingContainerName)] ??
                        "photos-staging");
            }
        }
    }
}