using TodoList.Models;

public static class AppInfo
{
	public static TodoList Todos { get; set; }
	public static List<Profile> AllProfiles { get; set; }
	public static int? CurrentProfileId { get; set; }

	public static Stack<IUndo> UndoStack { get; set; }
	public static Stack<IUndo> RedoStack { get; set; }
}