using System;

public class UnknownCommand : ICommand
{
	public void Execute()
	{
		throw new InvalidCommandException("Неизвестная команда. Введите 'help' для просмотра доступных команд.");
	}
}