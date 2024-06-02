// RabbitMQUnti.cs

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using MenuManagement.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.Entity;

namespace MenuManagement.RabbitMQS
{
    public class RabbitMQUnti : IRabbitMQ
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        // private readonly MenuDbContext _context;

        public RabbitMQUnti(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            //     _context = context;
        }

        public async Task ResTable(IModel channel, string rout, CancellationToken cancellationToken)
        {
            try
            {
                channel.ExchangeDeclare(exchange: "table_booking_exchange", type: ExchangeType.Fanout);

                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueName,
                        exchange: "table_booking_exchange",
                        routingKey: "Table.Booking");

                var con = new AsyncEventingBasicConsumer(channel);
                con.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        string[] messageparts = message.Split('-');

                        // Log modtaget besked
                        Console.WriteLine($"Received message: {message}");
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<MenuDbContext>();
                            // Use dbContext here


                            if (int.TryParse(messageparts[0], out int tabelId))
                            {
                                var tablemessage = await dbContext.TableDatas.FindAsync(tabelId);
                                if (tablemessage != null)
                                {
                                    if (tablemessage.IsAvailable != null)
                                    {
                                        tablemessage.IsAvailable = messageparts[1] ?? tablemessage.IsAvailable;
                                    }
                                    else
                                    {
                                        tablemessage.IsAvailable = messageparts[1];
                                    }

                                    await dbContext.SaveChangesAsync(cancellationToken);

                                    Console.WriteLine($"Update succeeded for ID: {messageparts[0]}, New Status: {messageparts[1]}");
                                }
                                else
                                {
                                    Console.WriteLine($"Tabel with ID {messageparts[0]} not found.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Invalid ID format: {messageparts[0]}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                    }
                };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer: con);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Feil under behandling av meldinger: {ex.Message}");
            }
        }
    }
}
