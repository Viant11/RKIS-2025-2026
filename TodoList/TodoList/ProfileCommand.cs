public class ProfileCommand : ICommand
{
    public Profile? UserProfile { get; set; }

    public void Execute()
    {
        if (UserProfile == null)
        {
            Console.WriteLine("Профиль пользователя не создан.");
            return;
        }

        string profile = @$"
Данные пользователя:
{UserProfile.GetInfo()}";

        Console.WriteLine(profile);
    }
}