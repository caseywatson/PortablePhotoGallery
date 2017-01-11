using System.IO;
using System.Threading.Tasks;

namespace PortablePhotoGallery.Shared.Interfaces
{
    public interface IPhotoThumbnailProvider
    {
        Task<Stream> DownloadPhotoThumbnailAsync(string photoId);
    }
}