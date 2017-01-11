using System;
using System.Threading.Tasks;

namespace PortablePhotoGallery.Shared.Interfaces
{
    public interface IBlobUrlProvider
    {
        string GetBlobUrl(string containerName, string blobName);
    }
}