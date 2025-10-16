using System;

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

namespace TodoList
{
    internal class Program
    {
        private const int InitialTasksCapacity = 2;
        private const string DateFormat = "yyyy";
        private static string[] tasks = new string[InitialTasksCapacity];
        private static bool[] taskStatuses = new bool[InitialTasksCapacity];
        private static DateTime[] taskDates = new DateTime[InitialTasksCapacity];
        private static int taskCount = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнил Соловьёв Евгений и Тареев Юрий");

            var user = CreateUserProfile();
            RunTodoApplication(user);
        }

        static (string Name, string Surname, int Age) CreateUserProfile()
        {
            Console.WriteLine("Введите Имя");
            string name = Console.ReadLine();

            Console.WriteLine("Введите Фамилию");
            string surname = Console.ReadLine();

            Console.WriteLine("Введите Год Рождения");
            int birthYear = int.Parse(Console.ReadLine());

            int currentYear = int.Parse(DateTime.Now.ToString(DateFormat));
            int age = currentYear - birthYear;

            Console.WriteLine($"Добавлен пользователь {name} {surname} возраст - {age}");

            return (name, surname, age);
        }

        static void RunTodoApplication((string Name, string Surname, int Age) user)
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
            Console.WriteLine("Доступные команды: ");
            Console.WriteLine("profile - выводит данные о пользователе");
            Console.WriteLine("add - добавляет новую задачу");
            Console.WriteLine("view - выводит все задачи из массива");
            Console.WriteLine("read <idx> - показывает полную информацию о задаче");
            Console.WriteLine("done <idx> - отмечает задачу выполненной");
            Console.WriteLine("delete <idx> - удаляет задачу по индексу");
            Console.WriteLine("update <idx> \"new_text\" - обновляет текст задачи");
            Console.WriteLine("exit - завершает цикл и останавливает выполнение программы");
            Console.WriteLine("Флаги:");
            Console.WriteLine("  --multiline или -m - многострочный ввод для add");
            Console.WriteLine("  -i - показывать только невыполненные задачи для view");
            Console.WriteLine("  -s - показывать статистику для view");
            Console.WriteLine("Флаги для view:");
            Console.WriteLine("  --index или -i - показывать индекс задачи");
            Console.WriteLine("  --status или -s - показывать статус задачи");
            Console.WriteLine("  --update-date или -d - показывать дату изменения");
            Console.WriteLine("  --all или -a - показывать все данные");
            Console.WriteLine("  --incomplete или -I - показывать только невыполненные");
            Console.WriteLine("  --statistics или -S - показывать статистику");
            Console.WriteLine("Примеры: view -isd, view --all, view -i --status");
        }

        static void ShowUserProfile((string Name, string Surname, int Age) user)
        {
            Console.WriteLine("Данные пользователя:");
            Console.WriteLine($"Имя: {user.Name}");
            Console.WriteLine($"Фамилия: {user.Surname}");
            Console.WriteLine($"Возраст: {user.Age}");
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

            if (taskCount >= tasks.Length)
            {
                ResizeAllArrays();
            }

            tasks[taskCount] = newTask;
            taskStatuses[taskCount] = false;
            taskDates[taskCount] = DateTime.Now;
            taskCount++;
            Console.WriteLine("Задача добавлена!");
        }

        static string ReadMultilineInput()
        {
            string result = "";
            string line;

            while ((line = Console.ReadLine()) != "!end")
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (!string.IsNullOrEmpty(result))
                        result += Environment.NewLine;
                    result += line;
                }
            }

            return result;
        }

        static void ResizeAllArrays()
        {
            int newSize = tasks.Length * 2;

            string[] newTasks = new string[newSize];
            bool[] newStatuses = new bool[newSize];
            DateTime[] newDates = new DateTime[newSize];

            for (int i = 0; i < taskCount; i++)
            {
                newTasks[i] = tasks[i];
                newStatuses[i] = taskStatuses[i];
                newDates[i] = taskDates[i];
            }

            tasks = newTasks;
            taskStatuses = newStatuses;
            taskDates = newDates;
        }

        static void ShowTasks(CommandData commandData)
        {
            bool showIncompleteOnly = commandData.IncompleteFlag;
            bool showStatistics = commandData.StatisticsFlag;

            if (taskCount == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            bool showIndex = commandData.ShowIndexFlag;
            bool showStatus = commandData.ShowStatusFlag;
            bool showDate = commandData.ShowDateFlag;
            bool showOnlyText = !showIndex && !showStatus && !showDate;
            int displayedCount = 0;
            int completedCount = 0;

            if (!showOnlyText)
            {
                PrintTableHeader(showIndex, showStatus, showDate);
            }

            for (int i = 0; i < taskCount; i++)
            {
                if (showIncompleteOnly && taskStatuses[i])
                    continue;

                if (showOnlyText)
                {
                    string shortText = tasks[i].Length > 30 ? tasks[i].Substring(0, 27) + "..." : tasks[i];
                    Console.WriteLine(shortText);
                }
                else
                {
                    PrintTaskRow(i, showIndex, showStatus, showDate);
                }

                displayedCount++;
                if (taskStatuses[i])
                    completedCount++;
            }

            if (showStatistics)
            {
                Console.WriteLine("\n=== Статистика ===");
                Console.WriteLine($"Всего задач: {taskCount}");
                Console.WriteLine($"Выполнено: {completedCount}");
                Console.WriteLine($"Не выполнено: {taskCount - completedCount}");
                Console.WriteLine($"Показано: {displayedCount}");

                if (taskCount > 0)
                {
                    double completionRate = (double)completedCount / taskCount * 100;
                    Console.WriteLine($"Процент выполнения: {completionRate:F1}%");
                }
            }
        }

        static void PrintTableHeader(bool showIndex, bool showStatus, bool showDate)
        {
            string header = "";

            if (showIndex)
                header += "Индекс".PadRight(8) + " | ";

            header += "Текст задачи".PadRight(33) + " | ";

            if (showStatus)
                header += "Статус".PadRight(12) + " | ";

            if (showDate)
                header += "Дата изменения";

            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));
        }

        static void PrintTaskRow(int index, bool showIndex, bool showStatus, bool showDate)
        {
            string row = "";

            if (showIndex)
                row += $"#{index + 1}".PadRight(8) + " | ";

            string taskText = tasks[index];
            string shortText = taskText.Length > 30 ? taskText.Substring(0, 27) + "..." : taskText.PadRight(30);
            row += shortText.PadRight(33) + " | ";

            if (showStatus)
            {
                string status = taskStatuses[index] ? "Выполнена" : "Не выполнена";
                row += status.PadRight(12) + " | ";
            }

            if (showDate)
                row += taskDates[index].ToString("dd.MM.yyyy HH:mm");

            Console.WriteLine(row);
        }

        static void ReadTask(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                Console.WriteLine("Ошибка: Укажите индекс задачи. Пример: read 1");
                return;
            }

            if (!int.TryParse(argument, out int index) || index <= 0 || index > taskCount)
            {
                Console.WriteLine("Ошибка: Неверный индекс задачи. Пример: read 1");
                return;
            }

            Console.WriteLine("=== Полная информация о задаче ===");
            Console.WriteLine($"Индекс: #{index}");
            Console.WriteLine($"Текст: {tasks[index - 1]}");
            Console.WriteLine($"Статус: {(taskStatuses[index - 1] ? "Выполнена" : "Не выполнена")}");
            Console.WriteLine($"Дата последнего изменения: {taskDates[index - 1]:dd.MM.yyyy HH:mm}");
            Console.WriteLine(new string('=', 40));
        }

        static void MarkTaskAsDone(string argument)
        {
            if (int.TryParse(argument, out int index) && index > 0 && index <= taskCount)
            {
                taskStatuses[index - 1] = true;
                taskDates[index - 1] = DateTime.Now;
                Console.WriteLine($"Задача {index} отмечена как выполненная");
            }
            else
            {
                Console.WriteLine("Ошибка: Неверный индекс задачи. Пример: done 1");
            }
        }

        static void DeleteTask(string argument)
        {
            if (int.TryParse(argument, out int index) && index > 0 && index <= taskCount)
            {
                for (int i = index - 1; i < taskCount - 1; i++)
                {
                    tasks[i] = tasks[i + 1];
                    taskStatuses[i] = taskStatuses[i + 1];
                    taskDates[i] = taskDates[i + 1];
                }

                taskCount--;
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

            if (!int.TryParse(indexStr, out int index) || index <= 0 || index > taskCount)
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

            tasks[index - 1] = newText;
            taskDates[index - 1] = DateTime.Now;
            Console.WriteLine($"Задача {index} обновлена");
        }
    }
}