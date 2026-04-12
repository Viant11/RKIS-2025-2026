using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TodoList.Models;
using TodoList.Services;

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
		Console.WriteLine("Хранилище: SQLite (EF Core)");

		try
		{
			AppInfo.AllProfiles = _profileRepo.GetAll();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"КРИТИЧЕСКАЯ ОШИБКА БД: {ex.Message}");
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
			Console.WriteLine($"Критическая ошибка приложения: {ex.Message}");
		}
	}

	private static void HandleUserLogin()
	{
		while (AppInfo.CurrentProfileId == null)
		{
			Console.WriteLine("\nВойти в существующий профиль? [y/n] (или 'exit' для выхода)");
			string choice = Console.ReadLine()?.ToLower();

			try
			{
				switch (choice)
				{
					case "y": LoginUser(); break;
					case "n": CreateNewProfile(); break;
					case "exit": AppInfo.CurrentProfileId = -1; break;
					case null: AppInfo.CurrentProfileId = -1; break;
					default: Console.WriteLine("Неверный ввод. Попробуйте еще раз."); break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка: {ex.Message}");
			}
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
			throw new Exception("Неверный логин или пароль.");
		}

		var loadedTasks = _todoRepo.GetByProfile(foundProfile.Id);
		var userTodos = new TodoList(loadedTasks.ToList());

		userTodos.OnTodoAdded += (item) => {
			item.ProfileId = foundProfile.Id;
			_todoRepo.Add(item);
		};
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
		Console.Write("Введите новый логин: ");
		string login = Console.ReadLine();

		if (string.IsNullOrWhiteSpace(login))
			throw new Exception("Логин не может быть пустым.");

		if (AppInfo.AllProfiles.Any(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
			throw new Exception($"Логин '{login}' уже занят.");

		Console.Write("Введите пароль: ");
		string password = Console.ReadLine();
		Console.Write("Введите ваше имя: ");
		string firstName = Console.ReadLine();
		Console.Write("Введите вашу фамилию: ");
		string lastName = Console.ReadLine();

		int birthYear;
		while (true)
		{
			Console.Write("Введите год рождения: ");
			if (int.TryParse(Console.ReadLine(), out birthYear) && birthYear > 1900 && birthYear <= DateTime.Now.Year)
				break;
			Console.WriteLine("Некорректный год рождения.");
		}

		var newProfile = new Profile
		{
			Login = login,
			Password = password,
			FirstName = firstName,
			LastName = lastName,
			BirthYear = birthYear
		};

		try
		{
			_profileRepo.Add(newProfile);
			AppInfo.AllProfiles.Add(newProfile);

			AppInfo.CurrentProfileId = newProfile.Id;

			var newUserTodos = new TodoList();
			newUserTodos.OnTodoAdded += (item) => {
				item.ProfileId = newProfile.Id;
				_todoRepo.Add(item);
			};
			newUserTodos.OnTodoDeleted += (item) => _todoRepo.Delete(item.Id);
			newUserTodos.OnTodoUpdated += (item) => _todoRepo.Update(item);
			newUserTodos.OnStatusChanged += (item) => _todoRepo.Update(item);

			AppInfo.Todos = newUserTodos;
			AppInfo.UndoStack.Clear();
			AppInfo.RedoStack.Clear();

			Console.WriteLine("Новый профиль успешно создан и сохранен в БД!");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при сохранении профиля: {ex.Message}");
		}
	}

	private static void RunUserSession()
	{
		Console.WriteLine("Сессия запущена. Введите 'help' для списка команд.");
		while (AppInfo.CurrentProfileId.HasValue)
		{
			Console.Write("> ");
			string input = Console.ReadLine();
			if (input == null) break;
			if (string.IsNullOrWhiteSpace(input)) continue;

			try
			{
				ICommand command = CommandParser.Parse(input);
				command.Execute();

				if (command is IUndo undoableCommand)
				{
					AppInfo.UndoStack.Push(undoableCommand);
					AppInfo.RedoStack.Clear();
				}

				if (!AppInfo.CurrentProfileId.HasValue) break;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка: {ex.Message}");
			}
		}
		Console.WriteLine("Вы вышли из профиля.");
	}
}