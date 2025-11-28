using System;

public class ExitCommand : ICommand
{
	public void Execute()
	{
		Console.WriteLine("Сохранение данных перед выходом...");
		if (AppInfo.Todos != null && AppInfo.CurrentProfile != null)
		{
			FileManager.SaveTodos(AppInfo.Todos, Program.TodoFilePath);
			FileManager.SaveProfile(AppInfo.CurrentProfile, Program.ProfileFilePath);
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