using System;

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
                var (command, argument) = ParseUserInput(Console.ReadLine());

                switch (command)
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "profile":
                        ShowUserProfile(user);
                        break;
                    case "add":
                        AddTask(argument);
                        break;
                    case "view":
                        ShowTasks();
                        break;
                    case "done":
                        MarkTaskAsDone(argument);
                        break;
                    case "delete":
                        DeleteTask(argument);
                        break;
                    case "update":
                        UpdateTask(argument);
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

        static (string Command, string Argument) ParseUserInput(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
                return ("", "");

            if (!userInput.Contains(" "))
                return (userInput, "");

            string[] parts = userInput.Split(new char[] { ' ' }, 2);
            return (parts[0], parts.Length > 1 ? parts[1] : "");
        }

        static void ShowHelp()
        {
            Console.WriteLine("Доступные команды: ");
            Console.WriteLine("profile - выводит данные о пользователе");
            Console.WriteLine("add - добавляет новую задачу");
            Console.WriteLine("view - выводит все задачи из массива");
            Console.WriteLine("done <idx> - отмечает задачу выполненной");
            Console.WriteLine("delete <idx> - удаляет задачу по индексу");
            Console.WriteLine("update <idx> \"new_text\" - обновляет текст задачи");
            Console.WriteLine("exit - завершает цикл и останавливает выполнение программы");
        }

        static void ShowUserProfile((string Name, string Surname, int Age) user)
        {
            Console.WriteLine("Данные пользователя:");
            Console.WriteLine($"Имя: {user.Name}");
            Console.WriteLine($"Фамилия: {user.Surname}");
            Console.WriteLine($"Возраст: {user.Age}");
        }

        static void AddTask(string taskDescription)
        {
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

        static void ShowTasks()
        {
            if (taskCount == 0)
            {
                Console.WriteLine("Список задач пуст.");
            }
            else
            {
                Console.WriteLine("Все задачи:");
                for (int i = 0; i < taskCount; i++)
                {
                    string status = taskStatuses[i] ? "Выполнена" : "Не выполнена";
                    Console.WriteLine($"{i + 1}. {tasks[i]} - {status} (Создана: {taskDates[i]})");
                }
            }
        }

        static string[] ResizeTasksArray(string[] tasks)
        {
            string[] newTasks = new string[tasks.Length * 2];
            for (int i = 0; i < tasks.Length; i++)
            {
                newTasks[i] = tasks[i];
            }
            return newTasks;
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

            int firstSpaceIndex = argument.IndexOf(' ');
            if (firstSpaceIndex == -1)
            {
                Console.WriteLine("Ошибка: Неверный формат команды. Пример: update 1 \"Новый текст\"");
                return;
            }

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