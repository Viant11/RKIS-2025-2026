using System;

namespace TodoList
{
    internal class Program
    {
        private const int InitialTasksCapacity = 2;
        private const string DateFormat = "yyyy";

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
            string[] tasks = new string[InitialTasksCapacity];
            bool isRunning = true;
            int taskCount = 0;

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
                        AddTask(ref tasks, ref taskCount, argument);
                        break;
                    case "view":
                        ShowTasks(tasks, taskCount);
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
            Console.WriteLine("exit - завершает цикл и останавливает выполнение программы");
        }

        static void ShowUserProfile((string Name, string Surname, int Age) user)
        {
            Console.WriteLine("Данные пользователя:");
            Console.WriteLine($"Имя: {user.Name}");
            Console.WriteLine($"Фамилия: {user.Surname}");
            Console.WriteLine($"Возраст: {user.Age}");
        }

        static void AddTask(ref string[] tasks, ref int taskCount, string taskDescription)
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
                tasks = ResizeTasksArray(tasks);
            }

            tasks[taskCount] = newTask;
            taskCount++;
            Console.WriteLine("Задача добавлена!");
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

        static void ShowTasks(string[] tasks, int taskCount)
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
                    Console.WriteLine($"{i + 1}. {tasks[i]}");
                }
            }
        }
    }
}