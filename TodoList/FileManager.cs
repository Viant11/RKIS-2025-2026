using System;
using System.Globalization;
using System.IO;
using System.Text;

public static class FileManager
{
    public static void EnsureDataDirectory(string dirPath)
    {
        try
        {
            if (!Directory.Exists(dirPath))
            {
                Console.WriteLine($"Создаем папку для данных: {dirPath}");
                Directory.CreateDirectory(dirPath);
                Console.WriteLine($"Папка создана: {dirPath}");
            }
            else
            {
                Console.WriteLine($"Папка уже существует: {dirPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании папки {dirPath}: {ex.Message}");
            throw;
        }
    }

    public static void SaveProfile(Profile profile, string filePath)
    {
        try
        {
            Console.WriteLine($"Сохранение профиля в: {filePath}");

            string[] lines = {
                profile.FirstName,
                profile.LastName,
                profile.BirthYear.ToString()
            };

            File.WriteAllLines(filePath, lines, Encoding.UTF8);

            Console.WriteLine($"Профиль сохранен: {profile.FirstName} {profile.LastName}");
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
            {
                Console.WriteLine($"Файл профиля не найден: {filePath}");
                return null;
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            if (lines.Length >= 3)
            {
                string firstName = lines[0];
                string lastName = lines[1];
                string birthYearStr = lines[2];

                Console.WriteLine($"Загружаем из файла: Имя='{firstName}', Фамилия='{lastName}', Год='{birthYearStr}'");

                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                {
                    Console.WriteLine("Ошибка: Имя или фамилия пустые в файле");
                    return null;
                }

                if (int.TryParse(birthYearStr, out int birthYear))
                {
                    var profile = new Profile(firstName.Trim(), lastName.Trim(), birthYear);
                    Console.WriteLine($"Успешно загружен профиль: {profile.GetInfo()}");
                    return profile;
                }
                else
                {
                    Console.WriteLine($"Ошибка: неверный формат года рождения: '{birthYearStr}'");
                }
            }
            else
            {
                Console.WriteLine($"Ошибка: файл профиля поврежден или имеет неверный формат.");
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
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter writer = new StreamWriter(filePath, false, new UTF8Encoding(false)))
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

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8, true))
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