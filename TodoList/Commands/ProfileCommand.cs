using System;
using System.Linq;

public class ProfileCommand : ICommand
{
	public bool LogoutFlag { get; set; }

	public void Execute()
	{
		if (LogoutFlag)
		{
			AppInfo.CurrentProfileId = null;
			return;
		}

		if (!AppInfo.CurrentProfileId.HasValue)
		{
			throw new AuthenticationException("Пользователь не авторизован.");
		}

		Profile currentUser = AppInfo.AllProfiles.FirstOrDefault(p => p.Id == AppInfo.CurrentProfileId.Value);

		if (currentUser == null)
		{
			throw new ProfileNotFoundException("Текущий профиль не найден в базе данных.");
		}

		string profile = @$"
Данные пользователя:
{currentUser.GetInfo()}";

		Console.WriteLine(profile);
	}
}