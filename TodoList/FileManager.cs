using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class FileManager
{
	public static void SaveTodoListOnChange(TodoItem item)
	{
		if (AppInfo.CurrentProfileId.HasValue && AppInfo.Todos != null)
		{
			SaveUserTodos(AppInfo.CurrentProfileId.Value, AppInfo.Todos, Program.DataDir);
		}
	}

	public static void EnsureDataDirectory(string dirPath)
	{
		if (!Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
			Console.WriteLine($"Создана папка: {dirPath}");
		}
	}

	public static void SaveProfiles(List<Profile> profiles, string filePath)
	{
		try
		{
			var lines = profiles.Select(p =>
				$"{p.Id};{p.Login};{p.Password};{p.FirstName};{p.LastName};{p.BirthYear}");

			File.WriteAllLines(filePath, lines, Encoding.UTF8);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка сохранения профилей: {ex.Message}");
		}
	}

	public static List<Profile> LoadProfiles(string filePath)
	{
		var profiles = new List<Profile>();
		if (!File.Exists(filePath))
		{
			return profiles;
		}

		try
		{
			string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
			foreach (var line in lines)
			{
				if (string.IsNullOrWhiteSpace(line)) continue;

				string[] parts = line.Split(';');
				if (parts.Length == 6)
				{
					if (Guid.TryParse(parts[0], out Guid id) &&
						int.TryParse(parts[5], out int birthYear))
					{
						var profile = new Profile(
							firstName: parts[3],
							lastName: parts[4],
							birthYear: birthYear,
							login: parts[1],
							password: parts[2],
							id: id
						);
						profiles.Add(profile);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка загрузки профилей: {ex.Message}");
		}
		return profiles;
	}

	public static void SaveUserTodos(Guid userId, TodoList todos, string dataDir)
	{
		string filePath = Path.Combine(dataDir, $"todos_{userId}.csv");
		try
		{
			var lines = new List<string>();
			for (int i = 0; i < todos.Count; i++)
			{
				var item = todos[i];
				string escapedText = item.Text.Replace("\"", "\"\"").Replace("\n", "\\n").Replace("\r", "\\r");
				lines.Add($"{i};\"{escapedText}\";{item.Status.ToString()};{item.LastUpdate:yyyy-MM-dd HH:mm:ss}");
			}
			File.WriteAllLines(filePath, lines, Encoding.UTF8);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка сохранения задач для пользователя {userId}: {ex.Message}");
		}
	}

	public static TodoList LoadUserTodos(Guid userId, string dataDir)
	{
		string filePath = Path.Combine(dataDir, $"todos_{userId}.csv");
		var todoList = new TodoList();
		if (!File.Exists(filePath))
		{
			return todoList;
		}

		try
		{
			string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
			foreach (var line in lines)
			{
				if (string.IsNullOrWhiteSpace(line)) continue;

				string[] parts = ParseCsvLine(line, ';');
				if (parts.Length == 4)
				{
					string text = parts[1].Replace("\"\"", "\"").Replace("\\n", "\n").Replace("\r", "\r");

					if (Enum.TryParse<TodoStatus>(parts[2], true, out var status) &&
						DateTime.TryParse(parts[3], out DateTime lastUpdate))
					{
						var todoItem = new TodoItem(text, status, lastUpdate);
						todoList.Add(todoItem);
					}
					else
					{

					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка загрузки задач для пользователя {userId}: {ex.Message}");
		}
		return todoList;
	}

	private static string[] ParseCsvLine(string line, char separator = ';')
	{
		var parts = new List<string>();
		var currentPart = new StringBuilder();
		bool inQuotes = false;
		for (int i = 0; i < line.Length; i++)
		{
			char c = line[i];
			if (c == '"')
			{
				if (i + 1 < line.Length && line[i + 1] == '"')
				{
					currentPart.Append('"');
					i++;
				}
				else
				{
					inQuotes = !inQuotes;
				}
			}
			else if (c == separator && !inQuotes)
			{
				parts.Add(currentPart.ToString());
				currentPart.Clear();
			}
			else
			{
				currentPart.Append(c);
			}
		}
		parts.Add(currentPart.ToString());
		return parts.ToArray();
	}
}