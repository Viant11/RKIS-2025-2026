using System;
using System.Linq;

public class ProfileCommand : ICommand
{
	public void Execute()
	{
		if (!AppInfo.CurrentProfileId.HasValue)
		{
			Console.WriteLine("Вы не вошли ни в один профиль.");
			return;
		}

		Profile currentUser = AppInfo.AllProfiles.FirstOrDefault(p => p.Id == AppInfo.CurrentProfileId.Value);

		if (currentUser == null)
		{
			Console.WriteLine("Ошибка: не удалось найти данные текущего профиля.");
			return;
		}

		string profile = @$"
Данные пользователя:
{currentUser.GetInfo()}";

		Console.WriteLine(profile);
	}

	public void Unexecute() { }
}