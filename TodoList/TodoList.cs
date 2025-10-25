using System;

class TodoList
{
    private TodoItem[] tasks;
    private int taskCount;

    public int Count => taskCount;

    public TodoList(int initialCapacity = 2)
    {
        tasks = new TodoItem[initialCapacity];
        taskCount = 0;
    }

    public void Add(TodoItem item)
    {
        if (taskCount >= tasks.Length)
        {
            IncreaseArray();
        }
        tasks[taskCount] = item;
        taskCount++;
    }

    public void Delete(int index)
    {
        if (index < 0 || index >= taskCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        for (int i = index; i < taskCount - 1; i++)
        {
            tasks[i] = tasks[i + 1];
        }
        taskCount--;
    }

    public void View(bool showIndex, bool showDone, bool showDate, bool showStatus = true)
    {
        if (taskCount == 0)
        {
            Console.WriteLine("Список задач пуст.");
            return;
        }

        if (showIndex || showDate || showStatus)
        {
            PrintTableHeader(showIndex, showStatus, showDate);
        }

        for (int i = 0; i < _taskCount; i++)
        {
            if (!showDone && _tasks[i].IsDone)
                continue;

            if (showIndex || showDate || showStatus)
            {
                PrintTaskRow(i, showIndex, showStatus, showDate);
            }
            else
            {
                Console.WriteLine(_tasks[i].GetShortInfo());
            }
        }
    }

    public void Read(int index)
    {
        if (index < 0 || index >= taskCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        Console.WriteLine("=== Полная информация о задаче ===");
        Console.WriteLine($"Индекс: #{index + 1}");
        Console.WriteLine(tasks[index].GetFullInfo());
        Console.WriteLine(new string('=', 40));
    }

    public TodoItem GetTask(int index)
    {
        if (index < 0 || index >= taskCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return tasks[index];
    }

    public void MarkAsDone(int index)
    {
        if (index < 0 || index >= taskCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        tasks[index].MarkDone();
    }

    public void UpdateText(int index, string newText)
    {
        if (index < 0 || index >= taskCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        tasks[index].UpdateText(newText);
    }

    public int GetCompletedCount()
    {
        int count = 0;
        for (int i = 0; i < taskCount; i++)
        {
            if (tasks[i].IsDone)
                count++;
        }
        return count;
    }

    private void IncreaseArray()
    {
        int newSize = tasks.Length * 2;
        TodoItem[] newTasks = new TodoItem[newSize];

        for (int i = 0; i < taskCount; i++)
        {
            newTasks[i] = tasks[i];
        }

        tasks = newTasks;
    }

    private void PrintTableHeader(bool showIndex, bool showStatus, bool showDate)
    {
        string header = "";

        if (showIndex)
            header += "Индекс".PadRight(8) + " | ";

        header += "Текст задачи".PadRight(33) + " | ";

        if (showStatus)
            header += "Статус".PadRight(12);

        if (showDate)
            header += (showStatus ? " | " : "") + "Дата изменения";

        Console.WriteLine(header);
        Console.WriteLine(new string('-', header.Length));
    }

    private void PrintTaskRow(int index, bool showIndex, bool showStatus, bool showDate)
    {
        string row = "";

        if (showIndex)
            row += $"#{index + 1}".PadRight(8) + " | ";

        string cleanText = tasks[index].Text.Replace("\n", " ").Replace("\r", " ");
        string shortText = cleanText.Length > 30 ? cleanText.Substring(0, 27) + "..." : cleanText;
        string status = tasks[index].IsDone ? "Выполнена" : "Не выполнена";

        row += shortText.PadRight(33) + " | ";

        if (showStatus)
            row += status.PadRight(12);

        if (showDate)
            row += (showStatus ? " | " : "") + tasks[index].LastUpdate.ToString("dd.MM.yyyy HH:mm");

        Console.WriteLine(row);
    }
}