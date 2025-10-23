using System;


class ToolItem
{
    public string Text { get; private set; }
    public bool IsDone { get; private set; }
    public DateTime LastUpdate { get; private set; }

    public ToolItem(string text)
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

class Profile
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public int BirthYear { get; private set; }

    public Profile(string firstName, string lastName, int birthYear)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthYear = birthYear;
    }

    public string GetInfo()
    {
        int currentYear = DateTime.Now.Year;
        int age = currentYear - BirthYear;
        return $"{FirstName} {LastName}, возраст {age}";
    }
}

class TodoList
{
    private ToolItem[] tasks;
    private int taskCount;

    public TodoList(int initialCapacity = 2)
    {
        tasks = new ToolItem[initialCapacity];
        taskCount = 0;
    }

    public void Add(ToolItem item)
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

        for (int i = 0; i < taskCount; i++)
        {
            if (!showDone && tasks[i].IsDone)
                continue;

            if (showIndex || showDate || showStatus)
            {
                PrintTaskRow(i, showIndex, showStatus, showDate);
            }
            else
            {
                string cleanText = tasks[i].Text.Replace("\n", " ").Replace("\r", " ");
                string shortText = cleanText.Length > 30 ? cleanText.Substring(0, 27) + "..." : cleanText;
                Console.WriteLine(shortText);
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

    private void IncreaseArray()
    {
        int newSize = tasks.Length * 2;
        ToolItem[] newTasks = new ToolItem[newSize];

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

    public int Count => taskCount;

    public ToolItem this[int index]
    {
        get
        {
            if (index < 0 || index >= taskCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return tasks[index];
        }
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
}

struct CommandData
{
    public string Command;
    public string Argument;
    public bool MultilineFlag;
    public bool IncompleteFlag; 
    public bool StatisticsFlag;
    public bool ShowIndexFlag;
    public bool ShowStatusFlag; 
    public bool ShowDateFlag;
    public bool ShowAllFlag;
}

namespace TodoListApp
{
    internal class Program
    {
        private const int InitialTasksCapacity = 2;
        private static TodoList todoList = new TodoList(InitialTasksCapacity);
        private static Profile userProfile;

        static void Main(string[] args)
        {   
            Console.WriteLine("Работу выполнил Соловьёв Евгений и Тареев Юрий");
            userProfile = CreateUserProfile();
            if (userProfile != null)
            {
                RunTodoApplication(userProfile);
            }
            else
            {
                Console.WriteLine("Не удалось создать профиль пользователя.");
            }
        }

        static Profile CreateUserProfile()
        {
            Console.WriteLine("Введите Имя");
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Ошибка: Имя не может быть пустым");
                return null;
            }

            Console.WriteLine("Введите Фамилию");
            string surname = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(surname))
            {
                Console.WriteLine("Ошибка: Фамилия не может быть пустой");
                return null;
            }

            Console.WriteLine("Введите Год Рождения");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Ошибка: Год рождения не может быть пустым");
                return null;
            }

            if (!int.TryParse(input, out int birthYear))
            {
                Console.WriteLine("Ошибка: Неверный формат года рождения");
                return null;
            }

            var profile = new Profile(name, surname, birthYear);
            Console.WriteLine($"Добавлен пользователь {profile.GetInfo()}");

            return profile;
        }

        static void RunTodoApplication(Profile user)
        {
            bool isRunning = true;

            Console.WriteLine("Введите команду help для просмотра всех команд.");

            while (isRunning)
            {
                CommandData commandData = ParseUserInput(Console.ReadLine());

                switch (commandData.Command)
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "profile":
                        ShowUserProfile(user);
                        break;
                    case "add":
                        AddTask(commandData);
                        break;
                    case "view":
                        ShowTasks(commandData);
                        break;
                    case "done":
                        MarkTaskAsDone(commandData.Argument);
                        break;
                    case "delete":
                        DeleteTask(commandData.Argument);
                        break;
                    case "update":
                        UpdateTask(commandData.Argument);
                        break;
                    case "read":
                        ReadTask(commandData.Argument);
                        break;
                    case "exit":
                        isRunning = false;
                        Console.WriteLine("Программа завершена.");
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда. Введите команду help для просмотра доступных команд");
                        break;
                }
            }
        }

        static CommandData ParseUserInput(string userInput)
        {
            var result = new CommandData();

            if (string.IsNullOrEmpty(userInput))
                return result;

            string[] parts = userInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return result;

            result.Command = parts[0];

            string argument = "";

            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i] == null)
                    continue;

                if (parts[i].StartsWith("--"))
                {
                    string flagName = parts[i].Substring(2);
                    switch (flagName)
                    {
                        case "multiline": result.MultilineFlag = true; break;
                        case "index": result.ShowIndexFlag = true; break;
                        case "status": result.ShowStatusFlag = true; break;
                        case "update-date": result.ShowDateFlag = true; break;
                        case "all": result.ShowAllFlag = true; break;
                        case "incomplete": result.IncompleteFlag = true; break;
                        case "statistics": result.StatisticsFlag = true; break;
                    }
                }
                else if (parts[i].StartsWith("-") && parts[i].Length > 1)
                {
                    string shortFlags = parts[i].Substring(1);
                    foreach (char flagChar in shortFlags)
                    {
                        switch (flagChar)
                        {
                            case 'm': result.MultilineFlag = true; break;
                            case 'i': result.ShowIndexFlag = true; break;
                            case 's': result.ShowStatusFlag = true; break;
                            case 'd': result.ShowDateFlag = true; break;
                            case 'a': result.ShowAllFlag = true; break;
                            case 'I': result.IncompleteFlag = true; break;
                            case 'S': result.StatisticsFlag = true; break;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(argument))
                        argument = parts[i];
                    else
                        argument += " " + parts[i];
                }
            }

            if (result.ShowAllFlag)
            {
                result.ShowIndexFlag = true;
                result.ShowStatusFlag = true;
                result.ShowDateFlag = true;
            }

            result.Argument = argument;
            return result;
        }

