using Xunit;
using System;
using System.Collections.Generic;

namespace ProjectTests
{
	public class TodoListTests
	{
		[Fact]
		public void Add_ShouldIncreaseCount_AndTriggerEvent()
		{
			var list = new TodoList();
			var item = new TodoItem("Test");
			bool eventTriggered = false;

			list.OnTodoAdded += (addedItem) => {
				eventTriggered = true;
				Assert.Equal(item, addedItem);
			};

			list.Add(item);

			Assert.Equal(1, list.Count);
			Assert.True(eventTriggered);
		}

		[Fact]
		public void Delete_ShouldRemoveItem_AndTriggerEvent()
		{
			var item = new TodoItem("Test");
			var list = new TodoList(new List<TodoItem> { item });
			bool eventTriggered = false;

			list.OnTodoDeleted += (deletedItem) => {
				eventTriggered = true;
			};

			list.Delete(0);

			Assert.Equal(0, list.Count);
			Assert.True(eventTriggered);
		}

		[Fact]
		public void Delete_InvalidIndex_ShouldThrowException()
		{
			var list = new TodoList();

			Assert.Throws<ArgumentOutOfRangeException>(() => list.Delete(0));
		}

		[Fact]
		public void Insert_ShouldPlaceItemAtCorrectIndex()
		{
			var item1 = new TodoItem("1");
			var item2 = new TodoItem("2");
			var item3 = new TodoItem("3");
			var list = new TodoList(new List<TodoItem> { item1, item2 });

			list.Insert(1, item3);

			Assert.Equal(3, list.Count);
			Assert.Equal(item1, list[0]);
			Assert.Equal(item3, list[1]);
			Assert.Equal(item2, list[2]);
		}

		[Fact]
		public void GetCompletedCount_ShouldReturnCorrectNumber()
		{
			var list = new TodoList();
			var t1 = new TodoItem("1");
			var t2 = new TodoItem("2");
			t2.UpdateStatus(TodoStatus.Completed);
			var t3 = new TodoItem("3");
			t3.UpdateStatus(TodoStatus.Completed);

			list.Add(t1);
			list.Add(t2);
			list.Add(t3);

			int completed = list.GetCompletedCount();

			Assert.Equal(2, completed);
		}

		[Fact]
		public void SetStatus_ShouldUpdateItemStatus()
		{
			var item = new TodoItem("Test");
			var list = new TodoList(new List<TodoItem> { item });

			list.SetStatus(0, TodoStatus.Failed);

			Assert.Equal(TodoStatus.Failed, list[0].Status);
		}
	}
}