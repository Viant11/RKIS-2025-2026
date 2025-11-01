using System;

public class Profile
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public int BirthYear { get; private set; }

    public Profile(string firstName, string lastName, int birthYear)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthYear = birthYear;
    }

    public string GetInfo()
    {
        int currentYear = DateTime.Now.Year;
        int age = currentYear - BirthYear;
        return $"{FirstName} {LastName}, возраст {age}";
    }
}