using System;

namespace TodoListApp
{
    internal class Program
    {
        private const int InitialTasksCapacity = 2;
        private static TodoList todoList = new TodoList(InitialTasksCapacity);
        private static Profile? userProfile;

        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнил Соловьёв Евгений и Тареев Юрий");
            userProfile = CreateUserProfile();
            if (userProfile != null)
            {
                RunTodoApplication();
            }
            else
            {
                Console.WriteLine("Не удалось создать профиль пользователя.");
            }
        }

        static Profile? CreateUserProfile()
        {
            try
            {
                Console.WriteLine("Введите Имя");
                string? name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Ошибка: Имя не может быть пустым");
                    return null;
                }

                Console.WriteLine("Введите Фамилию");
                string? surname = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(surname))
                {
                    Console.WriteLine("Ошибка: Фамилия не может быть пустой");
                    return null;
                }

                int birthYear;
                while (true)
                {
                    Console.WriteLine("Введите Год Рождения");
                    string? input = Console.ReadLine();

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

        static void RunTodoApplication()
        {
            bool isRunning = true;

            Console.WriteLine("Введите команду help для просмотра всех команд.");

            while (isRunning)
            {
                try
                {
                    Console.Write("> ");
                    string? input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    ICommand command = CommandParser.Parse(input, todoList, userProfile);
                    command.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                    Console.WriteLine("Программа продолжает работу...");
                }
            }
        }
    }
}