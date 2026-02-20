using System;

public class ReadCommand : ICommand
{
	public string? Argument { get; set; }

	public void Execute()
	{
		if (AppInfo.Todos == null)
			throw new InvalidOperationException("TodoList не инициализирован");

		if (string.IsNullOrWhiteSpace(Argument))
			throw new InvalidArgumentException("Укажите индекс задачи. Пример: read 1");

		if (!int.TryParse(Argument, out int index))
			throw new InvalidArgumentException($"'{Argument}' не является корректным числом.");

		if (index <= 0 || index > AppInfo.Todos.Count)
			throw new TaskNotFoundException($"Задача с индексом {index} не найдена.");

		AppInfo.Todos.Read(index - 1);
	}
}