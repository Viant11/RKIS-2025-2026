public class ReadCommand : ICommand
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
				Console.WriteLine("Ошибка: Укажите индекс задачи. Пример: read 1");
				return;
			}

			if (!int.TryParse(Argument, out int index) || index <= 0 || index > TodoList.Count)
			{
				Console.WriteLine("Ошибка: Неверный индекс задачи. Пример: read 1");
				return;
			}

			TodoList.Read(index - 1);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при чтении задачи: {ex.Message}");
		}
	}
}