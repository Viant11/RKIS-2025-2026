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
                Console.WriteLine("������: TodoList �� ���������������");
                return;
            }

            if (string.IsNullOrWhiteSpace(Argument))
            {
                Console.WriteLine("������: ������� ������ ������. ������: done 1");
                return;
            }

            if (int.TryParse(Argument, out int index) && index > 0 && index <= TodoList.Count)
            {
                TodoList.MarkAsDone(index - 1);
                Console.WriteLine($"������ {index} �������� ��� �����������");
            }
            else
            {
                Console.WriteLine("������: �������� ������ ������. ������: done 1");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"������ ��� ������� ������: {ex.Message}");
        }
    }
}