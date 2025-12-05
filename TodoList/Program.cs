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
		Console.WriteLine("Войти в существующий профиль? [y/n] (или 'exit' для выхода)");
		string choice = Console.ReadLine()?.ToLower();

		if (choice == "y")
		{
			LoginUser();
		}
		else if (choice == "n")
		{
			CreateNewProfile();
		}
		else if (choice == "exit")
		{
			AppInfo.CurrentProfileId = null;
		}
		else
		{
			Console.WriteLine("Неверный ввод. Попробуйте еще раз.");
			HandleUserLogin();
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
			AppInfo.CurrentProfileId = null;
			HandleUserLogin();
		}
	}

	private static void CreateNewProfile()
	{
		Console.Write("Введите новый логин: ");
		string login = Console.ReadLine();
		if (string.IsNullOrWhiteSpace(login) || AppInfo.AllProfiles.Any(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
		{
			Console.WriteLine("Этот логин уже занят или некорректен.");
			CreateNewProfile();
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
		Console.WriteLine("Сессия пользователя запущена. Введите 'exit' для выхода.");
		while (true)
		{
			string input = Console.ReadLine();
			if (input == "exit") break;
		}
	}
}