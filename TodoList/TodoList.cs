using System;
using System.Collections;
using System.Collections.Generic;

public class TodoList : IEnumerable<TodoItem>
{
	public event Action<TodoItem> OnTodoAdded;
	public event Action<TodoItem> OnTodoDeleted;
	public event Action<TodoItem> OnTodoUpdated;
	public event Action<TodoItem> OnStatusChanged;

	private List<TodoItem> tasks;

	public int Count => tasks.Count;

	public TodoItem this[int index]
	{
		get
		{
			if (index < 0 || index >= tasks.Count)
				throw new ArgumentOutOfRangeException(nameof(index));
			return tasks[index];
		}
	}

	public TodoList()
	{
		tasks = new List<TodoItem>();
	}

	public TodoList(List<TodoItem> existingTasks)
	{
		tasks = existingTasks;
	}

	public void Add(TodoItem item)
	{
		tasks.Add(item);
		OnTodoAdded?.Invoke(item);
	}

	public void Delete(int index)
	{
		if (index < 0 || index >= tasks.Count)
			throw new ArgumentOutOfRangeException(nameof(index));

		var item = tasks[index];
		tasks.RemoveAt(index);
		OnTodoDeleted?.Invoke(item);
	}

	public void Insert(int index, TodoItem item)
	{
		if (index < 0 || index > tasks.Count)
			throw new ArgumentOutOfRangeException(nameof(index), "Индекс для вставки вне допустимого диапазона.");
		tasks.Insert(index, item);
		OnTodoAdded?.Invoke(item);
	}

	public void View(bool showIndex, bool showDone, bool showDate, bool showStatus = true)
	{
		if (tasks.Count == 0)
		{
			Console.WriteLine("Список задач пуст.");
			return;
		}

		if (showIndex || showDate || showStatus)
		{
			PrintTableHeader(showIndex, showStatus, showDate);
		}

		for (int i = 0; i < tasks.Count; i++)
		{
			if (!showDone && tasks[i].Status == TodoStatus.Completed)
				continue;

			if (showIndex || showDate || showStatus)
			{
				PrintTaskRow(i, showIndex, showStatus, showDate);
			}
			else
			{
				Console.WriteLine(tasks[i].GetShortInfo());
			}
		}
	}

	public void Read(int index)
	{
		if (index < 0 || index >= tasks.Count)
			throw new ArgumentOutOfRangeException(nameof(index));

		Console.WriteLine("=== Полная информация о задаче ===");
		Console.WriteLine($"Индекс: #{index + 1}");
		Console.WriteLine(tasks[index].GetFullInfo());
		Console.WriteLine(new string('=', 40));
	}

	public void SetStatus(int index, TodoStatus status)
	{
		if (index < 0 || index >= tasks.Count)
			throw new ArgumentOutOfRangeException(nameof(index));
		tasks[index].UpdateStatus(status);
		OnStatusChanged?.Invoke(tasks[index]);
	}

	public void UpdateText(int index, string newText)
	{
		if (index < 0 || index >= tasks.Count)
			throw new ArgumentOutOfRangeException(nameof(index));
		tasks[index].UpdateText(newText);
		OnTodoUpdated?.Invoke(tasks[index]);
	}

	public int GetCompletedCount()
	{
		int count = 0;
		foreach (var task in tasks)
		{
			if (task.Status == TodoStatus.Completed)
				count++;
		}
		return count;
	}

	private void PrintTableHeader(bool showIndex, bool showStatus, bool showDate)
	{
		string header = "";
		if (showIndex) header += "Индекс".PadRight(8) + " | ";
		header += "Текст задачи".PadRight(33) + " | ";
		if (showStatus) header += "Статус".PadRight(12);
		if (showDate) header += (showStatus ? " | " : "") + "Дата изменения";
		Console.WriteLine(header);
		Console.WriteLine(new string('-', header.Length));
	}

	private void PrintTaskRow(int index, bool showIndex, bool showStatus, bool showDate)
	{
		var task = tasks[index];
		string[] shortInfoParts = task.GetShortInfo().Split(new[] { " | " }, StringSplitOptions.None);

		string row = "";
		if (showIndex) row += $"#{index + 1}".PadRight(8) + " | ";
		row += shortInfoParts[0].PadRight(33) + " | ";
		if (showStatus) row += shortInfoParts[1].PadRight(12);
		if (showDate) row += (showStatus ? " | " : "") + shortInfoParts[2];

		Console.WriteLine(row);
	}

	public IEnumerator<TodoItem> GetEnumerator()
	{
		foreach (var task in tasks)
		{
			yield return task;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}