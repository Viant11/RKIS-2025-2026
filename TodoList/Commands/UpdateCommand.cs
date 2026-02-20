using System;

public class UpdateCommand : ICommand, IUndo
{
	public string? Argument { get; set; }
	private string _oldText;
	private int _taskIndex;

	public void Execute()
	{
		if (AppInfo.Todos == null)
			throw new InvalidOperationException("TodoList не инициализирован");

		if (string.IsNullOrWhiteSpace(Argument))
			throw new InvalidArgumentException("Укажите индекс и новый текст. Пример: update 1 \"Новый текст\"");

		if (!Argument.Contains(" "))
			throw new InvalidArgumentException("Неверный формат команды. Ожидалось: update <ID> <TEXT>");

		int firstSpaceIndex = Argument.IndexOf(' ');
		string indexStr = Argument.Substring(0, firstSpaceIndex);
		string newText = Argument.Substring(firstSpaceIndex + 1).Trim();

		if (!int.TryParse(indexStr, out int index))
			throw new InvalidArgumentException($"'{indexStr}' не является числом.");

		if (index <= 0 || index > AppInfo.Todos.Count)
			throw new TaskNotFoundException($"Задача с индексом {index} не найдена.");

		if (newText.StartsWith("\"") && newText.EndsWith("\""))
		{
			newText = newText.Substring(1, newText.Length - 2);
		}

		if (string.IsNullOrWhiteSpace(newText))
			throw new InvalidArgumentException("Новый текст задачи не может быть пустым.");

		_taskIndex = index - 1;
		_oldText = AppInfo.Todos[_taskIndex].Text;
		AppInfo.Todos.UpdateText(_taskIndex, newText);
		Console.WriteLine($"Задача {index} обновлена");
	}

	public void Unexecute()
	{
		AppInfo.Todos.UpdateText(_taskIndex, _oldText);
		Console.WriteLine("Действие 'update' отменено.");
	}
}