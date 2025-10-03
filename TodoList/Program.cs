using System.Data;

namespace TodoList
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнил Соловьёв Евгений и Тареев Юрий");
            Console.WriteLine("Введите Имя");
            string Name = Console.ReadLine();
            Console.WriteLine("Введите Фамилию");
            string Surname = Console.ReadLine();
            Console.WriteLine("Введите Год Рождения");
            string date = Console.ReadLine();
            int number = int.Parse(date);
            string Date = DateTime.Now.ToString("yyyy");
            int CurrentDate = int.Parse(Date);
            int age = CurrentDate - number;
            Console.WriteLine("Добавлен пользователь" + " " + Name + " " + Surname + " " + "возраст - " + age);


            string[] todos = { "help", "profile", "add", "view", "exit" };
            bool isRunning = true;

            string taskString = "";
            string[] tasks = new string[2];
            int taskCount = 0;

            todos[1] = "profile - выводит данные о пользователе";
            todos[2] = "add - добавляет новую задачу";
            todos[3] = "view - выводит все задачи из массива";
            todos[4] = "exit - завершает цикл и останавливает выполнение программы";

            Console.WriteLine("Введите команду help для просмотра всех команд.");

            while (isRunning)
            {

                string userInput = Console.ReadLine();
                string argument = "";


                if (userInput.Contains(" "))
                {
                    string[] parts = userInput.Split(new char[] { ' ' }, 2);
                    userInput = parts[0];
                    argument = parts.Length > 1 ? parts[1] : "";
                }

                switch (userInput)
                {
                    case "help":
                        Console.WriteLine(todos[1]);
                        Console.WriteLine(todos[2]);
                        Console.WriteLine(todos[3]);
                        Console.WriteLine(todos[4]);
                        break;

                    case "profile":
                        Console.WriteLine("Данные пользователя:");
                        Console.WriteLine($"Имя:{Name}");
                        Console.WriteLine($"Фамилия:{Surname}");
                        Console.WriteLine($"Возраст:{age}");
                        break;

                    case "add":
                        if (string.IsNullOrWhiteSpace(argument))
                        {
                            Console.WriteLine("Ошибка: Укажите задачу. Пример: add Новая задача");
                            break;
                        }

                        string newTask = argument.Trim();

                        if (string.IsNullOrWhiteSpace(newTask))
                        {
                            Console.WriteLine("Ошибка: Задача не может быть пустой");
                            break;
                        }

                        if (taskCount >= todos.Length)
                        {
                            string[] newTodos = new string[todos.Length * 2];
                            for (int i = 0; i < todos.Length; i++)
                            {
                                newTodos[i] = todos[i];
                            }
                            todos = newTodos;
                        }

                        todos[taskCount] = newTask;
                        taskCount++;

                        Console.WriteLine("Задача добавлена!");
                        break;

                    case "view":
                        if (taskCount == 0)
                        {
                            Console.WriteLine("Список задач пуст.");
                        }
                        else
                        {
                            Console.WriteLine("Все задачи:");
                            for (int i = 0; i < taskCount; i++)
                            {
                                Console.WriteLine($"{i + 1}. {todos[i]}");
                            }
                        }
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

    }
}