        static void ShowHelp()
        {
            string helpText = @"
Доступные команды:
profile - выводит данные о пользователе
add - добавляет новую задачу
view - выводит все задачи из массива
read <idx> - показывает полную информацию о задаче
done <idx> - отмечает задачу выполненной
delete <idx> - удаляет задачу по индексу
update <idx> ""new_text"" - обновляет текст задачи
exit - завершает цикл и останавливает выполнение программы

Флаги для add:
--multiline или -m - многострочный ввод для add

Флаги для view:
--index или -i - показывать индекс задачи
--status или -s - показывать статус задачи
--update-date или -d - показывать дату изменения
--all или -a - показывать все данные
--incomplete или -I - показывать только невыполненные
--statistics или -S - показывать статистику

Примеры: view -isd, view --all, view -i --status";

            Console.WriteLine(helpText);
        }

        static void ShowUserProfile(Profile user)
        {
            if (user == null)
            {
                Console.WriteLine("Профиль пользователя не создан.");
                return;
            }

            string profile = @$"
Данные пользователя:
{user.GetInfo()}";

            Console.WriteLine(profile);
        }

        static void AddTask(CommandData commandData)
        {
            string taskDescription;

            if (commandData.MultilineFlag)
            {
                Console.WriteLine("Многострочный режим. Вводите текст задачи построчно. Для завершения введите !end");
                taskDescription = ReadMultilineInput();
            }
            else
            {
                taskDescription = commandData.Argument;
            }

            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                Console.WriteLine("Ошибка: Укажите задачу. Пример: add Новая задача");
                return;
            }

            string newTask = taskDescription.Trim();

            if (string.IsNullOrWhiteSpace(newTask))
            {
                Console.WriteLine("Ошибка: Задача не может быть пустой");
                return;
            }
            
