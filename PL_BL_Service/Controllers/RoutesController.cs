using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PL_BL_Service.BL;
using PL_BL_Service.Models;

namespace PL_BL_Service.Controllers
{
    [Route("[controller]")]
    public class RoutesController : Controller
    {
        private readonly IBusinessService _businessService;

        // Внедрение зависимости для использования BusinessService
        public RoutesController(IBusinessService businessService)
        {
            _businessService = businessService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Метод для получения всех маршрутов
        [HttpGet("GetAllRoutes")]
        public IActionResult GetAllRoutes()
        {
            try
            {
                var task = _businessService.GetAllRoutes();
                task.Wait();
                var routes = task.Result;
                return Ok(routes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении маршрутов: {ex.Message}");
            }
        }

        // Метод для получения одного маршрута по id
        [HttpGet("GetRoute/{id}")]
        public IActionResult GetRoute(int id)
        {
            try
            {
                var task = _businessService.GetRoute(id); // Вызов метода для получения одного маршрута по id
                task.Wait();
                var route = task.Result;
                if (route == null)
                {
                    return NotFound(); // Если маршрут не найден, возвращаем 404
                }
                Console.WriteLine($"Получен маршрут №{route.Id}");
                return Ok(route); // Возвращаем данные автобуса в формате JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении маршрута: {ex.Message}");
            }
        }

        // Метод для добавления нового маршрута
        [HttpPost("AddRoute")]
        public IActionResult AddDriver([FromBody] Models.Route route)
        {
            if (route == null)
            {
                return BadRequest("Данные не были переданы");
            }

            var task = _businessService.AddRoute(route);
            task.Wait();
            var taskCopy = task;
            bool isAdded = task.Result;

            if (!isAdded)
            {
                return BadRequest("Ошибка при добавлении маршрута");
            }

            return CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route); // Возвращаем статус 201 (Created) с данными добавленного маршрута
        }

        // Метод для изменения маршрута по Id
        [HttpPost("UpdateRoute/{id}")]
        public IActionResult UpdateRoute(int id, [FromBody] Models.Route route)
        {
            Models.Route updated = route;
            if (route == null)
            {
                return BadRequest("Данные не были переданы");
            }

            var task = _businessService.UpdateRoute(id, route);
            task.Wait();
            bool isUpdated = task.Result;

            if (!isUpdated)
            {
                return BadRequest("Ошибка при обновлении маршрута");
            }

            return Ok(updated);
        }
        [HttpPost("DeleteRoute/{id}")]
        public IActionResult DeleteRoute(int id)
        {
            var task = _businessService.DeleteRoute(id); // Вызов метода для удаления маршрута по id
            task.Wait();
            bool isDeleted = task.Result;

            if (!isDeleted)
            {
                return BadRequest("Ошибка при удалении маршрута");
            }

            return Ok();
        }
    }
}