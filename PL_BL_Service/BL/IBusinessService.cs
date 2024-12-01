using PL_BL_Service.Models;
namespace PL_BL_Service.BL
{
    public interface IBusinessService
    {
        Task<List<Bus>> GetAllBuses();
        Task<Bus> GetBus(int id);
        Task<bool> AddBus(Bus bus);
        Task<bool> UpdateBus(int id, Bus bus);
        Task<bool> DeleteBus(int id);

        Task<List<Driver>> GetAllDrivers();
        Task<Driver> GetDriver(int id);
        Task<bool> AddDriver(Driver driver);
        Task<bool> UpdateDriver(int id, Driver driver);
        Task<bool> DeleteDriver(int id);

        Task<List<Models.Route>> GetAllRoutes();
        Task<Models.Route> GetRoute(int id);
        Task<bool> AddRoute(Models.Route route);
        Task<bool> UpdateRoute(int id, Models.Route route);
        Task<bool> DeleteRoute(int id);
    }
}
