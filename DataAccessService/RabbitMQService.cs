using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

public class RabbitMQService
{
    private readonly string _hostName = "localhost"; // Адрес RabbitMQ
    private readonly string _queueName = "busQueue"; // Название очереди
    private readonly string _userName = "guest";     // Имя пользователя RabbitMQ
    private readonly string _password = "guest";     // Пароль пользователя RabbitMQ

    private IConnection _connection; // Соединение с RabbitMQ
    private IModel _channel;         // Канал для общения с RabbitMQ

    public RabbitMQService()
    {
        Initialize(); // Инициализация при создании экземпляра
    }
    public void Initialize()
    {
        try
        {
            // Настраиваем подключение
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            // Создаем соединение и канал
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Объявляем очередь
            _channel.QueueDeclare(queue: _queueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            Console.WriteLine($"RabbitMQ initialized. Queue: {_queueName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing RabbitMQ: {ex.Message}");
        }
    }

    
    public void SendMessage(string message)
    {
        if (_channel == null)
        {
            Console.WriteLine("RabbitMQ channel is not initialized.");
            return;
        }

        try
        {
            var body = Encoding.UTF8.GetBytes(message);

            // Публикуем сообщение
            _channel.BasicPublish(exchange: "",
                                  routingKey: _queueName,
                                  basicProperties: null,
                                  body: body);

            Console.WriteLine($" [x] Sent: {message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    
    public void ReceiveMessages()
    {
        if (_channel == null)
        {
            Console.WriteLine("RabbitMQ channel is not initialized.");
            return;
        }

        try
        {
            var consumer = new EventingBasicConsumer(_channel);

            // Обработка полученного сообщения
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received: {message}");
            };

            // Запускаем потребление
            _channel.BasicConsume(queue: _queueName,
                                  autoAck: true,
                                  consumer: consumer);

            Console.WriteLine("Waiting for messages...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving messages: {ex.Message}");
        }
    }

    public void Close()
    {
        try
        {
            _channel?.Close();
            _connection?.Close();
            Console.WriteLine("RabbitMQ connection closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing RabbitMQ connection: {ex.Message}");
        }
    }
}
