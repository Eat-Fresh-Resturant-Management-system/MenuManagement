using RabbitMQ.Client;

namespace MenuManagement.RabbitMQS
{
    public interface IRabbitMQ
    {
        Task ResTable(IModel channel, string rout, CancellationToken cancellationToken);
    }
}
