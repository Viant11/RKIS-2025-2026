using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

public static class FileManager
{
	public static void EnsureDataDirectory(string dirPath)
	{
		if (!Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
			Console.WriteLine($"Создана папка: {dirPath}");
		}
	}

	public static void SaveProfile(Profile profile, string filePath)
	{
		try
		{
			string profileData = $"{profile.FirstName}|{profile.LastName}|{profile.BirthYear}";
			File.WriteAllText(filePath, profileData, Encoding.UTF8);
			Console.WriteLine("Профиль сохранен");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка сохранения профиля: {ex.Message}");
		}
	}

	public static Profile LoadProfile(string filePath)
	{
		try
		{
			if (!File.Exists(filePath))
			{
				Console.WriteLine("Файл профиля не найден");
				return null;
			}
			string line = File.ReadAllText(filePath, Encoding.UTF8);
			if (!string.IsNullOrEmpty(line))
			{
				string[] parts = line.Split('|');
				if (parts.Length == 3)
				{
					string firstName = parts[0];
					string lastName = parts[1];
					if (int.TryParse(parts[2], out int birthYear))
					{
						return new Profile(firstName, lastName, birthYear);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка загрузки профиля: {ex.Message}");
		}
		return null;
	}

	public static void SaveTodos(TodoList todos, string filePath)
	{
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
			Console.WriteLine($"Ошибка сохранения задач: {ex.Message}");
		}
	}

	public static TodoList LoadTodos(string filePath)
	{
		var todoList = new TodoList();
		try
		{
			if (!File.Exists(filePath))
			{
				Console.WriteLine("Файл задач не найден");
				return todoList;
			}

			string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				if (!string.IsNullOrEmpty(line))
				{
					string[] parts = ParseCsvLine(line, ';');
					if (parts.Length == 4)
					{
						string text = parts[1].Replace("\"\"", "\"").Replace("\\n", "\n").Replace("\r", "\r");

						if (!Enum.TryParse<TodoStatus>(parts[2], true, out var status))
						{
							if (bool.TryParse(parts[2], out bool isDone))
							{
								status = isDone ? TodoStatus.Completed : TodoStatus.NotStarted;
							}
							else
							{
								status = TodoStatus.NotStarted;
							}
						}

						DateTime lastUpdate = DateTime.Parse(parts[3]);
						var todoItem = new TodoItem(text, status, lastUpdate);
						todoList.Add(todoItem);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка загрузки задач: {ex.Message}");
		}
		return todoList;
	}

	private static string[] ParseCsvLine(string line, char separator = ';')
	{
		var parts = new List<string>();
		int start = 0;
		bool inQuotes = false;
		for (int i = 0; i < line.Length; i++)
		{
			if (line[i] == '"')
			{
				inQuotes = !inQuotes;
			}
			else if (line[i] == separator && !inQuotes)
			{
				string part = line.Substring(start, i - start);
				if (part.StartsWith("\"") && part.EndsWith("\""))
				{
					part = part.Substring(1, part.Length - 2);
				}
				parts.Add(part);
				start = i + 1;
			}
		}
		if (start < line.Length)
		{
			string part = line.Substring(start);
			if (part.StartsWith("\"") && part.EndsWith("\""))
			{
				part = part.Substring(1, part.Length - 2);
			}
			parts.Add(part);
		}
		return parts.ToArray();
	}
}