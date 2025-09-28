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
            int CurrentDate = int.Parse(Date); ;
            int age = CurrentDate - number;
            Console.WriteLine("Добавлен пользователь" + " " + Name + " " + Surname + " " + "возраст - " + age);
        }
        
        static void Second(string[] args)
        {
            string[] todos = { };
            
            while(true)
            {


            }
        }
    
    } 
}