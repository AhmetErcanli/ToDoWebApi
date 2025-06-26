using ToDoWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ToDoWebApi.Repositories
{
    /// <summary>
    /// ToDo görevleri için bellek içi (in-memory) veri deposu sağlar.
    /// </summary>
    public class ToDoRepository
    {
        private readonly AppDbContext _context;
        public ToDoRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tüm görevleri getirir.
        /// </summary>
        /// <returns>ToDo görevlerinin listesi</returns>
        public List<ToDo> GetAll() => _context.ToDos.ToList();

        /// <summary>
        /// Belirtilen ID'ye sahip görevi getirir.
        /// </summary>
        /// <param name="id">Görev kimliği</param>
        /// <returns>ToDo görevi veya null</returns>
        public ToDo? Get(int id) => _context.ToDos.FirstOrDefault(t => t.Id == id);

        /// <summary>
        /// Yeni görev ekler.
        /// </summary>
        /// <param name="todo">Eklenecek görev</param>
        /// <returns>Eklenen görev (ID atanmış olarak)</returns>
        public ToDo Add(ToDo todo)
        {
            _context.ToDos.Add(todo);
            _context.SaveChanges();
            return todo;
        }

        /// <summary>
        /// Var olan görevi günceller.
        /// </summary>
        /// <param name="todo">Güncellenecek görev</param>
        /// <returns>Güncelleme başarılıysa true, değilse false</returns>
        public bool Update(ToDo todo)
        {
            var existing = Get(todo.Id);
            if (existing == null) return false;
            existing.Title = todo.Title;
            existing.IsCompleted = todo.IsCompleted;
            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Görevi ID ile siler.
        /// </summary>
        /// <param name="id">Silinecek görev kimliği</param>
        /// <returns>Silme başarılıysa true, değilse false</returns>
        public bool Delete(int id)
        {
            var todo = Get(id);
            if (todo == null) return false;
            _context.ToDos.Remove(todo);
            _context.SaveChanges();
            return true;
        }
    }
}
