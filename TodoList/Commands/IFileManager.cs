using System;
using System.Collections.Generic;

public interface IFileManager
{
	void EnsureDataDirectory();
	void SaveProfiles(List<Profile> profiles);
	List<Profile> LoadProfiles();
	void SaveUserTodos(Guid userId, TodoList todos);
	TodoList LoadUserTodos(Guid userId);
}