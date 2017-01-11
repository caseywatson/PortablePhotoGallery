using System.IO;
using System.Threading.Tasks;

namespace PortablePhotoGallery.Shared.Interfaces
{
    public interface IBlobRepository
    {
        Task<Stream> DownloadBlobAsync(string containerName, string blobName);
        Task UploadBlobAsync(string containerName, string blobName, Stream blobStream);
    }
}