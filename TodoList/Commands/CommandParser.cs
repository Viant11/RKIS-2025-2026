using System;
static class CommandParser
{
	public static ICommand Parse(string inputString, TodoList? todoList, Profile? profile)
	{
		if (string.IsNullOrWhiteSpace(inputString))
			return new UnknownCommand();

		var commandData = ParseUserInput(inputString);

		switch (commandData.Command.ToLower())
		{
			case "help":
				return new HelpCommand();
			case "profile":
				return new ProfileCommand { UserProfile = profile };
			case "add":
				return new AddCommand
				{
					TodoList = todoList,
					TaskDescription = commandData.Argument,
					MultilineFlag = commandData.MultilineFlag
				};
			case "view":
				return new ViewCommand
				{
					TodoList = todoList,
					ShowIndexFlag = commandData.ShowIndexFlag,
					ShowStatusFlag = commandData.ShowStatusFlag,
					ShowDateFlag = commandData.ShowDateFlag,
					ShowAllFlag = commandData.ShowAllFlag,
					IncompleteFlag = commandData.IncompleteFlag,
					StatisticsFlag = commandData.StatisticsFlag
				};
			case "read":
				return new ReadCommand { TodoList = todoList, Argument = commandData.Argument };
			case "status":
				return new StatusCommand { TodoList = todoList, Argument = commandData.Argument };
			case "delete":
				return new DeleteCommand { TodoList = todoList, Argument = commandData.Argument };
			case "update":
				return new UpdateCommand { TodoList = todoList, Argument = commandData.Argument };
			case "exit":
				return new ExitCommand { TodoList = todoList, UserProfile = profile };
			default:
				return new UnknownCommand();
		}
	}

	private static CommandData ParseUserInput(string userInput)
	{
		var result = new CommandData();
		if (string.IsNullOrEmpty(userInput)) return result;

		string[] parts = userInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length == 0) return result;

		result.Command = parts[0];
		string argument = "";

		for (int i = 1; i < parts.Length; i++)
		{
			if (parts[i] == null) continue;

			if (parts[i].StartsWith("--"))
			{
				string flagName = parts[i].Substring(2);
				switch (flagName)
				{
					case "multiline": result.MultilineFlag = true; break;
					case "index": result.ShowIndexFlag = true; break;
					case "status": result.ShowStatusFlag = true; break;
					case "update-date": result.ShowDateFlag = true; break;
					case "all": result.ShowAllFlag = true; break;
					case "incomplete": result.IncompleteFlag = true; break;
					case "statistics": result.StatisticsFlag = true; break;
				}
			}
			else if (parts[i].StartsWith("-") && parts[i].Length > 1)
			{
				string shortFlags = parts[i].Substring(1);
				foreach (char flagChar in shortFlags)
				{
					switch (flagChar)
					{
						case 'm': result.MultilineFlag = true; break;
						case 'i': result.ShowIndexFlag = true; break;
						case 's': result.ShowStatusFlag = true; break;
						case 'd': result.ShowDateFlag = true; break;
						case 'a': result.ShowAllFlag = true; break;
						case 'I': result.IncompleteFlag = true; break;
						case 'S': result.StatisticsFlag = true; break;
					}
				}
			}
			else
			{
				if (string.IsNullOrEmpty(argument))
					argument = parts[i];
				else
					argument += " " + parts[i];
			}
		}

		if (result.ShowAllFlag)
		{
			result.ShowIndexFlag = true;
			result.ShowStatusFlag = true;
			result.ShowDateFlag = true;
		}

		result.Argument = argument;
		return result;
	}
}