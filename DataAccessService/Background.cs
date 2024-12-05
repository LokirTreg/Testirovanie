using DataAccessService.Data;
using Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace DataAccessService
{
    public class Background : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private const string QueueName = "busQueue";
        private const string ResponseQueueName = "busQueueResponse";
        private readonly IConnection _rabbitConnection;
        private readonly IModel _rabbitChannel;

        public Background(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _rabbitConnection = factory.CreateConnection();
            _rabbitChannel = _rabbitConnection.CreateModel();
            InitializeRabbitMq();
        }

        private void InitializeRabbitMq()
        {
            _rabbitChannel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _rabbitChannel.QueueDeclare(queue: ResponseQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_rabbitChannel);
            consumer.Received += async (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"[x] Получено сообщение: {message}");

                var response = await ProcessMessageAsync(message);

                SendResponse(response);
            };

            _rabbitChannel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(500, stoppingToken);
            }
        }

        private async Task<string> ProcessMessageAsync(string message)
        {
            var responseBuilder = new StringBuilder();

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var command = ParseCommand(message);

                switch (command.TableName)
                {
                    case "Users":
                        responseBuilder.Append(await HandleUsersCommandAsync(dbContext, command));
                        break;

                    default:
                        responseBuilder.Append("Неизвестная таблица");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                responseBuilder.Append($"Ошибка: {ex.Message}");
            }

            return responseBuilder.ToString();
        }

        private async Task<string> HandleUsersCommandAsync(AppDbContext dbContext, Command command)
        {
            return command.Action switch
            {
                "GetAll" => JsonConvert.SerializeObject(await dbContext.Users.ToListAsync()),
                "Get" => await HandleGetUserByIdAsync(dbContext, command),
                "Add" => await HandleAddUserAsync(dbContext, command),
                "Update" => await HandleUpdateUserAsync(dbContext, command),
                "Delete" => await HandleDeleteUserAsync(dbContext, command),
                _ => "Неизвестная команда"
            };
        }

        private async Task<string> HandleGetUserByIdAsync(AppDbContext dbContext, Command command)
        {
            var user = await dbContext.Users.FindAsync(command.Id);
            return user != null ? JsonConvert.SerializeObject(user) : "Пользователь не найден";
        }

        private async Task<string> HandleAddUserAsync(AppDbContext dbContext, Command command)
        {
            var user = JsonConvert.DeserializeObject<User>(command.Data);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return JsonConvert.SerializeObject(user);
        }

        private async Task<string> HandleUpdateUserAsync(AppDbContext dbContext, Command command)
        {
            var user = JsonConvert.DeserializeObject<User>(command.Data);
            var existingUser = await dbContext.Users.FindAsync(user.Id);

            if (existingUser == null)
                return "Пользователь не найден";

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.PasswordHash = user.PasswordHash;
            await dbContext.SaveChangesAsync();

            return JsonConvert.SerializeObject(existingUser);
        }

        private async Task<string> HandleDeleteUserAsync(AppDbContext dbContext, Command command)
        {
            var user = await dbContext.Users.FindAsync(command.Id);

            if (user == null)
                return "Пользователь не найден";

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
            return JsonConvert.SerializeObject(user);
        }

        private Command ParseCommand(string message)
        {
            var parts = message.Split(' ', 3);
            return new Command
            {
                TableName = parts[0],
                Action = parts[1],
                Id = parts.Length > 2 && int.TryParse(parts[2], out var id) ? id : null,
                Data = parts.Length > 2 ? parts[2] : null
            };
        }

        private void SendResponse(string response)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response);
            _rabbitChannel.BasicPublish(exchange: "", routingKey: ResponseQueueName, basicProperties: null, body: responseBytes);
            Console.WriteLine($"[x] Отправлено: {response}");
        }

        public override void Dispose()
        {
            _rabbitChannel?.Close();
            _rabbitConnection?.Close();
            base.Dispose();
        }

        private class Command
        {
            public string TableName { get; set; }
            public string Action { get; set; }
            public int? Id { get; set; }
            public string Data { get; set; }
        }
    }
}
