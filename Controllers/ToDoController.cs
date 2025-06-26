using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoWebApi.Models;
using ToDoWebApi.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ToDoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoRepository _repo;

        public ToDoController(ToDoRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Tüm görevleri getirir.
        /// </summary>
        /// <returns>Görev listesini döner.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(typeof(List<ToDo>), StatusCodes.Status200OK)]
        public IActionResult GetAll() => Ok(_repo.GetAll());

        /// <summary>
        /// Belirli bir görevi ID'ye göre getirir.
        /// </summary>
        /// <param name="id">Görev kimliği</param>
        /// <returns>Görev objesi veya 404 Not Found</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(typeof(ToDo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            var todo = _repo.Get(id);
            return todo == null ? NotFound() : Ok(todo);
        }

        /// <summary>
        /// Yeni bir görev ekler.
        /// </summary>
        /// <param name="todo">Eklenecek görev</param>
        /// <returns>Eklenen görev objesi</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ToDo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Add(ToDo todo)
        {
            return Ok(_repo.Add(todo));
        }

        /// <summary>
        /// Var olan bir görevi günceller.
        /// </summary>
        /// <param name="id">Güncellenecek görev kimliği</param>
        /// <param name="todo">Güncel görev verisi</param>
        /// <returns>Başarı durumu</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(int id, ToDo todo)
        {
            if (id != todo.Id) return BadRequest();
            return _repo.Update(todo) ? Ok() : NotFound();
        }

        /// <summary>
        /// Görevi ID'ye göre siler.
        /// </summary>
        /// <param name="id">Silinecek görev kimliği</param>
        /// <returns>Başarı durumu</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            return _repo.Delete(id) ? Ok() : NotFound();
        }

        /// <summary>
        /// Test amacıyla hata fırlatır (middleware test için).
        /// </summary>
        /// <returns>Hata fırlatır</returns>
        [HttpGet("throw")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult ThrowException()
        {
            throw new Exception("Test exception!");
        }
    }
}
