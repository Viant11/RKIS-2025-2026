using Xunit;
using System;

namespace ProjectTests
{
	public class TodoItemTests
	{
		[Fact]
		public void GetShortInfo_ShouldTruncateLongText()
		{
			// Arrange
			string longText = "Это очень очень очень очень очень очень длинная задача";
			var item = new TodoItem(longText);

			// Act
			string shortInfo = item.GetShortInfo();

			// Assert
			Assert.Contains("Это очень очень очень очень...", shortInfo);
		}

		[Fact]
		public void GetShortInfo_ShouldReplaceNewLines()
		{
			// Arrange
			string multilineText = "Строка1\nСтрока2";
			var item = new TodoItem(multilineText);

			// Act
			string shortInfo = item.GetShortInfo();

			// Assert
			Assert.DoesNotContain("\n", shortInfo);
			Assert.Contains("Строка1 Строка2", shortInfo);
		}

		[Fact]
		public void UpdateText_ShouldUpdateTextAndTimestamp()
		{
			// Arrange
			var item = new TodoItem("Old Text");
			DateTime oldTime = item.LastUpdate;

			System.Threading.Thread.Sleep(50);

			// Act
			item.UpdateText("New Text");

			// Assert
			Assert.Equal("New Text", item.Text);
			Assert.True(item.LastUpdate > oldTime);
		}

		[Fact]
		public void GetFullInfo_ShouldContainStatusText()
		{
			// Arrange
			var item = new TodoItem("Task");
			item.UpdateStatus(TodoStatus.Completed);

			// Act
			string fullInfo = item.GetFullInfo();

			// Assert
			Assert.Contains("Выполнено", fullInfo);
			Assert.Contains("Task", fullInfo);
		}
	}
}