using System;

public class ExitCommand : ICommand
{
	public void Execute()
	{
		Console.WriteLine("Сохранение данных перед выходом...");
		if (AppInfo.CurrentProfileId.HasValue && AppInfo.Todos != null)
		{
			FileManager.SaveUserTodos(AppInfo.CurrentProfileId.Value, AppInfo.Todos, Program.DataDir);
		}

		if (AppInfo.AllProfiles != null)
		{
			FileManager.SaveProfiles(AppInfo.AllProfiles, Program.ProfileFilePath);
		}

		Console.WriteLine("Программа завершена.");
		Environment.Exit(0);
	}
}