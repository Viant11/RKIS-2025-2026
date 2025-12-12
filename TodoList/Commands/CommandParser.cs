using System;
using System.Collections.Generic;

static class CommandParser
{
	private static Dictionary<string, Func<string, CommandData, ICommand>> _commandHandlers = new();

	static CommandParser()
	{
		_commandHandlers["help"] = (args, data) => new HelpCommand();
		_commandHandlers["profile"] = (args, data) => new ProfileCommand { LogoutFlag = data.LogoutFlag };
		_commandHandlers["add"] = (args, data) => new AddCommand { TaskDescription = args, MultilineFlag = data.MultilineFlag };
		_commandHandlers["view"] = (args, data) => new ViewCommand
		{
			ShowIndexFlag = data.ShowIndexFlag,
			ShowStatusFlag = data.ShowStatusFlag,
			ShowDateFlag = data.ShowDateFlag,
			ShowAllFlag = data.ShowAllFlag,
			IncompleteFlag = data.IncompleteFlag,
			StatisticsFlag = data.StatisticsFlag
		};
		_commandHandlers["read"] = (args, data) => new ReadCommand { Argument = args };
		_commandHandlers["status"] = (args, data) => new StatusCommand { Argument = args };
		_commandHandlers["delete"] = (args, data) => new DeleteCommand { Argument = args };
		_commandHandlers["update"] = (args, data) => new UpdateCommand { Argument = args };
		_commandHandlers["undo"] = (args, data) => new UndoCommand();
		_commandHandlers["redo"] = (args, data) => new RedoCommand();
		_commandHandlers["exit"] = (args, data) => new ExitCommand();
	}

	public static ICommand Parse(string inputString)
	{
		if (string.IsNullOrWhiteSpace(inputString))
			return new UnknownCommand();

		var commandData = ParseUserInput(inputString);
		string commandName = commandData.Command.ToLower();

		if (_commandHandlers.TryGetValue(commandName, out var handler))
		{
			return handler(commandData.Argument, commandData);
		}

		return new UnknownCommand();
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
					case "out": result.LogoutFlag = true; break;
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
						case 'o': result.LogoutFlag = true; break;
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