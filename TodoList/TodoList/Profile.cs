using System;

public class Profile
{
	public Guid Id { get; private set; }
	public string Login { get; private set; }
	public string Password { get; private set; }
	public string FirstName { get; private set; }
	public string LastName { get; private set; }
	public int BirthYear { get; private set; }

	public Profile(string firstName, string lastName, int birthYear, string login, string password, Guid id)
	{
		FirstName = firstName;
		LastName = lastName;
		BirthYear = birthYear;
		Login = login;
		Password = password;
		Id = id;
	}

	public string GetInfo()
	{
		int currentYear = DateTime.Now.Year;
		int age = currentYear - BirthYear;
		return $"{FirstName} {LastName}, возраст {age}, Логин: {Login}";
	}
}