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
			// Arrange
			var list = new TodoList();
			var item = new TodoItem("Test");
			bool eventTriggered = false;

			list.OnTodoAdded += (addedItem) => {
				eventTriggered = true;
				Assert.Equal(item, addedItem);
			};

			// Act
			list.Add(item);

			// Assert
			Assert.Equal(1, list.Count);
			Assert.True(eventTriggered);
		}

		[Fact]
		public void Delete_ShouldRemoveItem_AndTriggerEvent()
		{
			// Arrange
			var item = new TodoItem("Test");
			var list = new TodoList(new List<TodoItem> { item });
			bool eventTriggered = false;

			list.OnTodoDeleted += (deletedItem) => {
				eventTriggered = true;
			};

			// Act
			list.Delete(0);

			// Assert
			Assert.Equal(0, list.Count);
			Assert.True(eventTriggered);
		}

		[Fact]
		public void Delete_InvalidIndex_ShouldThrowException()
		{
			// Arrange
			var list = new TodoList();

			// Act & Assert
			Assert.Throws<ArgumentOutOfRangeException>(() => list.Delete(0));
		}

		[Fact]
		public void Insert_ShouldPlaceItemAtCorrectIndex()
		{
			// Arrange
			var item1 = new TodoItem("1");
			var item2 = new TodoItem("2");
			var item3 = new TodoItem("3");
			var list = new TodoList(new List<TodoItem> { item1, item2 });

			// Act
			list.Insert(1, item3);

			// Assert
			Assert.Equal(3, list.Count);
			Assert.Equal(item1, list[0]);
			Assert.Equal(item3, list[1]);
			Assert.Equal(item2, list[2]);
		}

		[Fact]
		public void GetCompletedCount_ShouldReturnCorrectNumber()
		{
			// Arrange
			var list = new TodoList();
			var t1 = new TodoItem("1");
			var t2 = new TodoItem("2");
			t2.UpdateStatus(TodoStatus.Completed);
			var t3 = new TodoItem("3");
			t3.UpdateStatus(TodoStatus.Completed);

			list.Add(t1);
			list.Add(t2);
			list.Add(t3);

			// Act
			int completed = list.GetCompletedCount();

			// Assert
			Assert.Equal(2, completed);
		}

		[Fact]
		public void SetStatus_ShouldUpdateItemStatus()
		{
			// Arrange
			var item = new TodoItem("Test");
			var list = new TodoList(new List<TodoItem> { item });

			// Act
			list.SetStatus(0, TodoStatus.Failed);

			// Assert
			Assert.Equal(TodoStatus.Failed, list[0].Status);
		}
	}
}