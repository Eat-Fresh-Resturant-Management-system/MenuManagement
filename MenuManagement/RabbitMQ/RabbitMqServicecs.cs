// RabbitMqService.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace MenuManagement.RabbitMQS
{
    public class RabbitMqService : BackgroundService
    {
        private readonly IRabbitMQ _rabbitMQ;
        private IModel _channel;
        private IConnection _connection;

        public RabbitMqService(IRabbitMQ rabbitMQ)
        {
            _rabbitMQ = rabbitMQ;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@rabbitmq.eatfresh.svc.cluster.local:5672/"),
                DispatchConsumersAsync = true,
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitMQ.ResTable(_channel, "Table.Booking", stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.Close();
            return base.StopAsync(cancellationToken);
        }
    }
}
