using System;
using System.Configuration;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Newtonsoft.Json;
using PortablePhotoGallery.Aws.Interfaces;
using PortablePhotoGallery.Shared.Interfaces;

namespace PortablePhotoGallery.Aws.Senders
{
    public class AwsSnsTopicMessageSender<T> : IMessageSender<T>
    {
        private readonly IAwsConfiguration awsConfiguration;
        private readonly IAwsRegionEndpointProvider endpointProvider;

        private readonly Lazy<AmazonSimpleNotificationServiceClient> lazySnsClient;
        private readonly IConfiguration senderConfiguration;

        public AwsSnsTopicMessageSender(IAwsConfiguration awsConfiguration,
                                        IAwsRegionEndpointProvider endpointProvider,
                                        IConfiguration senderConfiguration)
        {
            this.awsConfiguration = awsConfiguration;
            this.endpointProvider = endpointProvider;
            this.senderConfiguration = senderConfiguration;

            lazySnsClient = new Lazy<AmazonSimpleNotificationServiceClient>(CreateSnsClient);
        }

        public Task SendMessageAsync(T message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return lazySnsClient.Value.PublishAsync(senderConfiguration.TopicArn,
                                                    JsonConvert.SerializeObject(message));
        }

        private AmazonSimpleNotificationServiceClient CreateSnsClient()
        {
            var awsCredentials = new BasicAWSCredentials(awsConfiguration.AwsAccessKeyId,
                                                         awsConfiguration.AwsSecretAccessKey);

            var awsEndpoint = endpointProvider.GetRegionEndpoint(awsConfiguration.AwsRegionName);

            if (awsEndpoint == null)
                throw new ConfigurationErrorsException($"[{awsConfiguration.AwsRegionName}] is not a valid AWS region.");

            return new AmazonSimpleNotificationServiceClient(awsCredentials, awsEndpoint);
        }

        public interface IConfiguration
        {
            string TopicArn { get; }
        }

        public class LocalConfiguration : IConfiguration
        {
            public LocalConfiguration()
            {
                TopicArn = GetTopicArn();
            }

            public string TopicArn { get; }

            private string GetTopicArn()
            {
                var appSettingKey = $"{typeof(T).Name}.{nameof(TopicArn)}";
                var topicArn = ConfigurationManager.AppSettings[appSettingKey];

                if (string.IsNullOrEmpty(topicArn))
                    throw new ConfigurationErrorsException($"[{appSettingKey}] not configured.");

                return topicArn;
            }
        }
    }
}