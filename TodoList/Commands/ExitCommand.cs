using System;
using TodoList.Models;
using TodoList.Exceptions;

namespace TodoList.Commands;

public class ExitCommand : ICommand
{
	public void Execute()
	{
		Console.WriteLine("Сохранение данных перед выходом...");
		try
		{
			if (AppInfo.Storage != null)
			{
				if (AppInfo.CurrentProfileId.HasValue && AppInfo.Todos != null)
				{
					AppInfo.Storage.SaveTodos(Guid.Empty, AppInfo.Todos);
				}
				if (AppInfo.AllProfiles != null)
				{
					AppInfo.Storage.SaveProfiles(AppInfo.AllProfiles);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка сохранения при выходе: {ex.Message}");
		}

		Console.WriteLine("Программа завершена.");
		Environment.Exit(0);
	}
}