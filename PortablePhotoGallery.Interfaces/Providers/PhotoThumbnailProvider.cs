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

            return blobRepository.DownloadBlobAsync(configuration.PhotoThumbnailContainerName, photoId);
        }

        public interface IConfiguration
        {
            string PhotoThumbnailContainerName { get; }
        }

        public class LocalConfiguration : IConfiguration
        {
            public LocalConfiguration()
            {
                PhotoThumbnailContainerName = GetPhotoThumbnailContainerName();
            }

            public string PhotoThumbnailContainerName { get; }

            private string GetPhotoThumbnailContainerName()
            {
                return (ConfigurationManager.AppSettings[nameof(PhotoThumbnailContainerName)] ??
                        "photo-thumbnails");
            }
        }
    }
}