using System;

public class DeleteCommand : ICommand, IUndo
{
	public string? Argument { get; set; }
	private TodoItem _deletedItem;
	private int _deletedItemIndex;

	public void Execute()
	{
		if (AppInfo.Todos == null)
			throw new InvalidOperationException("TodoList не инициализирован");

		if (string.IsNullOrWhiteSpace(Argument))
			throw new InvalidArgumentException("Укажите индекс задачи для удаления. Пример: delete 1");

		if (!int.TryParse(Argument, out int index))
			throw new InvalidArgumentException($"'{Argument}' не является числом.");

		if (index <= 0 || index > AppInfo.Todos.Count)
			throw new TaskNotFoundException($"Задача с индексом {index} не найдена. Всего задач: {AppInfo.Todos.Count}");

		_deletedItemIndex = index - 1;
		_deletedItem = AppInfo.Todos[_deletedItemIndex];
		AppInfo.Todos.Delete(_deletedItemIndex);
		Console.WriteLine($"Задача {index} удалена");
	}

	public void Unexecute()
	{
		if (_deletedItem != null)
		{
			AppInfo.Todos.Insert(_deletedItemIndex, _deletedItem);
			Console.WriteLine("Действие 'delete' отменено.");
		}
	}
}