using Xunit;
using Moq;
using System;
using TodoList;
using TodoList.Models;

namespace ProjectTests
{
	public class TodoItemTests
	{
		[Fact]
		public void Constructor_NewTask_SetsCorrectLastUpdate()
		{
			// Arrange
			var fixedTime = new DateTime(2026, 4, 12, 10, 0, 0);
			var mockClock = new Mock<IClock>();
			mockClock.Setup(c => c.Now).Returns(fixedTime);

			// Act
			var item = new TodoItem("Test Task", mockClock.Object);

			// Assert
			Assert.Equal(fixedTime, item.LastUpdate);
		}

		[Fact]
		public void UpdateStatus_StatusChanged_UpdatesLastUpdateToCurrentTime()
		{
			// Arrange
			var startTime = new DateTime(2026, 4, 12, 10, 0, 0);
			var updateTime = new DateTime(2026, 4, 12, 11, 0, 0);

			var mockClock = new Mock<IClock>();
			mockClock.SetupSequence(c => c.Now)
				.Returns(startTime)
				.Returns(updateTime);

			var item = new TodoItem("Test Task", mockClock.Object);

			// Act
			item.UpdateStatus(TodoStatus.Completed);

			// Assert
			Assert.Equal(updateTime, item.LastUpdate);
		}

		[Fact]
		public void UpdateText_TextChanged_UpdatesLastUpdateToCurrentTime()
		{
			// Arrange
			var startTime = new DateTime(2026, 4, 12, 10, 0, 0);
			var updateTime = new DateTime(2026, 4, 12, 10, 30, 0);

			var mockClock = new Mock<IClock>();
			mockClock.SetupSequence(c => c.Now)
				.Returns(startTime)
				.Returns(updateTime);

			var item = new TodoItem("Old Text", mockClock.Object);

			// Act
			item.UpdateText("New Text");

			// Assert
			Assert.Equal(updateTime, item.LastUpdate);
		}

		[Fact]
		public void GetShortInfo_LongText_ReturnsTruncatedString()
		{
			// Arrange
			string longText = "Это очень длинная задача, которая должна быть обрезана в выводе";
			var item = new TodoItem(longText);

			// Act
			string result = item.GetShortInfo();

			// Assert
			Assert.Contains("...", result);
		}
	}
}