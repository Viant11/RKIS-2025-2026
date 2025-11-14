using System;
using System.IO;
using System.Linq;
using System.Text;

internal class Program
{
	public static string ProfileFilePath { get; private set; }
	public static string TodoFilePath { get; private set; }

	private static void Main(string[] args)
	{
		if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
		{
			Console.InputEncoding = Encoding.Unicode;
			Console.OutputEncoding = Encoding.Unicode;
		}

		Console.WriteLine("Работу выполнили: Соловьёв Евгений и Тареев Юрий");
		string dataDir = "data";

		ProfileFilePath = Path.Combine(dataDir, "profile.txt");
		TodoFilePath = Path.Combine(dataDir, "todo.csv");

		FileManager.EnsureDataDirectory(dataDir);
		Profile userProfile = FileManager.LoadProfile(ProfileFilePath) ?? CreateUserProfile(ProfileFilePath);
		TodoList todos = FileManager.LoadTodos(TodoFilePath);
		bool isRunning = true;

		Console.WriteLine("Введите команду help для просмотра всех команд.");

		while (isRunning)
		{
			Console.Write("> ");
			string userCommand = Console.ReadLine();
			if (userCommand?.ToLower() == "exit")
			{
				FileManager.SaveProfile(userProfile, ProfileFilePath);
				FileManager.SaveTodos(todos, TodoFilePath);
				isRunning = false;
				continue;
			}
			try
			{
				ICommand command = CommandParser.Parse(userCommand, todos, userProfile);
				if (command != null)
				{
					command.Execute();

					if (command is AddCommand || command is DeleteCommand ||
						command is UpdateCommand || command is StatusCommand)
					{
						FileManager.SaveTodos(todos, TodoFilePath);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка: {ex.Message}");
			}
		}
	}

	private static Profile CreateUserProfile(string profileFilePath)
	{
		string name, surname;
		int yearOfBirth;
		string fullName;

		while (true)
		{
			Console.WriteLine("Напишите ваше имя и фамилию:");
			fullName = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(fullName))
			{
				Console.WriteLine("Ошибка: Имя и фамилия не могут быть пустыми.");
				continue;
			}

			if (fullName.Any(char.IsDigit))
			{
				Console.WriteLine("Ошибка: Имя и фамилия не могут содержать цифры.");
				continue;
			}

			break;
		}

		string[] splitFullName = fullName.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
		name = splitFullName[0];
		surname = splitFullName.Length > 1 ? splitFullName[1] : "";

		int currentYear = DateTime.Now.Year;
		while (true)
		{
			Console.WriteLine("Введите Год Рождения:");
			if (int.TryParse(Console.ReadLine(), out yearOfBirth))
			{
				if (yearOfBirth > currentYear)
				{
					Console.WriteLine($"Ошибка: Год рождения не может быть в будущем. Пожалуйста, введите год до {currentYear}.");
				}
				else if (yearOfBirth < 1900)
				{
					Console.WriteLine("Ошибка: Указан слишком ранний год рождения. Пожалуйста, введите корректный год.");
				}
				else
				{
					break;
				}
			}
			else
			{
				Console.WriteLine("Ошибка: Неверный формат года. Введите четыре цифры, например: 2000");
			}
		}

		Profile profile = new Profile(name, surname, yearOfBirth);
		Console.WriteLine("Добавлен пользователь: " + profile.GetInfo());
		FileManager.SaveProfile(profile, profileFilePath);
		return profile;
	}
}