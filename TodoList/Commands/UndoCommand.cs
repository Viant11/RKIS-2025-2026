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
			FileManager.SaveUserTodos(AppInfo.CurrentProfileId.Value, AppInfo.Todos, Program.DataDir);
		}
		else
		{
			Console.WriteLine("Нечего отменять.");
		}
	}

	public void Unexecute() { }
}