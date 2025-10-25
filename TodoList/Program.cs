using System;

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
            try
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

                int birthYear;
                while (true)
                {
                    Console.WriteLine("Введите Год Рождения");
                    string input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Ошибка: Год рождения не может быть пустым");
                        continue;
                    }

                    if (!int.TryParse(input, out birthYear))
                    {
                        Console.WriteLine("Ошибка: Неверный формат года рождения. Введите число.");
                        continue;
                    }

                    break;
                }

                var profile = new Profile(name, surname, birthYear);
                Console.WriteLine($"Добавлен пользователь {profile.GetInfo()}");

                return profile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла непредвиденная ошибка при создании профиля: {ex.Message}");
                return null;
            }
        }

        static void RunTodoApplication(Profile user)
        {
            bool isRunning = true;

            Console.WriteLine("Введите команду help для просмотра всех команд.");

            while (isRunning)
            {
                try
                {
                    Console.Write("> ");
                    string input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    CommandData commandData = ParseUserInput(input);

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
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                    Console.WriteLine("Программа продолжает работу...");
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

            todoList.Add(new TodoItem(newTask));
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
                    if (showIncompleteOnly && todoList.GetTask(i).IsDone)
                        continue;

                    string shortInfo = todoList.GetTask(i).GetShortInfo();
                    string[] parts = shortInfo.Split(new[] { " | " }, StringSplitOptions.None);
                    Console.WriteLine(parts.Length > 0 ? parts[0].Trim() : shortInfo);
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
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении задачи: {ex.Message}");
            }
        }

        static void MarkTaskAsDone(string argument)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отметке задачи: {ex.Message}");
            }
        }

        static void DeleteTask(string argument)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении задачи: {ex.Message}");
            }
        }

        static void UpdateTask(string argument)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении задачи: {ex.Message}");
            }
        }
    }
}