using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PL_BL_Service.BL;
using PL_BL_Service.Models;

namespace PL_BL_Service.Controllers
{
    [Route("[controller]")]
    public class BusesController : Controller
    {
        private readonly IBusinessService _businessService; 

        // Внедрение зависимости для использования BusinessService
        public BusesController(IBusinessService businessService)
        {
            _businessService = businessService;
        }
        [HttpGet]
        public IActionResult Index()
        {  
            return View();
        }

        // Метод для получения всех автобусов
        [HttpGet("GetAllBuses")]
        public IActionResult GetAllBuses()
        {
            try
            {
                var task = _businessService.GetAllBuses();
                task.Wait();
                var buses = task.Result;
                return Ok(buses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении автобусов: {ex.Message}");
            }
        }

        // Метод для получения одного автобуса по id
        [HttpGet("GetBus/{id}")]
        public IActionResult GetBus(int id)
        {
            try
            {
                var task = _businessService.GetBus(id); // Вызов метода для получения одного автобуса по id
                task.Wait();
                var bus = task.Result;
                if (bus == null)
                {
                    return NotFound(); // Если автобус не найден, возвращаем 404
                }
                Console.WriteLine($"Получен автобус {bus.Manufacturer}");
                return Ok(bus); // Возвращаем данные автобуса в формате JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении автобуса: {ex.Message}");
            }
        }

        // Метод для добавления нового автобуса
        [HttpPost("AddBus")]
        public IActionResult AddBus([FromBody] Bus bus)
        {
            if (bus == null)
            {
                return BadRequest("Данные не были переданы");
            }

            var task = _businessService.AddBus(bus);
            task.Wait();
            var taskCopy = task;
            bool isAdded = task.Result;

            if (!isAdded)
            {
                return BadRequest("Ошибка при добавлении автобуса");
            }

            return CreatedAtAction(nameof(GetBus), new { id = bus.Id }, bus); // Возвращаем статус 201 (Created) с данными добавленного автобуса
        }

        // Метод для изменения автобуса по Id
        [HttpPost("UpdateBus/{id}")]
        public IActionResult UpdateBus(int id, [FromBody] Bus bus)
        {
            Bus updated = bus;
            if (bus == null)
            {
                return BadRequest("Данные не были переданы");
            }

            var task = _businessService.UpdateBus(id, bus);
            task.Wait();
            bool isUpdated = task.Result;

            if (!isUpdated)
            {
                return BadRequest("Ошибка при обновлении автобуса");
            }

            return Ok(updated);
        }
        [HttpPost("DeleteBus/{id}")]
        public IActionResult DeleteBus(int id)
        {
            var task = _businessService.DeleteBus(id); // Вызов метода для удаления автобуса по id
            task.Wait();
            bool isDeleted = task.Result;

            if (!isDeleted)
            {
                return BadRequest("Ошибка при удалении автобуса");
            }

            return Ok();
        }
    }
}