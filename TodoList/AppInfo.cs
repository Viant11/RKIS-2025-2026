using System;
using System.Collections.Generic;
using TodoList.Models;

namespace TodoList;

public static class AppInfo
{
	public static IDataStorage? Storage { get; set; }
	public static TodoList? Todos { get; set; }
	public static List<Profile> AllProfiles { get; set; } = new();
	public static int? CurrentProfileId { get; set; }

	public static Stack<IUndo> UndoStack { get; set; } = new();
	public static Stack<IUndo> RedoStack { get; set; } = new();
}