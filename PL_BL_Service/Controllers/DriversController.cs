using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PL_BL_Service.BL;
using PL_BL_Service.Models;

namespace PL_BL_Service.Controllers
{
    [Route("[controller]")]
    public class DriversController : Controller
    {
        private readonly IBusinessService _businessService;

        // Внедрение зависимости для использования BusinessService
        public DriversController(IBusinessService businessService)
        {
            _businessService = businessService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Метод для получения всех водителей
        [HttpGet("GetAllDrivers")]
        public IActionResult GetAllDrivers()
        {
            try
            {
                var task = _businessService.GetAllDrivers();
                task.Wait();
                var drivers = task.Result;
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении водителей: {ex.Message}");
            }
        }

        // Метод для получения одного водителя по id
        [HttpGet("GetDriver/{id}")]
        public IActionResult GetDriver(int id)
        {
            try
            {
                var task = _businessService.GetDriver(id); // Вызов метода для получения одного водителя по id
                task.Wait();
                var driver = task.Result;
                if (driver == null)
                {
                    return NotFound(); // Если автобус не найден, возвращаем 404
                }
                Console.WriteLine($"Получен водитель {driver.Name}");
                return Ok(driver); // Возвращаем данные автобуса в формате JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении водителя: {ex.Message}");
            }
        }

        // Метод для добавления нового водителя
        [HttpPost("AddDriver")]
        public IActionResult AddDriver([FromBody] Driver driver)
        {
            if (driver == null)
            {
                return BadRequest("Данные не были переданы");
            }

            var task = _businessService.AddDriver(driver);
            task.Wait();
            var taskCopy = task;
            bool isAdded = task.Result;

            if (!isAdded)
            {
                return BadRequest("Ошибка при добавлении водителя");
            }

            return CreatedAtAction(nameof(GetDriver), new { id = driver.Id }, driver); // Возвращаем статус 201 (Created) с данными добавленного автобуса
        }

        // Метод для изменения водителя по Id
        [HttpPost("UpdateDriver/{id}")]
        public IActionResult UpdateDriver(int id, [FromBody] Driver driver)
        {
            Driver updated = driver;
            if (driver == null)
            {
                return BadRequest("Данные не были переданы");
            }

            var task = _businessService.UpdateDriver(id, driver);
            task.Wait();
            bool isUpdated = task.Result;

            if (!isUpdated)
            {
                return BadRequest("Ошибка при обновлении водителя");
            }

            return Ok(updated);
        }
        [HttpPost("DeleteDriver/{id}")]
        public IActionResult DeleteDriver(int id)
        {
            var task = _businessService.DeleteDriver(id); // Вызов метода для удаления водителя по id
            task.Wait();
            bool isDeleted = task.Result;

            if (!isDeleted)
            {
                return BadRequest("Ошибка при удалении водителя");
            }

            return Ok();
        }
    }
}