using System;

class TodoItem
{
    private string text;
    private bool isDone;
    private DateTime lastUpdate;

    public string Text
    {
        get => text;
        private set => text = value;
    }

    public bool IsDone
    {
        get => isDone;
        private set => isDone = value;
    }

    public DateTime LastUpdate
    {
        get => lastUpdate;
        private set => lastUpdate = value;
    }

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