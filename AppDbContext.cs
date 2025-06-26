using Microsoft.EntityFrameworkCore;
using ToDoWebApi.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<ToDo> ToDos { get; set; }
} 