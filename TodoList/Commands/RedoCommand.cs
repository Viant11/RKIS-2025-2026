using System;

public class RedoCommand : ICommand
{
	public void Execute()
	{
		if (!AppInfo.CurrentProfileId.HasValue)
			throw new AuthenticationException("Вы не авторизованы для выполнения этой операции.");

		if (AppInfo.RedoStack.Count > 0)
		{
			IUndo lastUndoneCommand = AppInfo.RedoStack.Pop();

			if (lastUndoneCommand is ICommand command)
			{
				command.Execute();
				AppInfo.UndoStack.Push(lastUndoneCommand);
			}
		}
		else
		{
			throw new InvalidCommandException("Стек возврата пуст. Нет действий для повтора.");
		}
	}
}