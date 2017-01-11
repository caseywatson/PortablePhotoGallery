using System;
using System.Configuration;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using PortablePhotoGallery.Aws.Interfaces;
using PortablePhotoGallery.Shared.Interfaces;

namespace PortablePhotoGallery.Aws.Providers
{
    public class AwsS3BlobUrlProvider : IBlobUrlProvider
    {
        private readonly IAwsConfiguration awsConfiguration;
        private readonly IAwsRegionEndpointProvider endpointProvider;
        private readonly IBlobUrlProviderConfiguration providerConfiguration;

        private readonly Lazy<AmazonS3Client> lazyS3Client;

        public AwsS3BlobUrlProvider(IAwsConfiguration awsConfiguration,
                                    IAwsRegionEndpointProvider endpointProvider,
                                    IBlobUrlProviderConfiguration providerConfiguration)
        {
            this.awsConfiguration = awsConfiguration;
            this.endpointProvider = endpointProvider;
            this.providerConfiguration = providerConfiguration;

            lazyS3Client = new Lazy<AmazonS3Client>(CreateS3Client);
        }

        public string GetBlobUrl(string containerName, string blobName)
        {
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException(nameof(containerName));

            if (string.IsNullOrEmpty(blobName))
                throw new ArgumentNullException(nameof(blobName));

            var urlRequest = new GetPreSignedUrlRequest
            {
                BucketName = containerName,
                Key = blobName,
                Expires = DateTime.UtcNow.Add(providerConfiguration.BlobUrlExpiryInterval)
            };

            return lazyS3Client.Value.GetPreSignedURL(urlRequest);
        }

        private AmazonS3Client CreateS3Client()
        {
            var awsCredentials = new BasicAWSCredentials(awsConfiguration.AwsAccessKeyId,
                                                         awsConfiguration.AwsSecretAccessKey);

            var awsEndpoint = endpointProvider.GetRegionEndpoint(awsConfiguration.AwsRegionName);

            if (awsEndpoint == null)
                throw new ConfigurationErrorsException($"[{awsConfiguration.AwsRegionName}] is not a valid AWS region.");

            return new AmazonS3Client(awsCredentials, awsEndpoint);
        }
    }
}