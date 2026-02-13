using System;

public class RedoCommand : ICommand
{
	public void Execute()
	{
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
			Console.WriteLine("Нечего возвращать.");
		}
	}
}