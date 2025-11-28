using System;

public class ProfileCommand : ICommand
{
	public void Execute()
	{
		if (AppInfo.CurrentProfile == null)
		{
			Console.WriteLine("Профиль пользователя не создан.");
			return;
		}

		string profile = @$"
Данные пользователя:
{AppInfo.CurrentProfile.GetInfo()}";

		Console.WriteLine(profile);
	}

	public void Unexecute() { }
}