using System;

public class UndoCommand : ICommand
{
	public void Execute()
	{
		if (AppInfo.UndoStack.Count > 0)
		{
			ICommand lastCommand = AppInfo.UndoStack.Pop();
			lastCommand.Unexecute();
			AppInfo.RedoStack.Push(lastCommand);
			FileManager.SaveTodos(AppInfo.Todos, Program.TodoFilePath);
		}
		else
		{
			Console.WriteLine("Нечего отменять.");
		}
	}

	public void Unexecute() { }
}