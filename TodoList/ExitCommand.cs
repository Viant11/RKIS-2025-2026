public class ExitCommand : ICommand
{
    public void Execute()
    {
        Console.WriteLine("��������� ���������.");
        Environment.Exit(0);
    }
}