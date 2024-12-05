using Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace PL_BL_Service.BL
{
    public class UserBL : IUserBL
    {
        private readonly RabbitMqClientService _rabbitMqClient;
        public UserBL(RabbitMqClientService rabbitMqClient)
        {
            _rabbitMqClient = rabbitMqClient;
        }
        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                List<User> useres = new List<User>();

                _rabbitMqClient.SendMessage("Users GetAll");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    return useres;
                }
                
                useres = (JsonConvert.DeserializeObject<List<User>>(response));

                return useres;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении: {ex.Message}");
                return new List<User>();
            }
        }
        public async Task<User> GetUser(int id)
        {
            try
            {
                User user;
                _rabbitMqClient.SendMessage($"Users Get {id}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }

                response = response.Replace("\n", "");
                user = JsonConvert.DeserializeObject<User>(response);

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении : {ex.Message}");
                return null;
            }
        }
        private string GetStringHash(string s)
        {
            if (s == null)
                return null;
            using var hashAlgorithm = SHA512.Create();
            var hash = hashAlgorithm.ComputeHash(Encoding.Unicode.GetBytes(s));
            return string.Concat(hash.Select(item => item.ToString("x2")));
        }

        public async Task<bool> AddUser(User user)
        {
            try
            {
                user.PasswordHash = GetStringHash(user.PasswordHash);
                _rabbitMqClient.SendMessage($"Users Add {JsonConvert.SerializeObject(user)}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Не удалось добавить");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                user.PasswordHash = GetStringHash(user.PasswordHash);
                _rabbitMqClient.SendMessage($"Users Update {JsonConvert.SerializeObject(user)}");
                //Task.Delay(200).Wait();
                string response = await _rabbitMqClient.ReceiveMessageAsync();

                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Не удалось обновить");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteUser(int id)
        {
            _rabbitMqClient.SendMessage($"Users Delete {id}");
            //Task.Delay(200).Wait();
            string response = await _rabbitMqClient.ReceiveMessageAsync();

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Не удалось удалить");
                return false;
            }
            return true;
        }

    }
}
