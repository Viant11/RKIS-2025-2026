using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using TodoList;
using TodoList.Models;
using TodoList.Commands;

namespace ProjectTests
{
	public class SyncCommandTests
	{
		[Fact]
		public void SyncCommand_PushMode_CallsSaveTodosOnStorage()
		{
			// Arrange
			var mockStorage = new Mock<IDataStorage>();
			AppInfo.Storage = mockStorage.Object;
			AppInfo.CurrentProfileId = 1;
			AppInfo.Todos = new TaskList();

			var command = new SyncCommand { Push = true };

			// Act
			command.Execute();

			// Assert
			mockStorage.Verify(s => s.SaveTodos(Guid.Empty, It.IsAny<IEnumerable<TodoItem>>()), Times.AtLeastOnce());
		}
	}
}