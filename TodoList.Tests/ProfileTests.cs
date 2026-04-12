using Xunit;
using System;
using TodoList.Models;

namespace ProjectTests
{
	public class ProfileTests
	{
		[Fact]
		public void Constructor_ShouldInitializePropertiesCorrectly()
		{
			// Arrange
			string first = "Ivan";
			string last = "Ivanov";
			int year = 1990;
			string login = "ivan90";
			string pass = "secret";

			// Act
			var profile = new Profile
			{
				FirstName = first,
				LastName = last,
				BirthYear = year,
				Login = login,
				Password = pass
			};

			// Assert
			Assert.Equal(first, profile.FirstName);
			Assert.Equal(last, profile.LastName);
			Assert.Equal(year, profile.BirthYear);
			Assert.Equal(login, profile.Login);
			Assert.Equal(pass, profile.Password);
		}

		[Fact]
		public void GetInfo_ShouldCalculateAgeCorrectly()
		{
			// Arrange
			int birthYear = 2000;
			var profile = new Profile
			{
				FirstName = "Test",
				LastName = "User",
				BirthYear = birthYear,
				Login = "login",
				Password = "pass"
			};

			int currentYear = DateTime.Now.Year;
			int expectedAge = currentYear - birthYear;

			// Act
			string info = profile.GetInfo();

			// Assert
			Assert.Contains($"возраст {expectedAge}", info);
			Assert.Contains("Test User", info);
			Assert.Contains("login", info);
		}
	}
}