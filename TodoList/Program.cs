using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TodoList.Models;
using TodoList.Services;
using TodoList.Data;
using TodoList.Commands;

namespace TodoList;

internal class Program
{
	private static readonly ProfileRepository _profileRepo = new();
	private static readonly TodoRepository _todoRepo = new();

	private static void Main(string[] args)
	{
		if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
		{
			Console.InputEncoding = Encoding.Unicode;
			Console.OutputEncoding = Encoding.Unicode;
		}

		Console.WriteLine("Работу выполнили: Соловьёв Евгений и Тареев Юрий");

		Console.WriteLine("\nВыберите режим хранения данных:");
		Console.WriteLine("1 - Локальное файловое хранилище (FileManager)");
		Console.WriteLine("2 - Удаленное API-хранилище (ApiDataStorage)");
		Console.WriteLine("3 - База данных SQLite (Entity Framework Core)");
		Console.Write("Ваш выбор: ");

		string mode = Console.ReadLine();

		try
		{
			switch (mode)
			{
				case "1":
					Console.WriteLine("Выбран режим: Файлы");
					break;
				case "2":
					AppInfo.Storage = new ApiDataStorage();
					Console.WriteLine("Выбран режим: API");
					break;
				case "3":
				default:
					AppInfo.Storage = new SqliteDataStorage();
					Console.WriteLine("Выбран режим: SQLite");
					break;
			}

			if (AppInfo.Storage != null)
			{
				AppInfo.AllProfiles = AppInfo.Storage.LoadProfiles().ToList();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка инициализации: {ex.Message}");
			return;
		}

		AppInfo.UndoStack = new Stack<IUndo>();
		AppInfo.RedoStack = new Stack<IUndo>();

		try
		{
			while (true)
			{
				HandleUserLogin();

				if (AppInfo.CurrentProfileId.HasValue)
				{
					RunUserSession();
				}
				else
				{
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Критическая ошибка: {ex.Message}");
		}
	}

	private static void HandleUserLogin()
	{
		while (AppInfo.CurrentProfileId == null)
		{
			Console.WriteLine("\nВойти в существующий профиль? [y/n] (exit для выхода)");
			string choice = Console.ReadLine()?.ToLower();

			if (choice == "y") LoginUser();
			else if (choice == "n") CreateNewProfile();
			else if (choice == "exit") AppInfo.CurrentProfileId = -1;
			else Console.WriteLine("Неверный ввод.");
		}
		if (AppInfo.CurrentProfileId == -1) AppInfo.CurrentProfileId = null;
	}

	private static void LoginUser()
	{
		Console.Write("Введите логин: ");
		string login = Console.ReadLine();
		Console.Write("Введите пароль: ");
		string password = Console.ReadLine();

		Profile foundProfile = AppInfo.AllProfiles.FirstOrDefault(p => p.Login == login && p.Password == password);

		if (foundProfile == null)
		{
			Console.WriteLine("Ошибка: Неверный логин или пароль.");
			return;
		}

		var loadedTasks = _todoRepo.GetByProfile(foundProfile.Id);
		var userTodos = new TodoList(loadedTasks.ToList());

		userTodos.OnTodoAdded += (item) => { item.ProfileId = foundProfile.Id; _todoRepo.Add(item); };
		userTodos.OnTodoDeleted += (item) => _todoRepo.Delete(item.Id);
		userTodos.OnTodoUpdated += (item) => _todoRepo.Update(item);
		userTodos.OnStatusChanged += (item) => _todoRepo.Update(item);

		AppInfo.Todos = userTodos;
		AppInfo.CurrentProfileId = foundProfile.Id;

		AppInfo.UndoStack.Clear();
		AppInfo.RedoStack.Clear();
		Console.WriteLine($"Добро пожаловать, {foundProfile.FirstName}!");
	}

	private static void CreateNewProfile()
	{
		Console.Write("Новый логин: ");
		string login = Console.ReadLine();
		if (AppInfo.AllProfiles.Any(p => p.Login == login)) { Console.WriteLine("Логин занят!"); return; }

		Console.Write("Пароль: ");
		string password = Console.ReadLine();
		Console.Write("Имя: ");
		string firstName = Console.ReadLine();
		Console.Write("Фамилия: ");
		string lastName = Console.ReadLine();
		Console.Write("Год рождения: ");
		int.TryParse(Console.ReadLine(), out int year);

		var newProfile = new Profile
		{
			Login = login,
			Password = password,
			FirstName = firstName,
			LastName = lastName,
			BirthYear = year
		};

		_profileRepo.Add(newProfile);
		AppInfo.AllProfiles.Add(newProfile);

		Console.WriteLine("Профиль создан. Теперь войдите в него.");
	}

	private static void RunUserSession()
	{
		Console.WriteLine("Сессия запущена. Введите 'help' для списка команд.");
		while (AppInfo.CurrentProfileId.HasValue)
		{
			Console.Write("> ");
			string input = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(input)) continue;

			try
			{
				ICommand command = CommandParser.Parse(input);
				command.Execute();

				if (command is IUndo undoable)
				{
					AppInfo.UndoStack.Push(undoable);
					AppInfo.RedoStack.Clear();
				}

				if (!AppInfo.CurrentProfileId.HasValue) break;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка команды: {ex.Message}");
			}
		}
		Console.WriteLine("Вы вышли из профиля.");
	}
}