using System.Collections.Generic;
using System.Linq;
using TodoList.Data;
using TodoList.Models;

namespace TodoList.Services;

public class ProfileRepository
{
	public List<Profile> GetAll()
	{
		using var context = new AppDbContext();
		return context.Profiles.ToList();
	}

	public void Add(Profile profile)
	{
		using var context = new AppDbContext();
		context.Profiles.Add(profile);
		context.SaveChanges();
	}
}