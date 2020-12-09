using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderApi.Messaging.Receive.Options.v1;
using OrderApi.Service.v1.Models;
using OrderApi.Service.v1.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderApi.Messaging.Receive.Receiver.v1
{
    /// <summary>
    /// Background Services no .NET Core executam tarefas em segundo plano.
    /// Escutar eventos do MessageQueue
    /// É uma solução simples e atende cenários básicos. Como mandar um e-mail, gerar um relatório etc.
    /// https://www.programmingwithwolfgang.com/rabbitmq-in-an-asp-net-core-3-1-microservice/
    /// </summary>
    public class CustomerFullNameUpdateReceiver : BackgroundService
    {
        private IModel _channel;
        private IConnection _connection;
        private readonly ICustomerNameUpdateService _customerNameUpdateService;
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;
        private readonly ILogger<CustomerFullNameUpdateReceiver> _logger;

        public CustomerFullNameUpdateReceiver(
            ICustomerNameUpdateService customerNameUpdateService,
            IOptions<RabbitMqConfiguration> rabbitMqOptions,
            ILogger<CustomerFullNameUpdateReceiver> logger)
        {

            _customerNameUpdateService = customerNameUpdateService;
            _hostname = rabbitMqOptions.Value.Hostname;
            _queueName = rabbitMqOptions.Value.QueueName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            _logger = logger;

            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _queueName,
                          durable: false,
                          exclusive: false,
                          autoDelete: false,
                          arguments: null);
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("Evento disparado: RabbitMQ_ConnectionShutdown");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExecuteAsync - Timed Hosted Service running.");

            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                // lendo o conteudo
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var updateCustomerFullNameModel = JsonConvert.DeserializeObject<UpdateCustomerFullNameModel>(message);

                // Then I am using this Customer object to call another service that will do the update in the database.
                HandleMessage(updateCustomerFullNameModel);

                _channel.BasicAck(ea.DeliveryTag, false);

            };

            // I register events that I won’t implement now but might be useful in the future.
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(queue: _queueName,
                                 autoAck: false,
                                 consumer: consumer);

            return Task.CompletedTask;
        }

        private void HandleMessage(UpdateCustomerFullNameModel updateCustomerFullNameModel)
        {
            _logger.LogInformation($"Serviço recebe mensagem {nameof(_customerNameUpdateService)} - Alterando o nome do cliente no pedido");
            _customerNameUpdateService.UpdateCustomerNameInOrders(updateCustomerFullNameModel);
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation("Evento disparado: OnConsumerConsumerCancelled");
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation("Evento disparado: OnConsumerUnregistered");
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation("Evento disparado: OnConsumerRegistered");
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("Evento disparado: OnConsumerShutdown");
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }


    }
}
