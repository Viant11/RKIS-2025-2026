using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

static class CommandParser
{
	private static Dictionary<string, Func<string, CommandData, ICommand>> _commandHandlers = new();

	static CommandParser()
	{
		_commandHandlers["help"] = (args, data) => new HelpCommand();
		_commandHandlers["profile"] = (args, data) => new ProfileCommand { LogoutFlag = data.LogoutFlag };
		_commandHandlers["add"] = (args, data) => new AddCommand { TaskDescription = args, MultilineFlag = data.MultilineFlag };

		_commandHandlers["search"] = (args, data) =>
		{
			var cmd = new SearchCommand();
			if (data.Parameters.TryGetValue("contains", out var c)) cmd.Contains = c;
			if (data.Parameters.TryGetValue("starts-with", out var s)) cmd.StartsWith = s;
			if (data.Parameters.TryGetValue("ends-with", out var e)) cmd.EndsWith = e;

			if (data.Parameters.TryGetValue("from", out var f) && DateTime.TryParse(f, out var df)) cmd.FromDate = df;
			if (data.Parameters.TryGetValue("to", out var t) && DateTime.TryParse(t, out var dt)) cmd.ToDate = dt;

			if (data.Parameters.TryGetValue("status", out var stat) && Enum.TryParse<TodoStatus>(stat, true, out var ds)) cmd.Status = ds;

			if (data.Parameters.TryGetValue("sort", out var sort)) cmd.SortBy = sort;
			if (data.Parameters.ContainsKey("desc")) cmd.IsDesc = true;

			if (data.Parameters.TryGetValue("top", out var top) && int.TryParse(top, out var topN)) cmd.Top = topN;

			if (string.IsNullOrEmpty(cmd.Contains) &&
				string.IsNullOrEmpty(cmd.StartsWith) &&
				string.IsNullOrEmpty(cmd.EndsWith) &&
				!string.IsNullOrWhiteSpace(args))
			{
				cmd.Contains = args;
			}

			return cmd;
		};

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

	private static List<string> SplitCommandLine(string line)
	{
		var result = new List<string>();
		var current = new StringBuilder();
		bool inQuotes = false;

		for (int i = 0; i < line.Length; i++)
		{
			char c = line[i];
			if (c == '\"')
			{
				inQuotes = !inQuotes;
			}
			else if (c == ' ' && !inQuotes)
			{
				if (current.Length > 0)
				{
					result.Add(current.ToString());
					current.Clear();
				}
			}
			else
			{
				current.Append(c);
			}
		}
		if (current.Length > 0) result.Add(current.ToString());
		return result;
	}

	private static CommandData ParseUserInput(string userInput)
	{
		var result = new CommandData
		{
			Parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		};

		if (string.IsNullOrEmpty(userInput)) return result;

		List<string> parts = SplitCommandLine(userInput);
		if (parts.Count == 0) return result;

		result.Command = parts[0];

		var looseArgs = new List<string>();

		for (int i = 1; i < parts.Count; i++)
		{
			string part = parts[i];

			if (part.StartsWith("--"))
			{
				string key = part.Substring(2);

				if (i + 1 < parts.Count && !parts[i + 1].StartsWith("-"))
				{
					result.Parameters[key] = parts[i + 1];

					HandleLegacyFlags(key, ref result);

					i++;
				}
				else
				{
					result.Parameters[key] = "true";
					HandleLegacyFlags(key, ref result);
				}
			}
			else if (part.StartsWith("-"))
			{
				string shortFlags = part.Substring(1);
				foreach (char c in shortFlags)
				{
					HandleLegacyShortFlag(c, ref result);
				}
			}
			else
			{
				looseArgs.Add(part);
			}
		}

		result.Argument = string.Join(" ", looseArgs);
		return result;
	}

	private static void HandleLegacyFlags(string key, ref CommandData data)
	{
		switch (key.ToLower())
		{
			case "multiline": data.MultilineFlag = true; break;
			case "index": data.ShowIndexFlag = true; break;
			case "status": data.ShowStatusFlag = true; break;
			case "update-date": data.ShowDateFlag = true; break;
			case "all": data.ShowAllFlag = true; break;
			case "incomplete": data.IncompleteFlag = true; break;
			case "statistics": data.StatisticsFlag = true; break;
			case "out": data.LogoutFlag = true; break;
		}
	}

	private static void HandleLegacyShortFlag(char flag, ref CommandData data)
	{
		switch (flag)
		{
			case 'm': data.MultilineFlag = true; break;
			case 'i': data.ShowIndexFlag = true; break;
			case 's': data.ShowStatusFlag = true; break;
			case 'd': data.ShowDateFlag = true; break;
			case 'a': data.ShowAllFlag = true; break;
			case 'I': data.IncompleteFlag = true; break;
			case 'S': data.StatisticsFlag = true; break;
			case 'o': data.LogoutFlag = true; break;
		}
	}
}