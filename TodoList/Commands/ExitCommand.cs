using System;

public class ExitCommand : ICommand
{
	public void Execute()
	{
		Console.WriteLine("Сохранение данных перед выходом...");

		if (AppInfo.FileManager != null)
		{
			if (AppInfo.CurrentProfileId.HasValue && AppInfo.Todos != null)
			{
				AppInfo.FileManager.SaveUserTodos(AppInfo.CurrentProfileId.Value, AppInfo.Todos);
			}

			if (AppInfo.AllProfiles != null)
			{
				AppInfo.FileManager.SaveProfiles(AppInfo.AllProfiles);
			}
		}

		Console.WriteLine("Программа завершена.");
		Environment.Exit(0);
	}
}