using System;

public class StatusCommand : ICommand, IUndo
{
	public string? Argument { get; set; }
	private TodoStatus _oldStatus;
	private int _taskIndex;

	public void Execute()
	{
		try
		{
			if (AppInfo.Todos == null)
			{
				Console.WriteLine("Ошибка: TodoList не инициализирован");
				return;
			}

			if (string.IsNullOrWhiteSpace(Argument))
			{
				Console.WriteLine("Ошибка: Укажите индекс и новый статус. Пример: status 1 InProgress");
				return;
			}

			string[] parts = Argument.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length != 2)
			{
				Console.WriteLine("Ошибка: Неверный формат. Пример: status 1 InProgress");
				return;
			}

			if (!int.TryParse(parts[0], out int index) || index <= 0 || index > AppInfo.Todos.Count)
			{
				Console.WriteLine("Ошибка: Неверный индекс задачи.");
				return;
			}

			if (!Enum.TryParse<TodoStatus>(parts[1], true, out var newStatus))
			{
				Console.WriteLine($"Ошибка: Неверный статус. Доступные статусы: {string.Join(", ", Enum.GetNames(typeof(TodoStatus)))}");
				return;
			}

			_taskIndex = index - 1;
			_oldStatus = AppInfo.Todos[_taskIndex].Status;
			AppInfo.Todos.SetStatus(_taskIndex, newStatus);
			Console.WriteLine($"Задаче {index} установлен статус: {newStatus}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при изменении статуса: {ex.Message}");
		}
	}

	public void Unexecute()
	{
		AppInfo.Todos.SetStatus(_taskIndex, _oldStatus);
		Console.WriteLine("Действие 'status' отменено.");
	}
}