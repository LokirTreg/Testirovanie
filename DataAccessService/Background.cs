using DataAccessService.Data;
using DataAccessService.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json.Serialization;

namespace DataAccessService
{
    public class Background : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _queueName = "busQueue";
        private readonly string _responseQueueName = "busQueueResponse";
        private IConnection _rabbitConnection;
        private IModel _rabbitChannel;
        public Background(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _rabbitConnection = factory.CreateConnection();
            _rabbitChannel = _rabbitConnection.CreateModel();
            _rabbitChannel.QueueDeclare(queue: _queueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

            _rabbitChannel.QueueDeclare(queue: _responseQueueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_rabbitChannel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Получено: {message}");

                // Обработка сообщения
                var result = await ProcessMessageAsync(message);

                var responseBytes = Encoding.UTF8.GetBytes(result);
                _rabbitChannel.BasicPublish(exchange: "",
                                            routingKey: _responseQueueName, 
                                            basicProperties: null,
                                            body: responseBytes);
                Console.WriteLine($" [x] Отправлено: {result}");
            };

            _rabbitChannel.BasicConsume(queue: _queueName,
                                        autoAck: true,
                                        consumer: consumer);

            // Постоянная проверка статуса работы
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(500, stoppingToken); // Задержка для предотвращения перегрузки CPU
            }
        }

