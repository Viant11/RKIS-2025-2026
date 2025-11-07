using System;
using System.IO;
using System.Globalization;

public static class FileManager
{
    public static void EnsureDataDirectory(string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    public static void SaveProfile(Profile profile, string filePath)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(profile.FirstName);
                writer.WriteLine(profile.LastName);
                writer.WriteLine(profile.BirthYear);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении профиля: {ex.Message}");
        }
    }

    public static Profile? LoadProfile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return null;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string firstName = reader.ReadLine() ?? "";
                string lastName = reader.ReadLine() ?? "";
                string birthYearStr = reader.ReadLine() ?? "";

                if (int.TryParse(birthYearStr, out int birthYear))
                {
                    return new Profile(firstName, lastName, birthYear);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке профиля: {ex.Message}");
        }

        return null;
    }

    public static void SaveTodos(TodoList todos, string filePath)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Index;Text;IsDone;LastUpdate");

                for (int i = 0; i < todos.Count; i++)
                {
                    var task = todos.GetTask(i);
                    string escapedText = task.Text.Replace("\"", "\"\"")
                                                 .Replace("\n", "\\n")
                                                 .Replace("\r", "\\r");
                    string line = $"{i};\"{escapedText}\";{task.IsDone};{task.LastUpdate:O}";
                    writer.WriteLine(line);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении задач: {ex.Message}");
        }
    }

    public static TodoList LoadTodos(string filePath)
    {
        var todoList = new TodoList();

        try
        {
            if (!File.Exists(filePath))
                return todoList;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string? header = reader.ReadLine();
                if (header == null || header != "Index;Text;IsDone;LastUpdate")
                    return todoList;

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = ParseCsvLine(line);
                    if (parts.Length == 4)
                    {
                        string text = parts[1].Replace("\\n", "\n")
                                             .Replace("\\r", "\r")
                                             .Replace("\"\"", "\"");

                        if (bool.TryParse(parts[2], out bool isDone) &&
                            DateTime.TryParse(parts[3], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime lastUpdate))
                        {
                            var todoItem = new TodoItem(text);
                            if (isDone)
                                todoItem.MarkDone();

                            SetLastUpdate(todoItem, lastUpdate);
                            todoList.Add(todoItem);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
        }

        return todoList;
    }

    private static string[] ParseCsvLine(string line)
    {
        var parts = new List<string>();
        bool inQuotes = false;
        string currentPart = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ';' && !inQuotes)
            {
                parts.Add(currentPart);
                currentPart = "";
            }
            else
            {
                currentPart += c;
            }
        }

        parts.Add(currentPart);
        return parts.ToArray();
    }

    private static void SetLastUpdate(TodoItem item, DateTime lastUpdate)
    {
        var field = typeof(TodoItem).GetField("LastUpdate",
                     System.Reflection.BindingFlags.NonPublic |
                     System.Reflection.BindingFlags.Instance);
        field?.SetValue(item, lastUpdate);
    }
}