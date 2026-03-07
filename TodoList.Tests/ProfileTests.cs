using Xunit;
using System;

namespace ProjectTests
{
	public class ProfileTests
	{
		[Fact]
		public void Constructor_ShouldInitializePropertiesCorrectly()
		{
			// Arrange
			Guid id = Guid.NewGuid();
			string first = "Ivan";
			string last = "Ivanov";
			int year = 1990;
			string login = "ivan90";
			string pass = "secret";

			// Act
			var profile = new Profile(first, last, year, login, pass, id);

			// Assert
			Assert.Equal(id, profile.Id);
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
			var profile = new Profile("Test", "User", birthYear, "login", "pass", Guid.NewGuid());

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