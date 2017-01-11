using Amazon;

namespace PortablePhotoGallery.Aws.Interfaces
{
    public interface IAwsRegionEndpointProvider
    {
        RegionEndpoint GetRegionEndpoint(string regionName);
    }
}