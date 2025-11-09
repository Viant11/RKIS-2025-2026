using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

public static class FileManager
{
	public static void EnsureDataDirectory(string dirPath)
	{
		try
		{
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
				Console.WriteLine($"Папка создана: {dirPath}");
			}
			else
			{
				Console.WriteLine($"Папка уже существует: {dirPath}");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при создании папки {dirPath}: {ex.Message}");
			throw;
		}
	}

	public static void SaveProfile(Profile profile, string filePath)
	{
		try
		{
			string[] lines = {
				profile.FirstName,
				profile.LastName,
				profile.BirthYear.ToString()
			};
			File.WriteAllLines(filePath, lines, Encoding.UTF8);
			Console.WriteLine($"Сохранение профиля в: {filePath}");
			Console.WriteLine($"Профиль сохранен: {profile.FirstName} {profile.LastName}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при сохранении профиля: {ex.Message}");
		}
	}

	public static Profile? LoadProfile(string filePath)
	{
		try
		{
			if (!File.Exists(filePath))
			{
				return null;
			}

			string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

			if (lines.Length >= 3)
			{
				string firstName = lines[0];
				string lastName = lines[1];
				if (int.TryParse(lines[2], out int birthYear))
				{
					var profile = new Profile(firstName.Trim(), lastName.Trim(), birthYear);
					Console.WriteLine($"Успешно загружен профиль: {profile.GetInfo()}");
					return profile;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при загрузке профиля: {ex.Message}");
		}
		return null;
	}

	public static void SaveTodos(TodoList todos, string filePath)
	{
		try
		{
			var lines = new List<string> { "Text;IsDone;LastUpdate" };
			for (int i = 0; i < todos.Count; i++)
			{
				var task = todos.GetTask(i);
				string escapedText = task.Text.Replace("\"", "\"\"").Replace("\n", "\\n");
				string line = $"\"{escapedText}\";{task.IsDone};{task.LastUpdate:O}";
				lines.Add(line);
			}
			File.WriteAllLines(filePath, lines, Encoding.UTF8);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при сохранении задач: {ex.Message}");
		}
	}

	public static TodoList LoadTodos(string filePath)
	{
		var todoList = new TodoList();
		if (!File.Exists(filePath))
			return todoList;

		try
		{
			var lines = File.ReadAllLines(filePath, Encoding.UTF8);
			if (lines.Length < 2) return todoList;

			for (int i = 1; i < lines.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(lines[i])) continue;

				var parts = lines[i].Split(new[] { ';' }, 3);
				if (parts.Length == 3)
				{
					string text = parts[0].Trim('"').Replace("\\n", "\n").Replace("\"\"", "\"");
					if (bool.TryParse(parts[1], out bool isDone) &&
						DateTime.TryParse(parts[2], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime lastUpdate))
					{
						var todoItem = new TodoItem(text, isDone, lastUpdate);
						todoList.Add(todoItem);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
		}
		return todoList;
	}
}