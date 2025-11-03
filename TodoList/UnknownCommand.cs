public class UnknownCommand : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Неизвестная команда. Введите команду help для просмотра доступных команд");
    }
}