using System;

public class TodoItem
{
	public string Text { get; private set; }
	public TodoStatus Status { get; private set; }
	public DateTime LastUpdate { get; private set; }

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

	private string GetStatusText()
	{
		switch (Status)
		{
			case TodoStatus.NotStarted: return "Не начато";
			case TodoStatus.InProgress: return "В процессе";
			case TodoStatus.Completed: return "Выполнено";
			case TodoStatus.Postponed: return "Отложено";
			case TodoStatus.Failed: return "Провалено";
			default: return "Неизвестно";
		}
	}

	public string GetShortInfo()
	{
		string cleanText = Text.Replace("\n", " ").Replace("\r", " ");
		string shortText = cleanText.Length > 30 ? cleanText.Substring(0, 27) + "..." : cleanText;
		string status = GetStatusText();
		return $"{shortText.PadRight(33)} | {status.PadRight(12)} | {LastUpdate:dd.MM.yyyy HH:mm}";
	}

	public string GetFullInfo()
	{
		return $"Текст: {Text}\nСтатус: {GetStatusText()}\nДата последнего изменения: {LastUpdate:dd.MM.yyyy HH:mm}";
	}
}