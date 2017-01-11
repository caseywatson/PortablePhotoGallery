namespace PortablePhotoGallery.Aws.Interfaces
{
    public interface IAwsConfiguration
    {
        string AwsAccessKeyId { get; }
        string AwsSecretAccessKey { get; }
        string AwsRegionName { get; }
    }
}