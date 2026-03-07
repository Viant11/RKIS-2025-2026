using Xunit;
using System;

namespace ProjectTests
{
	public class TodoItemTests
	{
		[Fact]
		public void GetShortInfo_ShouldTruncateLongText()
		{
			string longText = "Это очень очень очень очень очень очень длинная задача";
			var item = new TodoItem(longText);

			string shortInfo = item.GetShortInfo();

			Assert.Contains("Это очень очень очень очень...", shortInfo);
		}

		[Fact]
		public void GetShortInfo_ShouldReplaceNewLines()
		{
			string multilineText = "Строка1\nСтрока2";
			var item = new TodoItem(multilineText);

			string shortInfo = item.GetShortInfo();

			Assert.DoesNotContain("\n", shortInfo);
			Assert.Contains("Строка1 Строка2", shortInfo);
		}

		[Fact]
		public void UpdateText_ShouldUpdateTextAndTimestamp()
		{
			var item = new TodoItem("Old Text");
			DateTime oldTime = item.LastUpdate;

			System.Threading.Thread.Sleep(50);

			item.UpdateText("New Text");

			Assert.Equal("New Text", item.Text);
			Assert.True(item.LastUpdate > oldTime);
		}

		[Fact]
		public void GetFullInfo_ShouldContainStatusText()
		{
			var item = new TodoItem("Task");
			item.UpdateStatus(TodoStatus.Completed);

			string fullInfo = item.GetFullInfo();

			Assert.Contains("Выполнено", fullInfo);
			Assert.Contains("Task", fullInfo);
		}
	}
}