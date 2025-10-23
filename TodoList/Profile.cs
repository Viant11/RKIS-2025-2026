using System;

class Profile
{
    private string firstName;
    private string lastName;
    private int birthYear;

    public string FirstName
    {
        get => firstName;
        private set => firstName = value;
    }

    public string LastName
    {
        get => lastName;
        private set => lastName = value;
    }

    public int BirthYear
    {
        get => birthYear;
        private set => birthYear = value;
    }

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