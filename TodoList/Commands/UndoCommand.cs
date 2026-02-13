using System;

public class UndoCommand : ICommand
{
	public void Execute()
	{
		if (AppInfo.UndoStack.Count > 0)
		{
			IUndo lastCommand = AppInfo.UndoStack.Pop();
			lastCommand.Unexecute();
			AppInfo.RedoStack.Push(lastCommand);
		}
		else
		{
			Console.WriteLine("Нечего отменять.");
		}
	}
}