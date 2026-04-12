using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoList.Models;

public class TodoItem
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string Text { get; set; } = string.Empty;

	public TodoStatus Status { get; set; }

	public DateTime LastUpdate { get; set; }

	public int ProfileId { get; set; }

	[ForeignKey("ProfileId")]
	public Profile? Profile { get; set; }

	public TodoItem()
	{
	}

	public TodoItem(string text)
	{
		Text = text;
		Status = TodoStatus.NotStarted;
		LastUpdate = DateTime.Now;
	}

	public TodoItem(string text, TodoStatus status, DateTime lastUpdate)
	{
		Text = text;
		Status = status;
		LastUpdate = lastUpdate;
	}


	public void UpdateStatus(TodoStatus newStatus)
	{
		Status = newStatus;
		LastUpdate = DateTime.Now;
	}

	public void UpdateText(string newText)
	{
		Text = newText;
		LastUpdate = DateTime.Now;
	}

	public string GetShortInfo()
	{
		string cleanText = Text.Replace("\n", " ").Replace("\r", " ");
		string shortText = cleanText.Length > 30 ? cleanText.Substring(0, 27) + "..." : cleanText;

		string statusText = Status switch
		{
			TodoStatus.NotStarted => "Не начато",
			TodoStatus.InProgress => "В процессе",
			TodoStatus.Completed => "Выполнено",
			TodoStatus.Postponed => "Отложено",
			TodoStatus.Failed => "Провалено",
			_ => Status.ToString()
		};

		return $"{shortText.PadRight(33)} | {statusText.PadRight(12)} | {LastUpdate:dd.MM.yyyy HH:mm}";
	}

	public string GetFullInfo()
	{
		return $"Текст: {Text}\nСтатус: {Status}\nДата последнего изменения: {LastUpdate:dd.MM.yyyy HH:mm}";
	}
}