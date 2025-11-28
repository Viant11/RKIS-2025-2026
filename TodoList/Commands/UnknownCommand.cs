using System;

public class UnknownCommand : ICommand
{
	public void Execute()
	{
		Console.WriteLine("Неизвестная команда. Введите команду help для просмотра доступных команд");
	}

	public void Unexecute() { }
}