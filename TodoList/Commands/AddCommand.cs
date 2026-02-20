using System;
using System.IO;

public class AddCommand : ICommand, IUndo
{
	public string? TaskDescription { get; set; }
	public bool MultilineFlag { get; set; }

	public void Execute()
	{
		if (AppInfo.Todos == null)
			throw new InvalidOperationException("TodoList не инициализирован");

		string taskText = "";

		if (MultilineFlag)
		{
			taskText = ReadMultilineInput();
		}
		else
		{
			if (string.IsNullOrWhiteSpace(TaskDescription))
				throw new InvalidArgumentException("Текст задачи не может быть пустым. Пример: add Купить молоко");

			taskText = TaskDescription.Trim();
		}

		if (string.IsNullOrWhiteSpace(taskText))
			throw new InvalidArgumentException("Попытка добавить пустую задачу.");

		AppInfo.Todos.Add(new TodoItem(taskText));
		Console.WriteLine("Задача добавлена!");
	}

	public void Unexecute()
	{
		if (AppInfo.Todos != null && AppInfo.Todos.Count > 0)
		{
			AppInfo.Todos.Delete(AppInfo.Todos.Count - 1);
			Console.WriteLine("Действие 'add' отменено.");
		}
	}

	private string ReadMultilineInput()
	{
		Console.WriteLine("Многострочный режим. Вводите текст задачи построчно. Для завершения введите !end");
		string result = "";
		string? line;
		while (true)
		{
			Console.Write("> ");
			line = Console.ReadLine();
			if (line == null || line == "!end") break;

			if (!string.IsNullOrWhiteSpace(line))
			{
				if (!string.IsNullOrEmpty(result)) result += "\n";
				result += line;
			}
		}
		return result.Trim();
	}
}