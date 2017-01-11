using System;

namespace PortablePhotoGallery.Shared.Interfaces
{
    public interface IBlobUrlProviderConfiguration
    {
        TimeSpan BlobUrlExpiryInterval { get; }
    }
}