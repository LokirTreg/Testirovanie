using PL_BL_Service.Models;
using Newtonsoft.Json;

namespace PL_BL_Service.BL
{
    public class BusinessService : IBusinessService
    {
        private readonly RabbitMqClientService _rabbitMqClient;
        public BusinessService(RabbitMqClientService rabbitMqClient)
        {
            _rabbitMqClient = rabbitMqClient;
        }
        #region Автобусы
        public async Task<List<Bus>> GetAllBuses()
        {
            try
            {
                List<Bus> buses = new List<Bus>();

                _rabbitMqClient.SendMessage("Buses GetAll");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    return buses;
                }

                foreach (string json in response.Split('\n'))
                {
                    if (json != "")
                        buses.Add(JsonConvert.DeserializeObject<Bus>(json));
                }

                return buses;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении автобусов: {ex.Message}");
                return new List<Bus>();
            }
        }
        public async Task<Bus> GetBus(int id)
        {
            try
            {
                Bus bus;
                _rabbitMqClient.SendMessage($"Buses Get {id}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }

                response = response.Replace("\n", "");
                bus = JsonConvert.DeserializeObject<Bus>(response);

                return bus;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении автобуса: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> AddBus(Bus bus)
        {
            try
            {
                _rabbitMqClient.SendMessage($"Buses Add {JsonConvert.SerializeObject(bus)}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Не удалось добавить автобус");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении автобуса: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateBus(int id, Bus bus)
        {
            try
            {
                _rabbitMqClient.SendMessage($"Buses Update {id} {JsonConvert.SerializeObject(bus)}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Не удалось обновить автобус");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении автобуса: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteBus(int id)
        {
            _rabbitMqClient.SendMessage($"Buses Delete {id}");
            //Task.Delay(200).Wait();
            string response = await _rabbitMqClient.ReceiveMessageAsync();

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Не удалось удалить автобус");
                return false;
            }
            return true;
        }

        #endregion

        #region Водители
        public async Task<List<Driver>> GetAllDrivers()
        {
            try
            {
                List<Driver> drivers = new List<Driver>();

                _rabbitMqClient.SendMessage("Drivers GetAll");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    return drivers;
                }

                foreach (string json in response.Split('\n'))
                {
                    if (json != "")
                        drivers.Add(JsonConvert.DeserializeObject<Driver>(json));
                }

                return drivers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении водителей: {ex.Message}");
                return new List<Driver>();
            }
        }
        public async Task<Driver> GetDriver(int id)
        {
            try
            {
                Driver driver;
                _rabbitMqClient.SendMessage($"Drivers Get {id}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }

                response = response.Replace("\n", "");
                driver = JsonConvert.DeserializeObject<Driver>(response);

                return driver;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении водителя: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> AddDriver(Driver driver)
        {
            try
            {
                _rabbitMqClient.SendMessage($"Drivers Add {JsonConvert.SerializeObject(driver)}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Не удалось добавить водителя");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении водителя: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateDriver(int id, Driver driver)
        {
            try
            {
                _rabbitMqClient.SendMessage($"Drivers Update {id} {JsonConvert.SerializeObject(driver)}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Не удалось обновить водителя");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении водителя: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteDriver(int id)
        {
            _rabbitMqClient.SendMessage($"Drivers Delete {id}");
            //Task.Delay(200).Wait();
            string response = await _rabbitMqClient.ReceiveMessageAsync();

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Не удалось удалить водителя");
                return false;
            }
            return true;
        }
        #endregion

        #region Маршруты
        public async Task<List<Models.Route>> GetAllRoutes()
        {
            try
            {
                List<Models.Route> routes = new List<Models.Route>();

                _rabbitMqClient.SendMessage("Routes GetAll");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    return routes;
                }

                foreach (string json in response.Split('\n'))
                {
                    if (json != "")
                        routes.Add(JsonConvert.DeserializeObject<Models.Route>(json));
                }

                return routes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении маршрутов: {ex.Message}");
                return new List<Models.Route>();
            }
        }
        public async Task<Models.Route> GetRoute(int id)
        {
            try
            {
                Models.Route route;
                _rabbitMqClient.SendMessage($"Routes Get {id}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }

                response = response.Replace("\n", "");
                route = JsonConvert.DeserializeObject<Models.Route>(response);

                return route;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении маршрута: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> AddRoute(Models.Route route)
        {
            try
            {
                _rabbitMqClient.SendMessage($"Routes Add {JsonConvert.SerializeObject(route)}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Не удалось добавить маршрут");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении маршрута: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateRoute(int id, Models.Route route)
        {
            try
            {
                _rabbitMqClient.SendMessage($"Routes Update {id} {JsonConvert.SerializeObject(route)}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Не удалось обновить маршрут");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении маршрута: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteRoute(int id)
        {
            _rabbitMqClient.SendMessage($"Routes Delete {id}");
            //Task.Delay(200).Wait();
            string response = await _rabbitMqClient.ReceiveMessageAsync();

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Не удалось удалить маршрут");
                return false;
            }
            return true;
        }
        #endregion

    }
}
