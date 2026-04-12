using Microsoft.EntityFrameworkCore;
using TodoList.Models;

namespace TodoList.Data;

public class AppDbContext : DbContext
{
	public DbSet<TodoItem> Todos => Set<TodoItem>();
	public DbSet<Profile> Profiles => Set<Profile>();

	protected override void OnConfiguring(DbContextOptionsBuilder o)
		=> o.UseSqlite("Data Source=todos.db");
}