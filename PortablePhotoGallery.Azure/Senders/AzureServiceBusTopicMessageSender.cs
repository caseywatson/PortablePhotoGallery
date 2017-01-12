using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using PortablePhotoGallery.Shared.Interfaces;

namespace PortablePhotoGallery.Azure.Senders
{
    public class AzureServiceBusTopicMessageSender<T> : IMessageSender<T>
    {
        private readonly IConfiguration configuration;
        private readonly Lazy<TopicClient> lazyTopicClient;

        public AzureServiceBusTopicMessageSender(IConfiguration configuration)
        {
            this.configuration = configuration;

            lazyTopicClient = new Lazy<TopicClient>(CreateTopicClient);
        }

        public Task SendMessageAsync(T message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return lazyTopicClient.Value.SendAsync(new BrokeredMessage(JsonConvert.SerializeObject(message)));
        }

        private TopicClient CreateTopicClient()
        {
            return TopicClient.CreateFromConnectionString(configuration.ServiceBusConnectionString,
                                                          configuration.TopicName);
        }

        public interface IConfiguration
        {
            string ServiceBusConnectionString { get; }
            string TopicName { get; }
        }

        public class LocalConfiguration
        {
            public LocalConfiguration()
            {
                ServiceBusConnectionString = GetServiceBusConnectionString();
                TopicName = GetTopicName();
            }

            public string ServiceBusConnectionString { get; }
            public string TopicName { get; }

            private string GetServiceBusConnectionString()
            {
                const string key = nameof(ServiceBusConnectionString);
                var connectionString = ConfigurationManager.ConnectionStrings[key];

                if (string.IsNullOrEmpty(connectionString?.ConnectionString))
                    throw new ConfigurationErrorsException($"[{key}] not configured.");

                return connectionString.ConnectionString;
            }

            private string GetTopicName()
            {
                var key = $"{typeof(T).Name}.{nameof(TopicName)}";
                var topicName = ConfigurationManager.AppSettings[key];

                if (string.IsNullOrEmpty(topicName))
                    throw new ConfigurationErrorsException($"[{key}] not configured.");

                return topicName;
            }
        }
    }
}