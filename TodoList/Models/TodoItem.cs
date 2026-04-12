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

	[NotMapped]
	private readonly IClock _clock;

	public TodoItem() : this(string.Empty) { }

	public TodoItem(string text, IClock? clock = null)
	{
		_clock = clock ?? new SystemClock();

		Text = text;
		Status = TodoStatus.NotStarted;
		LastUpdate = _clock.Now;
	}

	public TodoItem(string text, TodoStatus status, DateTime lastUpdate, IClock? clock = null)
	{
		_clock = clock ?? new SystemClock();
		Text = text;
		Status = status;
		LastUpdate = lastUpdate;
	}

	public void UpdateStatus(TodoStatus newStatus)
	{
		Status = newStatus;
		LastUpdate = _clock.Now;
	}

	public void UpdateText(string newText)
	{
		Text = newText;
		LastUpdate = _clock.Now;
	}

	public string GetShortInfo()
	{
		string cleanText = Text.Replace("\n", " ").Replace("\r", " ");
		string shortText = cleanText.Length > 30 ? cleanText.Substring(0, 27) + "..." : cleanText;
		return $"{shortText.PadRight(33)} | {Status.ToString().PadRight(12)} | {LastUpdate:dd.MM.yyyy HH:mm}";
	}

	public string GetFullInfo()
	{
		return $"Текст: {Text}\nСтатус: {Status}\nДата последнего изменения: {LastUpdate:dd.MM.yyyy HH:mm}";
	}
}