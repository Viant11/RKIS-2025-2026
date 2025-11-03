public class UpdateCommand : ICommand
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
                Console.WriteLine("Ошибка: Укажите индекс и новый текст задачи. Пример: update 1 \"Новый текст\"");
                return;
            }

            if (!Argument.Contains(" "))
            {
                Console.WriteLine("Ошибка: Неверный формат команды. Пример: update 1 \"Новый текст\"");
                return;
            }

            int firstSpaceIndex = Argument.IndexOf(' ');
            string indexStr = Argument.Substring(0, firstSpaceIndex);
            string newText = Argument.Substring(firstSpaceIndex + 1).Trim();

            if (!int.TryParse(indexStr, out int index) || index <= 0 || index > TodoList.Count)
            {
                Console.WriteLine("Ошибка: Неверный индекс задачи. Пример: update 1 \"Новый текст\"");
                return;
            }

            if (newText.StartsWith("\"") && newText.EndsWith("\""))
            {
                newText = newText.Substring(1, newText.Length - 2);
            }

            if (string.IsNullOrWhiteSpace(newText))
            {
                Console.WriteLine("Ошибка: Новый текст задачи не может быть пустым");
                return;
            }

            TodoList.UpdateText(index - 1, newText);
            Console.WriteLine($"Задача {index} обновлена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обновлении задачи: {ex.Message}");
        }
    }
}