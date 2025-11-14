using System;

public class ExitCommand : ICommand
{
	public TodoList? TodoList { get; set; }
	public Profile? UserProfile { get; set; }

	public void Execute()
	{
		Console.WriteLine("Сохранение данных перед выходом...");
		if (TodoList != null && UserProfile != null)
		{
			FileManager.SaveTodos(TodoList, Program.TodoFilePath);
			FileManager.SaveProfile(UserProfile, Program.ProfileFilePath);
			Console.WriteLine("Данные успешно сохранены.");
		}
		else
		{
			Console.WriteLine("Не удалось сохранить данные: списки не инициализированы.");
		}

		Console.WriteLine("Программа завершена.");
		Environment.Exit(0);
	}
}