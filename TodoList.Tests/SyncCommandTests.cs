using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using TodoList;
using TodoList.Models;
using TodoList.Commands;
using TaskList = TodoList.TodoList;

namespace ProjectTests
{
	public class SyncCommandTests
	{
		[Fact]
		public void SyncCommand_PushMode_CallsSaveTodosOnStorage()
		{
			// Arrange
			var mockStorage = new Mock<IDataStorage>();

			AppInfo.CurrentProfileId = 1;
			AppInfo.Todos = new TaskList();

			var command = new SyncCommand
			{
				Push = true,
				ExternalStorage = mockStorage.Object
			};

			// Act
			command.Execute();

			// Assert
			mockStorage.Verify(s => s.SaveTodos(
				It.IsAny<Guid>(),
				It.IsAny<IEnumerable<TodoItem>>()),
				Times.AtLeastOnce());
		}

		[Fact]
		public void SyncCommand_NoProfile_DoesNotCallStorage()
		{
			// Arrange
			var mockStorage = new Mock<IDataStorage>();
			AppInfo.CurrentProfileId = null;

			var command = new SyncCommand
			{
				Push = true,
				ExternalStorage = mockStorage.Object
			};

			// Act
			command.Execute();

			// Assert
			mockStorage.Verify(s => s.SaveTodos(
				It.IsAny<Guid>(),
				It.IsAny<IEnumerable<TodoItem>>()),
				Times.Never());
		}
	}
}