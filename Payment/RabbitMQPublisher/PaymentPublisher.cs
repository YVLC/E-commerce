using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Payment.RabbitMQPublisher
{
    public class PaymentPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public PaymentPublisher(IConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));

            _channel = _connection.CreateModel();

            // Declare the exchange (ensures it exists)
            _channel.ExchangeDeclare(exchange: "paymentExchange", type: ExchangeType.Fanout);

            // Declare a queue (this should be done in the consumer side)
            _channel.QueueDeclare(queue: "paymentQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            // Bind the queue to the exchange
            _channel.QueueBind(queue: "paymentQueue", exchange: "paymentExchange", routingKey: "");
        }

        public void PublishPaymentProcessed(string paymentId, string orderId, string customerEmail)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                throw new ArgumentException("Payment ID cannot be null or empty", nameof(paymentId));
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("Order ID cannot be null or empty", nameof(orderId));
            if (string.IsNullOrWhiteSpace(customerEmail))
                throw new ArgumentException("Customer email cannot be null or empty", nameof(customerEmail));

            var message = new
            {
                PaymentId = paymentId,
                OrderId = orderId,
                CustomerEmail = customerEmail
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            // Publish the message to the exchange
            _channel.BasicPublish(
                exchange: "paymentExchange",
                routingKey: "",
                basicProperties: null,
                body: body);

            Console.WriteLine($"[PaymentPublisher] Published payment: {paymentId} for Order: {orderId}");
        }

        public void Dispose()
        {
            // Close the channel and connection
            _channel?.Close();
            _connection?.Close();
        }
    }
}