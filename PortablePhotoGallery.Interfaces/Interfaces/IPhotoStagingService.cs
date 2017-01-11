using System.IO;
using System.Threading.Tasks;
using PortablePhotoGallery.Shared.Models;

namespace PortablePhotoGallery.Shared.Interfaces
{
    public interface IPhotoStagingService
    {
        Task<string> StagePhotoAsync(PhotoMetadata photoMetadata, Stream photoStream);
    }
}