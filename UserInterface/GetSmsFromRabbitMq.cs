using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace UserInterface
{
    public class GetSmsFromRabbitMq
    {
        public static IConnectionFactory _connectionFactory;

        public void RunWorkerProcessForSmss(string queueName = "smss_to_send")
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                Console.WriteLine(" [*] Opened Channel");

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine($" [*] Waiting for messages from queue {queueName}");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    var messageDeserialized = JsonConvert.DeserializeObject<ErrorMessage>(message);
                    Console.WriteLine($" [x] Deserialized object {messageDeserialized.MessageBody}");
                    using (StreamWriter sw = new StreamWriter(@"ErrorLog.txt",true))
                    {
                        sw.WriteLine(message);
                    }
                    Console.WriteLine($" [x] Received {message}");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" [x] Operation Completed {message}");
                    Console.ForegroundColor = ConsoleColor.Green;

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: queueName,
                    autoAck: false,
                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        public GetSmsFromRabbitMq()
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
        }
    }
}

