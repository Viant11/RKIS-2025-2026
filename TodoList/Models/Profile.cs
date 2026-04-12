using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Models;

public class Profile
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string Login { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public int BirthYear { get; set; }

	public List<TodoItem> TodoItems { get; set; } = new();

	public string GetInfo()
	{
		int age = DateTime.Now.Year - BirthYear;
		return $"{FirstName} {LastName}, возраст {age}, Логин: {Login}";
	}
}