using System;
using System.Collections.Generic;

public static class AppInfo
{
	public static TodoList Todos { get; set; }
	public static List<Profile> AllProfiles { get; set; }
	public static Guid? CurrentProfileId { get; set; }

	public static Stack<IUndo> UndoStack { get; set; }
	public static Stack<IUndo> RedoStack { get; set; }
}