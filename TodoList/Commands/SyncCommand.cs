using System;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Models;
using TodoList.Exceptions;

namespace TodoList.Commands;

public class SyncCommand : ICommand
{
	public bool Pull { get; set; }
	public bool Push { get; set; }

	public IDataStorage? ExternalStorage { get; set; }

	public void Execute()
	{
		if (!AppInfo.CurrentProfileId.HasValue)
		{
			Console.WriteLine("Ошибка: Вы не авторизованы для выполнения синхронизации.");
			return;
		}

		ExecuteAsync().GetAwaiter().GetResult();
	}

	private async Task ExecuteAsync()
	{
		IDataStorage apiStorage = ExternalStorage ?? new ApiDataStorage();

		if (apiStorage is ApiDataStorage realApi)
		{
			Console.WriteLine("Проверка доступности сервера...");
			if (!await realApi.IsServerAvailableAsync())
			{
				Console.WriteLine("Ошибка: сервер недоступен.");
				return;
			}
			Console.WriteLine("Сервер доступен. Начинаю синхронизацию...");
		}

		try
		{
			if (Push)
			{
				await PushToServer(apiStorage);
			}
			else
			{
				await PullFromServer(apiStorage);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка синхронизации: {ex.Message}");
		}
	}

	private async Task PushToServer(IDataStorage storage)
	{
		Console.WriteLine("Отправка данных на сервер...");

		if (AppInfo.AllProfiles != null)
		{
			storage.SaveProfiles(AppInfo.AllProfiles);
			Console.WriteLine($"✓ Отправлено {AppInfo.AllProfiles.Count} профилей");
		}

		if (AppInfo.CurrentProfileId.HasValue && AppInfo.Todos != null)
		{
			storage.SaveTodos(Guid.Empty, AppInfo.Todos);
			Console.WriteLine($"✓ Отправлено {AppInfo.Todos.Count} задач");
		}

		Console.WriteLine("Синхронизация (push) завершена успешно.");
	}

	private async Task PullFromServer(IDataStorage storage)
	{
		Console.WriteLine("Загрузка данных с сервера...");

		var serverProfiles = storage.LoadProfiles().ToList();
		if (serverProfiles.Any())
		{
			AppInfo.AllProfiles = serverProfiles;
			Console.WriteLine($"Загружено {serverProfiles.Count} профилей");
		}

		if (AppInfo.CurrentProfileId.HasValue)
		{
			var serverTodos = storage.LoadTodos(Guid.Empty).ToList();

			if (serverTodos.Any())
			{
				var newTodoList = new TodoList(serverTodos);

				newTodoList.OnTodoAdded += OnTodoChanged;
				newTodoList.OnTodoDeleted += OnTodoChanged;
				newTodoList.OnTodoUpdated += OnTodoChanged;
				newTodoList.OnStatusChanged += OnTodoChanged;

				AppInfo.Todos = newTodoList;
				Console.WriteLine($"Загружено {serverTodos.Count} задач");
			}
		}

		Console.WriteLine("Синхронизация (pull) завершена успешно.");
	}

	private void OnTodoChanged(TodoItem item)
	{
		if (AppInfo.CurrentProfileId.HasValue && AppInfo.Todos != null && AppInfo.Storage != null)
		{
			try
			{
				AppInfo.Storage.SaveTodos(Guid.Empty, AppInfo.Todos);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\n[ВНИМАНИЕ] Ошибка автосохранения: {ex.Message}");
			}
		}
	}
}