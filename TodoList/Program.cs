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
		AppInfo.UndoStack = new Stack<ICommand>();
		AppInfo.RedoStack = new Stack<ICommand>();


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

	private static void HandleUserLogin()
	{
		while (AppInfo.CurrentProfileId == null)
		{
			Console.WriteLine("\nВойти в существующий профиль? [y/n] (или 'exit' для выхода)");
			string choice = Console.ReadLine()?.ToLower();

			switch (choice)
			{
				case "y":
					LoginUser();
					break;
				case "n":
					CreateNewProfile();
					break;
				case "exit":
					AppInfo.CurrentProfileId = Guid.Empty;
					break;
				default:
					Console.WriteLine("Неверный ввод. Попробуйте еще раз.");
					break;
			}
		}

		if (AppInfo.CurrentProfileId == Guid.Empty)
		{
			AppInfo.CurrentProfileId = null;
		}
	}

	private static void LoginUser()
	{
		Console.Write("Введите логин: ");
		string login = Console.ReadLine();
		Console.Write("Введите пароль: ");
		string password = Console.ReadLine();

		Profile foundProfile = AppInfo.AllProfiles.FirstOrDefault(p => p.Login == login && p.Password == password);

		if (foundProfile != null)
		{
			AppInfo.CurrentProfileId = foundProfile.Id;
			AppInfo.Todos = FileManager.LoadUserTodos(foundProfile.Id, DataDir);
			Console.WriteLine($"Добро пожаловать, {foundProfile.FirstName}!");
		}
		else
		{
			Console.WriteLine("Неверный логин или пароль.");
		}
	}

	private static void CreateNewProfile()
	{
		Console.Write("Введите новый логин: ");
		string login = Console.ReadLine();
		if (string.IsNullOrWhiteSpace(login) || AppInfo.AllProfiles.Any(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
		{
			Console.WriteLine("Этот логин уже занят или некорректен.");
			return;
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
			if (int.TryParse(Console.ReadLine(), out birthYear) && birthYear > 1900 && birthYear <= DateTime.Now.Year)
			{
				break;
			}
			Console.WriteLine("Некорректный год рождения.");
		}

		var newProfile = new Profile(firstName, lastName, birthYear, login, password, Guid.NewGuid());
		AppInfo.AllProfiles.Add(newProfile);
		FileManager.SaveProfiles(AppInfo.AllProfiles, ProfileFilePath);

		AppInfo.CurrentProfileId = newProfile.Id;
		AppInfo.Todos = new TodoList();
		FileManager.SaveUserTodos(newProfile.Id, AppInfo.Todos, DataDir);

		Console.WriteLine("Новый профиль успешно создан!");
	}

	private static void RunUserSession()
	{
		Console.WriteLine("Сессия пользователя запущена. Введите 'help' для списка команд.");
		while (AppInfo.CurrentProfileId.HasValue)
		{
			Console.Write("> ");
			string input = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(input)) continue;

			ICommand command = CommandParser.Parse(input);

			if (command is UndoCommand || command is RedoCommand)
			{
				command.Execute();
				continue;
			}

			command.Execute();

			if (!AppInfo.CurrentProfileId.HasValue)
			{
				break;
			}

			if (command is AddCommand || command is DeleteCommand || command is UpdateCommand || command is StatusCommand)
			{
				AppInfo.UndoStack.Push(command);
				AppInfo.RedoStack.Clear();
				FileManager.SaveUserTodos(AppInfo.CurrentProfileId.Value, AppInfo.Todos, DataDir);
			}
		}
		Console.WriteLine("Вы вышли из профиля.");
	}
}