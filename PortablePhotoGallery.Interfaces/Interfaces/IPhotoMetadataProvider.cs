using System.Collections.Generic;
using System.Threading.Tasks;
using PortablePhotoGallery.Shared.Models;

namespace PortablePhotoGallery.Shared.Interfaces
{
    public interface IPhotoMetadataProvider
    {
        Task<PhotoMetadata> GetPhotoMetadataAsync(string photoId);
        Task<IEnumerable<PhotoMetadata>> GetAllPhotoMetadataAsync();
        Task<IEnumerable<PhotoMetadata>> GetAllUserPhotoMetadataAsync(string userId);
    }
}