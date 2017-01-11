using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using PortablePhotoGallery.Shared.Interfaces;

namespace PortablePhotoGallery.Shared.Providers
{
    public class PhotoProvider : IPhotoProvider
    {
        private readonly IBlobRepository blobRepository;
        private readonly IConfiguration configuration;

        public PhotoProvider(IBlobRepository blobRepository,
                             IConfiguration configuration)
        {
            this.blobRepository = blobRepository;
            this.configuration = configuration;
        }

        public Task<Stream> DownloadPhotoAsync(string photoId)
        {
            if (string.IsNullOrEmpty(photoId))
                throw new ArgumentNullException(nameof(photoId));

            return blobRepository.DownloadBlobAsync(configuration.PhotosContainerName, photoId);
        }

        public interface IConfiguration
        {
            string PhotosContainerName { get; }
        }

        public class LocalConfiguration : IConfiguration
        {
            public LocalConfiguration()
            {
                PhotosContainerName = GetPhotosContainerName();
            }

            public string PhotosContainerName { get; }

            private string GetPhotosContainerName()
            {
                return (ConfigurationManager.AppSettings[nameof(PhotosContainerName)] ??
                        "photos");
            }
        }
    }
}