using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

internal class Program
{
	public static string DataDir { get; private set; }
	public static string ProfileFilePath { get; private set; }

	private static void Main(string[] args)
	{
		if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
		{
			Console.InputEncoding = Encoding.Unicode;
			Console.OutputEncoding = Encoding.Unicode;
		}

		Console.WriteLine("Работу выполнили: Соловьёв Евгений и Тареев Юрий");
		DataDir = "data";
		ProfileFilePath = Path.Combine(DataDir, "profile.csv");

		FileManager.EnsureDataDirectory(DataDir);

		AppInfo.AllProfiles = FileManager.LoadProfiles(ProfileFilePath);

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
					case "exit": AppInfo.CurrentProfileId = Guid.Empty; break;
					case null: AppInfo.CurrentProfileId = Guid.Empty; break;
					default: Console.WriteLine("Неверный ввод. Попробуйте еще раз."); break;
				}
			}
			catch (AuthenticationException ex)
			{
				Console.WriteLine($"Ошибка входа: {ex.Message}");
			}
			catch (DuplicateLoginException ex)
			{
				Console.WriteLine($"Ошибка регистрации: {ex.Message}");
			}
			catch (InvalidArgumentException ex)
			{
				Console.WriteLine($"Ошибка ввода: {ex.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка: {ex.Message}");
			}
		}
		if (AppInfo.CurrentProfileId == Guid.Empty) AppInfo.CurrentProfileId = null;
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
			throw new AuthenticationException("Неверный логин или пароль.");
		}

		AppInfo.CurrentProfileId = foundProfile.Id;
		var userTodos = FileManager.LoadUserTodos(foundProfile.Id, DataDir);
		userTodos.OnTodoAdded += FileManager.SaveTodoListOnChange;
		userTodos.OnTodoDeleted += FileManager.SaveTodoListOnChange;
		userTodos.OnTodoUpdated += FileManager.SaveTodoListOnChange;
		userTodos.OnStatusChanged += FileManager.SaveTodoListOnChange;
		AppInfo.Todos = userTodos;

		AppInfo.UndoStack.Clear();
		AppInfo.RedoStack.Clear();
		Console.WriteLine($"Добро пожаловать, {foundProfile.FirstName}!");
	}

	private static void CreateNewProfile()
	{
		Console.Write("Введите новый логин: ");
		string login = Console.ReadLine();

		if (string.IsNullOrWhiteSpace(login))
			throw new InvalidArgumentException("Логин не может быть пустым.");

		if (AppInfo.AllProfiles.Any(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
		{
			throw new DuplicateLoginException($"Логин '{login}' уже занят.");
		}

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
			string input = Console.ReadLine();
			if (int.TryParse(input, out birthYear) && birthYear > 1900 && birthYear <= DateTime.Now.Year)
				break;
			Console.WriteLine("Некорректный год рождения. Введите число от 1900 до текущего года.");
		}

		var newProfile = new Profile(firstName, lastName, birthYear, login, password, Guid.NewGuid());
		AppInfo.AllProfiles.Add(newProfile);
		FileManager.SaveProfiles(AppInfo.AllProfiles, ProfileFilePath);

		AppInfo.CurrentProfileId = newProfile.Id;
		var newUserTodos = new TodoList();
		newUserTodos.OnTodoAdded += FileManager.SaveTodoListOnChange;
		newUserTodos.OnTodoDeleted += FileManager.SaveTodoListOnChange;
		newUserTodos.OnTodoUpdated += FileManager.SaveTodoListOnChange;
		newUserTodos.OnStatusChanged += FileManager.SaveTodoListOnChange;
		AppInfo.Todos = newUserTodos;
		FileManager.SaveUserTodos(newProfile.Id, AppInfo.Todos, DataDir);

		AppInfo.UndoStack.Clear();
		AppInfo.RedoStack.Clear();
		Console.WriteLine("Новый профиль успешно создан!");
	}

	private static void RunUserSession()
	{
		Console.WriteLine("Сессия пользователя запущена. Введите 'help' для списка команд.");
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

				if (!AppInfo.CurrentProfileId.HasValue) break;

				if (command is IUndo undoableCommand)
				{
					AppInfo.UndoStack.Push(undoableCommand);
					AppInfo.RedoStack.Clear();
				}
			}
			catch (TaskNotFoundException ex)
			{
				Console.WriteLine($"Ошибка задачи: {ex.Message}");
			}
			catch (AuthenticationException ex)
			{
				Console.WriteLine($"Ошибка авторизации: {ex.Message}");
			}
			catch (InvalidCommandException ex)
			{
				Console.WriteLine($"Ошибка команды: {ex.Message}");
			}
			catch (InvalidArgumentException ex)
			{
				Console.WriteLine($"Ошибка аргументов: {ex.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Неожиданная ошибка.");
			}
		}
		Console.WriteLine("Вы вышли из профиля.");
	}
}