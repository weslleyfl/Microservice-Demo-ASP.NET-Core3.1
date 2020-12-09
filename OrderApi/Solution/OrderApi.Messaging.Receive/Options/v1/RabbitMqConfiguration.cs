﻿
namespace OrderApi.Messaging.Receive.Options.v1
{
    public class RabbitMqConfiguration
    {
        public const string RabbitMq = "RabbitMq";
        public string Hostname { get; set; }
        public string QueueName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
