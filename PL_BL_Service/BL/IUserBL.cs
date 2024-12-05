using Models;
namespace PL_BL_Service.BL
{
    public interface IUserBL
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUser(int id);
        Task<bool> AddUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
    }
}
