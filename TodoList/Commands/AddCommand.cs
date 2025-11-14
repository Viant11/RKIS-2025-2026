using System;
using System.IO;

public class AddCommand : ICommand
{
	public TodoList? TodoList { get; set; }
	public string? TaskDescription { get; set; }
	public bool MultilineFlag { get; set; }

	public void Execute()
	{
		if (TodoList == null)
		{
			Console.WriteLine("Ошибка: TodoList не инициализирован");
			return;
		}

		string taskText = "";

		if (MultilineFlag)
		{
			taskText = ReadMultilineInput();

			if (string.IsNullOrWhiteSpace(taskText))
			{
				Console.WriteLine("Ошибка: Задача не может быть пустой");
				return;
			}
		}
		else
		{
			if (string.IsNullOrWhiteSpace(TaskDescription))
			{
				Console.WriteLine("Ошибка: Укажите задачу. Пример: add Новая задача");
				return;
			}

			taskText = TaskDescription.Trim();
		}

		TodoList.Add(new TodoItem(taskText));
		Console.WriteLine("Задача добавлена!");

		try
		{
			string dataDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
			string todoFilePath = Path.Combine(dataDir, "todo.csv");

			FileManager.EnsureDataDirectory(dataDir);
			FileManager.SaveTodos(TodoList, todoFilePath);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при сохранении задач: {ex.Message}");
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

			if (line == "!end")
				break;

			if (!string.IsNullOrWhiteSpace(line))
			{
				if (!string.IsNullOrEmpty(result))
					result += "\n";
				result += line;
			}
		}

		return result.Trim();
	}
}