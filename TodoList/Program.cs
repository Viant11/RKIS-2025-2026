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


            string[] todos = new string[2];
            bool isRunning = true;
            int taskCount = 0;

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
                        Console.WriteLine("Доступные команды: ");
                        Console.WriteLine("profile - выводит данные о пользователе");
                        Console.WriteLine("add - добавляет новую задачу");
                        Console.WriteLine("view - выводит все задачи из массива");
                        Console.WriteLine("exit - завершает цикл и останавливает выполнение программы");
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