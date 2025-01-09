using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace NotificationService.NotificationConsumerEndpoints
{
    public class PaymentConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public PaymentConsumer(IConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _channel = _connection.CreateModel();

            // Declare the exchange (this should match the one declared in the publisher)
            _channel.ExchangeDeclare(exchange: "paymentExchange", type: ExchangeType.Fanout);

            // Declare the queue
            _channel.QueueDeclare(queue: "paymentQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            // Bind the queue to the exchange
            _channel.QueueBind(queue: "paymentQueue", exchange: "paymentExchange", routingKey: "");
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received message: {message}");
            };

            // Start consuming messages from the queue
            _channel.BasicConsume(queue: "paymentQueue", autoAck: true, consumer: consumer);
        }
    }
}