using System;
using System.IO;

public class DeleteCommand : ICommand
{
	public string? Argument { get; set; }
	private TodoItem _deletedItem;
	private int _deletedItemIndex;

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
				Console.WriteLine("Ошибка: Укажите индекс задачи. Пример: delete 1");
				return;
			}

			if (int.TryParse(Argument, out int index) && index > 0 && index <= AppInfo.Todos.Count)
			{
				_deletedItemIndex = index - 1;
				_deletedItem = AppInfo.Todos[_deletedItemIndex];
				AppInfo.Todos.Delete(_deletedItemIndex);
				Console.WriteLine($"Задача {index} удалена");
			}
			else
			{
				Console.WriteLine("Ошибка: Неверный индекс задачи. Пример: delete 1");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при удалении задачи: {ex.Message}");
		}
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