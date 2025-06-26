using Microsoft.AspNetCore.Http;

namespace ToDoWebApi.Models
{
    /// <summary>
    /// Bir yapılacak görev öğesini temsil eder.
    /// </summary>
    public class ToDo
    {
        /// <summary>
        /// Görevin benzersiz kimliği.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Görevin başlığı veya açıklaması.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Görevin tamamlanıp tamamlanmadığını belirtir.
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
