using System;

public class ReadCommand : ICommand
{
	public string? Argument { get; set; }

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
				Console.WriteLine("Ошибка: Укажите индекс задачи. Пример: read 1");
				return;
			}

			if (!int.TryParse(Argument, out int index) || index <= 0 || index > AppInfo.Todos.Count)
			{
				Console.WriteLine("Ошибка: Неверный индекс задачи. Пример: read 1");
				return;
			}

			AppInfo.Todos.Read(index - 1);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при чтении задачи: {ex.Message}");
		}
	}

	public void Unexecute() { }
}