using System;
using System.Collections.Generic;
using System.Linq;
using TodoList.Models;
using TodoList.Data;

namespace TodoList.Services;

public class SqliteDataStorage : IDataStorage
{
	public void SaveProfiles(IEnumerable<Profile> profiles) { }

	public IEnumerable<Profile> LoadProfiles()
	{
		using var db = new AppDbContext();
		return db.Profiles.ToList();
	}

	public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos) { }

	public IEnumerable<TodoItem> LoadTodos(Guid userId)
	{
		return new List<TodoItem>();
	}
}