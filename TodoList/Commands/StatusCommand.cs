using System;

public class StatusCommand : ICommand
{
	public TodoList? TodoList { get; set; }
	public string? Argument { get; set; }

	public void Execute()
	{
		try
		{
			if (TodoList == null)
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

			if (!int.TryParse(parts[0], out int index) || index <= 0 || index > TodoList.Count)
			{
				Console.WriteLine("Ошибка: Неверный индекс задачи.");
				return;
			}

			if (!Enum.TryParse<TodoStatus>(parts[1], true, out var newStatus))
			{
				Console.WriteLine($"Ошибка: Неверный статус. Доступные статусы: {string.Join(", ", Enum.GetNames(typeof(TodoStatus)))}");
				return;
			}

			TodoList.SetStatus(index - 1, newStatus);
			Console.WriteLine($"Задаче {index} установлен статус: {newStatus}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при изменении статуса: {ex.Message}");
		}
	}
}