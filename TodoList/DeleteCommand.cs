public class DeleteCommand : ICommand
{
    public TodoList TodoList { get; set; }
    public string Argument { get; set; }

    public void Execute()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Argument))
            {
                Console.WriteLine("Ошибка: Укажите индекс задачи. Пример: delete 1");
                return;
            }

            if (int.TryParse(Argument, out int index) && index > 0 && index <= TodoList.Count)
            {
                TodoList.Delete(index - 1);
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
}