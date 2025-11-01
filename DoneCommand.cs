public class DoneCommand : ICommand
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
                Console.WriteLine("Ошибка: Укажите индекс задачи. Пример: done 1");
                return;
            }

            if (int.TryParse(Argument, out int index) && index > 0 && index <= TodoList.Count)
            {
                TodoList.MarkAsDone(index - 1);
                Console.WriteLine($"Задача {index} отмечена как выполненная");
            }
            else
            {
                Console.WriteLine("Ошибка: Неверный индекс задачи. Пример: done 1");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отметке задачи: {ex.Message}");
        }
    }
}