public class ProfileCommand : ICommand
{
    public Profile? UserProfile { get; set; }

    public void Execute()
    {
        if (UserProfile == null)
        {
            Console.WriteLine("������� ������������ �� ������.");
            return;
        }

        string profile = @$"
������ ������������:
{UserProfile.GetInfo()}";

        Console.WriteLine(profile);
    }
}