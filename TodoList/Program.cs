using System;
using System.IO;
using System.Text;

namespace TodoListApp
{
	internal class Program
	{
		private const int InitialTasksCapacity = 2;
		private static readonly TodoList todoList = new TodoList(InitialTasksCapacity);
		private static Profile? userProfile;

		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
			Console.InputEncoding = Encoding.UTF8;

			Console.WriteLine("Работу выполнил Соловьёв Евгений и Тареев Юрий");

			string dataDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
			FileManager.EnsureDataDirectory(dataDir);

			userProfile = CreateUserProfile();
			LoadTodos();

			if (userProfile != null)
			{
				RunTodoApplication();
			}
			else
			{
				Console.WriteLine("Не удалось создать профиль пользователя.");
			}
		}

		static Profile? CreateUserProfile()
		{
			string profileFilePath = GetProfileFilePath();
			Profile? profile = FileManager.LoadProfile(profileFilePath);

			if (profile != null)
			{
				return profile;
			}

			try
			{
				Console.WriteLine("Профиль не найден, давайте создадим новый.");
				Console.WriteLine("Введите Имя");
				string? name = Console.ReadLine();

				if (string.IsNullOrWhiteSpace(name))
				{
					Console.WriteLine("Ошибка: Имя не может быть пустым");
					return null;
				}

				Console.WriteLine("Введите Фамилию");
				string? surname = Console.ReadLine();

				if (string.IsNullOrWhiteSpace(surname))
				{
					Console.WriteLine("Ошибка: Фамилия не может быть пустой");
					return null;
				}

				int birthYear;
				while (true)
				{
					Console.WriteLine("Введите Год Рождения");
					string? input = Console.ReadLine();

					if (string.IsNullOrWhiteSpace(input))
					{
						Console.WriteLine("Ошибка: Год рождения не может быть пустым");
						continue;
					}

					if (!int.TryParse(input, out birthYear))
					{
						Console.WriteLine("Ошибка: Неверный формат года. Введите четыре цифры, например: 2000");
						continue;
					}
					break;
				}

				var newProfile = new Profile(name, surname, birthYear);
				Console.WriteLine($"Добавлен пользователь {newProfile.GetInfo()}");
				FileManager.SaveProfile(newProfile, profileFilePath);
				return newProfile;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Произошла непредвиденная ошибка при создании профиля: {ex.Message}");
				return null;
			}
		}

		static void LoadTodos()
		{
			string todoFilePath = GetTodoFilePath();
			TodoList loadedList = FileManager.LoadTodos(todoFilePath);

			if (loadedList.Count > 0)
			{
				for (int i = 0; i < loadedList.Count; i++)
				{
					todoList.Add(loadedList.GetTask(i));
				}
				Console.WriteLine($"Загружено {todoList.Count} задач из файла.");
			}
			else
			{
				Console.WriteLine("Файл задач не найден или пуст. Создан новый список задач.");
			}
		}

		public static string GetTodoFilePath()
		{
			string dataDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
			return Path.Combine(dataDir, "todo.csv");
		}

		public static string GetProfileFilePath()
		{
			string dataDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
			return Path.Combine(dataDir, "profile.txt");
		}

		static void RunTodoApplication()
		{
			Console.WriteLine("Введите команду help для просмотра всех команд.");
			while (true)
			{
				try
				{
					Console.Write("> ");
					string? input = Console.ReadLine();

					if (string.IsNullOrWhiteSpace(input))
						continue;

					ICommand command = CommandParser.Parse(input, todoList, userProfile);
					command.Execute();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Произошла ошибка: {ex.Message}");
				}
			}
		}
	}
}