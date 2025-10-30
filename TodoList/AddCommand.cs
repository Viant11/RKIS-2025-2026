using System;

public class AddCommand : ICommand
{
    public TodoList? TodoList { get; set; }
    public string? TaskDescription { get; set; }
    public bool MultilineFlag { get; set; }

    public void Execute()
    {
        if (TodoList == null)
        {
            Console.WriteLine("������: TodoList �� ���������������");
            return;
        }

        string taskText = "";

        if (MultilineFlag)
        {
            taskText = ReadMultilineInput();

            if (string.IsNullOrWhiteSpace(taskText))
            {
                Console.WriteLine("������: ������ �� ����� ���� ������");
                return;
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(TaskDescription))
            {
                Console.WriteLine("������: ������� ������. ������: add ����� ������");
                return;
            }

            taskText = TaskDescription.Trim();
        }

        TodoList.Add(new TodoItem(taskText));
        Console.WriteLine("������ ���������!");
    }

    private string ReadMultilineInput()
    {
        Console.WriteLine("������������� �����. ������� ����� ������ ���������. ��� ���������� ������� !end");
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
