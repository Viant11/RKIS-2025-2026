using System;

public class RedoCommand : ICommand
{
	public void Execute()
	{
		if (AppInfo.RedoStack.Count > 0)
		{
			ICommand lastUndoneCommand = AppInfo.RedoStack.Pop();
			lastUndoneCommand.Execute();
			AppInfo.UndoStack.Push(lastUndoneCommand);
		}
		else
		{
			Console.WriteLine("Нечего возвращать.");
		}
	}

	public void Unexecute() { }
}