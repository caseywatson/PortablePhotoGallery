using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using PortablePhotoGallery.Aws.Interfaces;
using PortablePhotoGallery.Shared.Interfaces;

namespace PortablePhotoGallery.Aws.Repositories
{
    public class AwsS3BlobRepository : IBlobRepository
    {
        private readonly IAwsConfiguration awsConfiguration;
        private readonly IAwsRegionEndpointProvider endpointProvider;
        private readonly Lazy<AmazonS3Client> lazyS3Client;

        public AwsS3BlobRepository(IAwsConfiguration awsConfiguration,
                                   IAwsRegionEndpointProvider endpointProvider)
        {
            this.awsConfiguration = awsConfiguration;
            this.endpointProvider = endpointProvider;

            lazyS3Client = new Lazy<AmazonS3Client>(CreateS3Client);
        }

        public async Task<Stream> DownloadBlobAsync(string containerName, string blobName)
        {
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException(nameof(containerName));

            if (string.IsNullOrEmpty(blobName))
                throw new ArgumentNullException(nameof(blobName));

            var outputStream = new MemoryStream();
            var getObjectResponse = await lazyS3Client.Value.GetObjectAsync(containerName, blobName);

            getObjectResponse.ResponseStream.CopyTo(outputStream);

            outputStream.Position = 0;

            return outputStream;
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream blobStream)
        {
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException(nameof(containerName));

            if (string.IsNullOrEmpty(blobName))
                throw new ArgumentNullException(nameof(blobName));

            if ((blobStream == null) || (blobStream.Length == 0))
                throw new ArgumentNullException(nameof(blobStream));

            if (blobStream.CanSeek)
                blobStream.Position = 0;

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = containerName,
                InputStream = blobStream,
                Key = blobName
            };

            await CreateContainerIfNotExistsAsync(containerName);
            await lazyS3Client.Value.PutObjectAsync(putObjectRequest);
        }

        private async Task CreateContainerIfNotExistsAsync(string containerName)
        {
            if (await DoesContainerExistAsync(containerName) == false)
                await CreateContainerAsync(containerName);
        }

        private async Task CreateContainerAsync(string containerName)
        {
            await lazyS3Client.Value.PutBucketAsync(containerName);
        }

        private async Task<bool> DoesContainerExistAsync(string containerName)
        {
            var listBucketsResult = await lazyS3Client.Value.ListBucketsAsync();

            return (listBucketsResult.Buckets?.Any(b => b.BucketName == containerName) == true);
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