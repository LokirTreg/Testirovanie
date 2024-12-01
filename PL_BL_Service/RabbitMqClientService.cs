using System;
using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PL_BL_Service
{
    public class RabbitMqClientService
    {
        private IModel _channel;
        private string _requestQueue;
        private string _responseQueue;

        public RabbitMqClientService(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };

            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _requestQueue = configuration["RabbitMQ:RequestQueue"];
            _responseQueue = configuration["RabbitMQ:ResponseQueue"];

            // Убедитесь, что очереди существуют
            _channel.QueueDeclare(_requestQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(_responseQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void SendMessage(string message)
        {
            if (_channel == null || !_channel.IsOpen)
            {
                throw new InvalidOperationException("Канал RabbitMQ не открыт.");
            }

            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: _requestQueue, basicProperties: null, body: body);
        }

        public string ReceiveMessage()
        {
            var result = _channel.BasicGet(queue: _responseQueue, autoAck: true);
            if (result != null)
                Console.WriteLine($"- Из очереди извлечено {Encoding.UTF8.GetString(result.Body.ToArray())}");
            return result != null ? Encoding.UTF8.GetString(result.Body.ToArray()) : null;
        }
        public async Task<string> ReceiveMessageAsync()
        {
            //_channel.QueueDeclare(_responseQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var tcs = new TaskCompletionSource<string>();
            var consumer = new EventingBasicConsumer(_channel);

            EventHandler<BasicDeliverEventArgs> handler = null; // Локальная переменная для обработчика

            handler = (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received: {message}");

                // Удаляем обработчик, чтобы остановить прослушивание
                consumer.Received -= handler;
                
                tcs.TrySetResult(message); // Завершаем задачу

                _channel.BasicCancel(ea.ConsumerTag);
                //_channel.QueueDelete(_responseQueue);
            };

            consumer.Received += handler;
            _channel.BasicConsume(queue: "busQueueResponse", autoAck: true, consumer: consumer);
            return await tcs.Task; // Возвращаем значение, когда оно установлено
        }
    }
}
