using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using PortablePhotoGallery.Aws.Extensions;
using PortablePhotoGallery.Aws.Interfaces;
using PortablePhotoGallery.Shared.Interfaces;
using PortablePhotoGallery.Shared.Models;

namespace PortablePhotoGallery.Aws.Providers
{
    public class AwsSimpleDbPhotoMetadataProvider : IPhotoMetadataProvider
    {
        private readonly IAwsConfiguration awsConfiguration;
        private readonly IAwsRegionEndpointProvider endpointProvider;
        private readonly IConfiguration providerConfiguration;

        private readonly Lazy<AmazonSimpleDBClient> lazySimpleDbClient;

        public AwsSimpleDbPhotoMetadataProvider(IAwsConfiguration awsConfiguration,
                                                IAwsRegionEndpointProvider endpointProvider,
                                                IConfiguration providerConfiguration)
        {
            this.awsConfiguration = awsConfiguration;
            this.endpointProvider = endpointProvider;
            this.providerConfiguration = providerConfiguration;

            lazySimpleDbClient = new Lazy<AmazonSimpleDBClient>(CreateSimpleDbClient);
        }

        public async Task<IEnumerable<PhotoMetadata>> GetAllPhotoMetadataAsync()
        {
            var selectRequest = new SelectRequest(
                $"SELECT * FROM '{providerConfiguration.AllPhotosMetadataDomainName}'", true);

            var selectResult = await lazySimpleDbClient.Value.SelectAsync(selectRequest);

            return selectResult.Items.Select(i => i.ToPhotoMetadata());
        }

        public async Task<IEnumerable<PhotoMetadata>> GetAllUserPhotoMetadataAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var selectRequest = new SelectRequest($"SELECT * FROM '{userId}'", true);
            var selectResult = await lazySimpleDbClient.Value.SelectAsync(selectRequest);

            return selectResult.Items.Select(i => i.ToPhotoMetadata());
        }

        public async Task<PhotoMetadata> GetPhotoMetadataAsync(string photoId)
        {
            if (string.IsNullOrEmpty(photoId))
                throw new ArgumentNullException(nameof(photoId));

            var getRequest = new GetAttributesRequest(
                providerConfiguration.AllPhotosMetadataDomainName, photoId);

            var getResult = await lazySimpleDbClient.Value.GetAttributesAsync(getRequest);

            if (getResult.Attributes.Any())
                return new Item(photoId, getResult.Attributes).ToPhotoMetadata();

            return null;
        }

        private AmazonSimpleDBClient CreateSimpleDbClient()
        {
            var awsCredentials = new BasicAWSCredentials(awsConfiguration.AwsAccessKeyId,
                                                         awsConfiguration.AwsSecretAccessKey);

            var awsEndpoint = endpointProvider.GetRegionEndpoint(awsConfiguration.AwsRegionName);

            if (awsEndpoint == null)
                throw new ConfigurationErrorsException($"[{awsConfiguration.AwsRegionName}] is not a valid AWS region.");

            return new AmazonSimpleDBClient(awsCredentials, awsEndpoint);
        }

        public interface IConfiguration
        {
            string AllPhotosMetadataDomainName { get; }
        }

        public class LocalConfiguration : IConfiguration
        {
            public LocalConfiguration()
            {
                AllPhotosMetadataDomainName = GetAllPhotosMetadataDomainName();
            }

            public string AllPhotosMetadataDomainName { get; }

            private string GetAllPhotosMetadataDomainName()
            {
                return (ConfigurationManager.AppSettings[nameof(AllPhotosMetadataDomainName)] ??
                        "all-photos");
            }
        }
    }
}