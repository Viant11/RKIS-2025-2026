using System;

public class TodoItem
{
    public string Text { get; private set; }
    public bool IsDone { get; private set; }
    public DateTime LastUpdate { get; private set; }

    public TodoItem(string text)
    {
        Text = text;
        IsDone = false;
        LastUpdate = DateTime.Now;
    }

    public void MarkDone()
    {
        IsDone = true;
        LastUpdate = DateTime.Now;
    }

    public void UpdateText(string newText)
    {
        Text = newText;
        LastUpdate = DateTime.Now;
    }

    public void SetLastUpdate(DateTime dateTime)
    {
        LastUpdate = dateTime;
    }

    public string GetShortInfo()
    {
        string cleanText = Text.Replace("\n", " ").Replace("\r", " ");
        string shortText = cleanText.Length > 30 ? cleanText.Substring(0, 27) + "..." : cleanText;
        string status = IsDone ? "Выполнена" : "Не выполнена";
        return $"{shortText.PadRight(33)} | {status.PadRight(12)} | {LastUpdate:dd.MM.yyyy HH:mm}";
    }

    public string GetFullInfo()
    {
        return $"Текст: {Text}\nСтатус: {(IsDone ? "Выполнена" : "Не выполнена")}\nДата последнего изменения: {LastUpdate:dd.MM.yyyy HH:mm}";
    }
}