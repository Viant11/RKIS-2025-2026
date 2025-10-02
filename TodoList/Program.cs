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
            bool Running = true;

            string taskString = "";
            string[] tasks = new string[2];
            int taskCount = 0;

            todos[1] = "profile - выводит данные о пользователе";
            todos[2] = "add - добавляет новую задачу";
            todos[3] = "view - выводит все задачи из массива";
            todos[4] = "exit - завершает цикл и останавливает выполнение программы";
         
            Console.WriteLine("Введите команду help для просмотра всех команд.");

            while (Running)
            {

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "help":
                        Console.WriteLine("Доступные команды: ");
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
                        Console.WriteLine("Введи задачу которую хотите добавить:");
                        string Newtask = Console.ReadLine();

                        if (taskCount >= tasks.Length)
                        {
                            string[] newTasks = new string[tasks.Length * 2];

                            for (int i = 0; i < tasks.Length; i++)
                            {
                                newTasks[i] = tasks[i];
                            }

                            tasks = newTasks;
                            Console.WriteLine($"Массив расширен до {tasks.Length} элементов");
                        }


                        tasks[taskCount] = Newtask;
                        taskCount++;

                        if (string.IsNullOrEmpty(taskString))
                        {
                            taskString = Newtask;
                        }
                        else
                        {
                            taskString += "|" + Newtask; 
                        }

                        Console.WriteLine("Задача добавлена!");
                        break;

                    case "view":
                        if (string.IsNullOrEmpty(taskString))
                        {
                            Console.WriteLine("Список задач пуст.");
                        }
                        else
                        {
                            Console.WriteLine("Все задачи:");
                            string[] tasksGroup = taskString.Split("|");
                            for (int i = 0; i < tasksGroup.Length; i++)
                            {
                                Console.WriteLine($"{i + 1}. {tasksGroup[i]}");
                            }
                        }
                        break;

                    case "exit":
                        Running = false;
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