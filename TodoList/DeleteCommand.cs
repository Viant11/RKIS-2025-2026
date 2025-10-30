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
                Console.WriteLine("������: ������� ������ ������. ������: delete 1");
                return;
            }

            if (int.TryParse(Argument, out int index) && index > 0 && index <= TodoList.Count)
            {
                TodoList.Delete(index - 1);
                Console.WriteLine($"������ {index} �������");
            }
            else
            {
                Console.WriteLine("������: �������� ������ ������. ������: delete 1");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"������ ��� �������� ������: {ex.Message}");
        }
    }
}