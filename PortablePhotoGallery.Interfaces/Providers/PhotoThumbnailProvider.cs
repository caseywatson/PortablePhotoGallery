using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using PortablePhotoGallery.Shared.Interfaces;

namespace PortablePhotoGallery.Shared.Providers
{
    public class PhotoThumbnailProvider : IPhotoThumbnailProvider
    {
        private readonly IBlobRepository blobRepository;
        private readonly IConfiguration configuration;

        public PhotoThumbnailProvider(IBlobRepository blobRepository,
                                      IConfiguration configuration)
        {
            this.blobRepository = blobRepository;
            this.configuration = configuration;
        }

        public Task<Stream> DownloadPhotoThumbnailAsync(string photoId)
        {
            if (string.IsNullOrEmpty(photoId))
                throw new ArgumentNullException(nameof(photoId));

            return blobRepository.DownloadBlobAsync(configuration.PhotoThumbnailsContainerName, photoId);
        }

        public interface IConfiguration
        {
            string PhotoThumbnailsContainerName { get; }
        }

        public class LocalConfiguration : IConfiguration
        {
            public LocalConfiguration()
            {
                PhotoThumbnailsContainerName = GetPhotoThumbnailsContainerName();
            }

            public string PhotoThumbnailsContainerName { get; }

            private string GetPhotoThumbnailsContainerName()
            {
                return (ConfigurationManager.AppSettings[nameof(PhotoThumbnailsContainerName)] ??
                        "photo-thumbnails");
            }
        }
    }
}