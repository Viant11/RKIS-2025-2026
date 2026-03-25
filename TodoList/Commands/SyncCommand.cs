using System;
using System.Linq;
using System.Threading.Tasks;

public class SyncCommand : ICommand
{
	public bool Pull { get; set; }
	public bool Push { get; set; }

	public void Execute()
	{
		if (!AppInfo.CurrentProfileId.HasValue)
		{
			throw new AuthenticationException("Вы не авторизованы для выполнения синхронизации.");
		}

		var syncTask = ExecuteAsync();
		syncTask.Wait();
	}

	private async Task ExecuteAsync()
	{
		var apiStorage = new ApiDataStorage();

		Console.WriteLine("Проверка доступности сервера...");
		if (!await apiStorage.IsServerAvailableAsync())
		{
			Console.WriteLine("Ошибка: сервер недоступен.");
			return;
		}

		Console.WriteLine("Сервер доступен. Начинаю синхронизацию...");

		try
		{
			if (Push)
			{
				await PushToServer(apiStorage);
			}
			else if (Pull)
			{
				await PullFromServer(apiStorage);
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

	private async Task PushToServer(ApiDataStorage apiStorage)
	{
		Console.WriteLine("Отправка данных на сервер...");

		if (AppInfo.AllProfiles != null)
		{
			apiStorage.SaveProfiles(AppInfo.AllProfiles);
			Console.WriteLine($"✓ Отправлено {AppInfo.AllProfiles.Count} профилей");
		}

		if (AppInfo.CurrentProfileId.HasValue && AppInfo.Todos != null)
		{
			apiStorage.SaveTodos(AppInfo.CurrentProfileId.Value, AppInfo.Todos);
			Console.WriteLine($"✓ Отправлено {AppInfo.Todos.Count} задач");
		}

		Console.WriteLine("Синхронизация (push) завершена успешно.");
	}

	private async Task PullFromServer(ApiDataStorage apiStorage)
	{
		Console.WriteLine("Загрузка данных с сервера...");

		var serverProfiles = apiStorage.LoadProfiles().ToList();
		if (serverProfiles.Any())
		{
			Console.WriteLine($"Загружено {serverProfiles.Count} профилей с сервера");

			AppInfo.AllProfiles = serverProfiles;

			AppInfo.Storage.SaveProfiles(AppInfo.AllProfiles);
			Console.WriteLine("✓ Локальные профили обновлены");
		}
		else
		{
			Console.WriteLine("На сервере нет данных профилей");
		}

		if (AppInfo.CurrentProfileId.HasValue)
		{
			var serverTodos = apiStorage.LoadTodos(AppInfo.CurrentProfileId.Value).ToList();

			if (serverTodos.Any())
			{
				Console.WriteLine($"Загружено {serverTodos.Count} задач с сервера");

				if (AppInfo.Todos != null)
				{
					var oldTodos = AppInfo.Todos;

					var newTodoList = new TodoList(serverTodos);

					newTodoList.OnTodoAdded += OnTodoChanged;
					newTodoList.OnTodoDeleted += OnTodoChanged;
					newTodoList.OnTodoUpdated += OnTodoChanged;
					newTodoList.OnStatusChanged += OnTodoChanged;

					AppInfo.Todos = newTodoList;

					AppInfo.Storage.SaveTodos(AppInfo.CurrentProfileId.Value, AppInfo.Todos);
					Console.WriteLine("✓ Локальные задачи обновлены");
				}
				else
				{
					var newTodoList = new TodoList(serverTodos);
					newTodoList.OnTodoAdded += OnTodoChanged;
					newTodoList.OnTodoDeleted += OnTodoChanged;
					newTodoList.OnTodoUpdated += OnTodoChanged;
					newTodoList.OnStatusChanged += OnTodoChanged;
					AppInfo.Todos = newTodoList;
				}
			}
			else
			{
				Console.WriteLine("На сервере нет данных задач для текущего пользователя");
			}
		}

		Console.WriteLine("Синхронизация (pull) завершена успешно.");
	}

	private void OnTodoChanged(TodoItem item)
	{
		if (AppInfo.CurrentProfileId.HasValue && AppInfo.Todos != null)
		{
			try
			{
				AppInfo.Storage.SaveTodos(AppInfo.CurrentProfileId.Value, AppInfo.Todos);
			}
			catch (StorageException ex)
			{
				Console.WriteLine($"\n[ВНИМАНИЕ] Ошибка автосохранения: {ex.Message}");
			}
		}
	}
}