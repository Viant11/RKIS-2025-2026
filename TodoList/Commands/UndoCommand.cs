using System;

public class UndoCommand : ICommand
{
	public void Execute()
	{
		if (!AppInfo.CurrentProfileId.HasValue)
			throw new AuthenticationException("Вы не авторизованы для выполнения этой операции.");

		if (AppInfo.UndoStack.Count > 0)
		{
			IUndo lastCommand = AppInfo.UndoStack.Pop();
			lastCommand.Unexecute();
			AppInfo.RedoStack.Push(lastCommand);
		}
		else
		{
			throw new InvalidCommandException("Стек отмены пуст. Нет действий для отмены.");
		}
	}
}