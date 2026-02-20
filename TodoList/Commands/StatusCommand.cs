using System;

public class StatusCommand : ICommand, IUndo
{
	public string? Argument { get; set; }
	private TodoStatus _oldStatus;
	private int _taskIndex;

	public void Execute()
	{
		if (AppInfo.Todos == null)
			throw new InvalidOperationException("TodoList не инициализирован");

		if (string.IsNullOrWhiteSpace(Argument))
			throw new InvalidArgumentException("Укажите индекс и статус. Пример: status 1 InProgress");

		string[] parts = Argument.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length != 2)
			throw new InvalidArgumentException("Неверный формат. Ожидалось: status <ID> <STATUS>");

		if (!int.TryParse(parts[0], out int index))
			throw new InvalidArgumentException($"'{parts[0]}' не является числом.");

		if (index <= 0 || index > AppInfo.Todos.Count)
			throw new TaskNotFoundException($"Задача с индексом {index} не найдена.");

		if (!Enum.TryParse<TodoStatus>(parts[1], true, out var newStatus))
			throw new InvalidArgumentException($"Статус '{parts[1]}' не существует. Доступные: {string.Join(", ", Enum.GetNames(typeof(TodoStatus)))}");

		_taskIndex = index - 1;
		_oldStatus = AppInfo.Todos[_taskIndex].Status;
		AppInfo.Todos.SetStatus(_taskIndex, newStatus);
		Console.WriteLine($"Задаче {index} установлен статус: {newStatus}");
	}

	public void Unexecute()
	{
		AppInfo.Todos.SetStatus(_taskIndex, _oldStatus);
		Console.WriteLine("Действие 'status' отменено.");
	}
}