using Xunit;
using System;

namespace TodoList.Tests
{
	public class TodoItemTests
	{
		[Fact]
		public void Constructor_ShouldSetDefaultValues()
		{
			string expectedText = "Купить хлеб";

			var item = new TodoItem(expectedText);

			Assert.Equal(expectedText, item.Text);
			Assert.Equal(TodoStatus.NotStarted, item.Status);
			Assert.True(item.LastUpdate <= DateTime.Now);
		}

		[Fact]
		public void UpdateStatus_ShouldChangeStatusAndDate()
		{
			var item = new TodoItem("Test task");
			var oldDate = item.LastUpdate;

			System.Threading.Thread.Sleep(10);

			item.UpdateStatus(TodoStatus.InProgress);

			Assert.Equal(TodoStatus.InProgress, item.Status);
			Assert.NotEqual(oldDate, item.LastUpdate);
		}
	}
}