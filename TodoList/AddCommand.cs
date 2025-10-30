public class AddCommand : ICommand
{
    public TodoList TodoList { get; set; }
    public string TaskDescription { get; set; }
    public bool MultilineFlag { get; set; }

    public void Execute()
    {
        if (string.IsNullOrWhiteSpace(TaskDescription))
        {
            Console.WriteLine("������: ������� ������. ������: add ����� ������");
            return;
        }

        string newTask = TaskDescription.Trim();

        if (string.IsNullOrWhiteSpace(newTask))
        {
            Console.WriteLine("������: ������ �� ����� ���� ������");
            return;
        }

        TodoList.Add(new TodoItem(newTask));
        Console.WriteLine("������ ���������!");
    }
}