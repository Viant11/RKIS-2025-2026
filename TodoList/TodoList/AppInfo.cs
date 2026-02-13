using System;
using System.Collections.Generic;

public static class AppInfo
{
	public static TodoList Todos { get; set; }
	public static List<Profile> AllProfiles { get; set; }
	public static Guid? CurrentProfileId { get; set; }

	public static Stack<ICommand> UndoStack { get; set; }
	public static Stack<ICommand> RedoStack { get; set; }
}