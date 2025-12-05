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
			FileManager.SaveUserTodos(AppInfo.CurrentProfileId.Value, AppInfo.Todos, Program.DataDir);
		}
		else
		{
			Console.WriteLine("Нечего возвращать.");
		}
	}

	public void Unexecute() { }
}