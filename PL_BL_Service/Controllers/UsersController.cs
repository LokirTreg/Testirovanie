using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PL_BL_Service.BL;
using Models;

namespace PL_BL_Service.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserBL _userBL; 

        public UsersController(IUserBL userinessService)
        {
            _userBL = userinessService;
        }
        [HttpGet]
        public IActionResult Index()
        {  
            return View();
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var task = _userBL.GetAllUsers();
                task.Wait();
                var useres = task.Result;
                return Ok(useres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении: {ex.Message}");
            }
        }

        [HttpGet("GetUser/{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var task = _userBL.GetUser(id);
                task.Wait();
                var user = task.Result;
                if (user == null)
                {
                    return NotFound();
                }
                Console.WriteLine($"Получен {user.Username}");
                return Ok(user); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении: {ex.Message}");
            }
        }
        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Данные не были переданы");
            }

            var task = _userBL.AddUser(user);
            task.Wait();
            var taskCopy = task;
            bool isAdded = task.Result;

            if (!isAdded)
            {
                return BadRequest("Ошибка при добавлении");
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPost("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            User updated = user;
            if (user == null)
            {
                return BadRequest("Данные не были переданы");
            }
            user.Id = id;
            var task = _userBL.UpdateUser(user);
            task.Wait();
            bool isUpdated = task.Result;

            if (!isUpdated)
            {
                return BadRequest("Ошибка при обновлении");
            }

            return Ok(updated);
        }
        [HttpPost("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var task = _userBL.DeleteUser(id);
            task.Wait();
            bool isDeleted = task.Result;

            if (!isDeleted)
            {
                return BadRequest("Ошибка при удалении");
            }

            return Ok();
        }
    }
}