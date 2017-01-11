using System.IO;
using System.Threading.Tasks;

namespace PortablePhotoGallery.Shared.Interfaces
{
    public interface IPhotoProvider
    {
        Task<Stream> DownloadPhotoAsync(string photoId);
    }
}