            todoList.Add(new ToolItem(newTask));
            Console.WriteLine("Задача добавлена!");
        }

        static string ReadMultilineInput()
        {
            string result = "";
            string line;

            Console.Write("> ");

            while ((line = Console.ReadLine()) != "!end")
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (!string.IsNullOrEmpty(result))
                        result += "\n";
                    result += line;
                }
                Console.Write("> ");
            }

            return result;
        }

        static void ShowTasks(CommandData commandData)
        {
            bool showIncompleteOnly = commandData.IncompleteFlag;
            bool showStatistics = commandData.StatisticsFlag;

            if (todoList.Count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            bool showIndex = commandData.ShowIndexFlag;
            bool showStatus = commandData.ShowStatusFlag;
            bool showDate = commandData.ShowDateFlag;

            if (commandData.ShowAllFlag)
            {
                showIndex = true;
                showStatus = true;
                showDate = true;
            }

            bool showOnlyText = !showIndex && !showStatus && !showDate;

            if (showOnlyText)
            {
                for (int i = 0; i < todoList.Count; i++)
                {
                    if (showIncompleteOnly && todoList[i].IsDone)
                        continue;

                    string cleanText = todoList[i].Text.Replace("\n", " ").Replace("\r", " ");
                    string shortText = cleanText.Length > 30 ? cleanText.Substring(0, 27) + "..." : cleanText;
                    Console.WriteLine(shortText);
                }
            }
            else
            {
                bool showDone = !showIncompleteOnly;
                todoList.View(showIndex, showDone, showDate, showStatus);
            }

            if (showStatistics)
            {
                int completedCount = todoList.GetCompletedCount();
                int totalCount = todoList.Count;
                int displayedCount = showIncompleteOnly ? totalCount - completedCount : totalCount;

                Console.WriteLine("\n=== Статистика ===");
                Console.WriteLine($"Всего задач: {totalCount}");
                Console.WriteLine($"Выполнено: {completedCount}");
                Console.WriteLine($"Не выполнено: {totalCount - completedCount}");
                Console.WriteLine($"Показано: {displayedCount}");

                if (totalCount > 0)
                {
                    double completionRate = (double)completedCount / totalCount * 100;
                    Console.WriteLine($"Процент выполнения: {completionRate:F1}%");
                }
            }
        }

        static void ReadTask(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                Console.WriteLine("Ошибка: Укажите индекс задачи. Пример: read 1");
                return;
            }

            if (!int.TryParse(argument, out int index) || index <= 0 || index > todoList.Count)
            {
                Console.WriteLine("Ошибка: Неверный индекс задачи. Пример: read 1");
                return;
            }

            todoList.Read(index - 1);
        }

        static void MarkTaskAsDone(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                Console.WriteLine("Ошибка: Укажите индекс задачи. Пример: done 1");
                return;
            }

            if (int.TryParse(argument, out int index) && index > 0 && index <= todoList.Count)
            {
                todoList.MarkAsDone(index - 1);
                Console.WriteLine($"Задача {index} отмечена как выполненная");
            }
            else
            {
                Console.WriteLine("Ошибка: Неверный индекс задачи. Пример: done 1");
            }
        }

        static void DeleteTask(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                Console.WriteLine("Ошибка: Укажите индекс задачи. Пример: delete 1");
                return;
            }

            if (int.TryParse(argument, out int index) && index > 0 && index <= todoList.Count)
            {
                todoList.Delete(index - 1);
                Console.WriteLine($"Задача {index} удалена");
            }
            else
            {
                Console.WriteLine("Ошибка: Неверный индекс задачи. Пример: delete 1");
            }
        }

        static void UpdateTask(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                Console.WriteLine("Ошибка: Укажите индекс и новый текст задачи. Пример: update 1 \"Новый текст\"");
                return;
            }

            if (!argument.Contains(" "))
            {
                Console.WriteLine("Ошибка: Неверный формат команды. Пример: update 1 \"Новый текст\"");
                return;
            }

            int firstSpaceIndex = argument.IndexOf(' ');
            string indexStr = argument.Substring(0, firstSpaceIndex);
            string newText = argument.Substring(firstSpaceIndex + 1).Trim();

            if (!int.TryParse(indexStr, out int index) || index <= 0 || index > todoList.Count)
            {
                Console.WriteLine("Ошибка: Неверный индекс задачи. Пример: update 1 \"Новый текст\"");
                return;
            }

            if (newText.StartsWith("\"") && newText.EndsWith("\""))
            {
                newText = newText.Substring(1, newText.Length - 2);
            }

            if (string.IsNullOrWhiteSpace(newText))
            {
                Console.WriteLine("Ошибка: Новый текст задачи не может быть пустым");
                return;
            }

            todoList.UpdateText(index - 1, newText);
            Console.WriteLine($"Задача {index} обновлена");
        }
    }
}