        private async Task<string> ProcessMessageAsync(string message)
        {
            string result = "";
            /*try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    char[] splitters = { ' ', '\n', '\r', '\t' };
                    string[] splittedMessage = message.Split(splitters);

                    switch (splittedMessage[0]) // splittedMessage[0] - к какой табличке обращаемся
                    {
                        case "Buses":
                            if (splittedMessage[1] == "GetAll")
                            {
                                var allObjects = dbContext.Buses.ToList();
                                foreach (var obj in allObjects)
                                {
                                    result += JsonConvert.SerializeObject(obj) + "\n";
                                }
                            }
                            else if (splittedMessage[1] == "Get")
                            {
                                var obj = dbContext.Buses.Find(int.Parse(splittedMessage[2]));
                                if (obj != null)
                                    result += JsonConvert.SerializeObject(obj);
                                else
                                    Console.WriteLine($"{splittedMessage[0]} с Id={int.Parse(splittedMessage[2])} не найден.");
                            }
                            else if (splittedMessage[1] == "Add")
                            {
                                message = message.Replace("Buses Add", "");
                                var obj = JsonConvert.DeserializeObject<Bus>(message);
                                //obj.Id = 0;
                                dbContext.Buses.Add(obj);
                                await dbContext.SaveChangesAsync();
                                result += JsonConvert.SerializeObject(obj);
                            }
                            else if (splittedMessage[1] == "Update")
                            {
                                message = message.Replace($"Buses Update {splittedMessage[2]}", "");
                                var obj = JsonConvert.DeserializeObject<Bus>(message);

                                var existingObj = await dbContext.Buses.FindAsync(int.Parse(splittedMessage[2]));

                                if (existingObj != null)
                                {
                                    existingObj.Manufacturer = obj.Manufacturer;
                                    existingObj.RegistrationSign = obj.RegistrationSign;
                                    existingObj.Capacity = obj.Capacity;

                                    await dbContext.SaveChangesAsync();
                                    result += JsonConvert.SerializeObject(existingObj);
                                }
                                else
                                {
                                    Console.WriteLine($"{splittedMessage[0]} с Id={int.Parse(splittedMessage[2])} не найден.");
                                }
                            }
                            else if (splittedMessage[1] == "Delete")
                            {
                                var obj = await dbContext.Buses.FindAsync(int.Parse(splittedMessage[2]));
                                if (obj == null)
                                {
                                    Console.WriteLine($"{splittedMessage[0]} с Id={int.Parse(splittedMessage[2])} не найден.");
                                }
                                else
                                {
                                    dbContext.Buses.Remove(obj);
                                    await dbContext.SaveChangesAsync();
                                    result += JsonConvert.SerializeObject(obj);
                                }
                            }
                            break;
                            

                        case "Drivers":
                            if (splittedMessage[1] == "GetAll")
                            {
                                var allObjects = dbContext.Drivers.ToList();
                                foreach (var obj in allObjects)
                                {
                                    result += JsonConvert.SerializeObject(obj) + "\n";
                                }
                            }
                            else if (splittedMessage[1] == "Get")
                            {
                                var obj = dbContext.Drivers.Find(int.Parse(splittedMessage[2]));
                                if (obj != null)
                                    result += JsonConvert.SerializeObject(obj);
                                else
                                    Console.WriteLine($"{splittedMessage[0]} с Id={int.Parse(splittedMessage[2])} не найден.");
                            }
                            else if (splittedMessage[1] == "Add")
                            {
                                message = message.Replace("Drivers Add", "");
                                var obj = JsonConvert.DeserializeObject<Driver>(message);
                                //obj.Id = 0;
                                dbContext.Drivers.Add(obj);
                                await dbContext.SaveChangesAsync();
                                result += JsonConvert.SerializeObject(obj);
                            }
                            else if (splittedMessage[1] == "Update")
                            {
                                message = message.Replace($"Drivers Update {splittedMessage[2]}", "");
                                var obj = JsonConvert.DeserializeObject<Driver>(message);

                                var existingObj = await dbContext.Drivers.FindAsync(int.Parse(splittedMessage[2]));

                                if (existingObj != null)
                                {
                                    existingObj.Name = obj.Name;
                                    existingObj.Age = obj.Age;
                                    existingObj.Experience = obj.Experience;

                                    await dbContext.SaveChangesAsync();
                                    result += JsonConvert.SerializeObject(existingObj);
                                }
                                else
                                {
                                    Console.WriteLine($"{splittedMessage[0]} с Id={int.Parse(splittedMessage[2])} не найден.");
                                }
                            }
                            else if (splittedMessage[1] == "Delete")
                            {
                                var obj = dbContext.Drivers.Find(int.Parse(splittedMessage[2]));
                                if (obj == null)
                                {
                                    Console.WriteLine($"{splittedMessage[0]} с Id={int.Parse(splittedMessage[2])} не найден.");
                                }
                                else
                                {
                                    dbContext.Drivers.Remove(obj);
                                    await dbContext.SaveChangesAsync();
                                    result += JsonConvert.SerializeObject(obj);
                                }
                            }
                            break;

                        case "Routes":
                            if (splittedMessage[1] == "GetAll")
                            {
                                var allObjects = dbContext.Routes.ToList();
                                foreach (var obj in allObjects)
                                {
                                    result += JsonConvert.SerializeObject(obj) + "\n";
                                }
                            }
                            else if (splittedMessage[1] == "Get")
                            {
                                var obj = dbContext.Routes.Find(int.Parse(splittedMessage[2]));
                                if (obj != null)
                                    result += JsonConvert.SerializeObject(obj);
                                else
                                    Console.WriteLine($"{splittedMessage[0]} с Id={int.Parse(splittedMessage[2])} не найден.");
                            }
                            else if (splittedMessage[1] == "Add")
                            {
                                message = message.Replace("Routes Add", "");
                                var obj = JsonConvert.DeserializeObject<Models.Route>(message);
                                dbContext.Routes.Add(obj);
                                await dbContext.SaveChangesAsync();
                                result += JsonConvert.SerializeObject(obj);
                            }
                            else if (splittedMessage[1] == "Update")
                            {
                                message = message.Replace($"Routes Update {splittedMessage[2]}", "");
                                var obj = JsonConvert.DeserializeObject<Models.Route>(message);

                                var existingObj = dbContext.Routes.Find(int.Parse(splittedMessage[2]));

                                if (existingObj != null)
                                {
                                    existingObj.RouteFirstStop = obj.RouteFirstStop;
                                    existingObj.RouteLastStop = obj.RouteLastStop;
                                    existingObj.RouteDistanceKm = obj.RouteDistanceKm;

                                    await dbContext.SaveChangesAsync();
                                    result += JsonConvert.SerializeObject(existingObj);
                                }
                                else
                                {
                                    Console.WriteLine($"{splittedMessage[0]} с Id={int.Parse(splittedMessage[2])} не найден.");
                                }
                            }
                            else if (splittedMessage[1] == "Delete")
                            {
                                var obj = await dbContext.Routes.FindAsync(int.Parse(splittedMessage[2]));
                                if (obj == null)
                                {
                                    Console.WriteLine($"{splittedMessage[0]} с Id={int.Parse(splittedMessage[2])} не найден.");
                                }
                                else
                                {
                                    dbContext.Routes.Remove(obj);
                                    await dbContext.SaveChangesAsync();
                                    result += JsonConvert.SerializeObject(obj);
                                }
                            }
                            break;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки сообщения: {ex.Message}");
                return "";
            }*/
            return result;
        }

        public override void Dispose()
        {
            _rabbitChannel?.Close();
            _rabbitConnection?.Close();
            base.Dispose();
        }
    }

}
