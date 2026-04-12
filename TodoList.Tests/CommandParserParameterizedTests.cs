using Xunit;
using System;
using TodoList;
using TodoList.Commands;
using TodoList.Models;

namespace ProjectTests
{
	public class CommandParserParameterizedTests
	{
		[Theory]
		[InlineData("help", typeof(HelpCommand))]
		[InlineData("exit", typeof(ExitCommand))]
		[InlineData("undo", typeof(UndoCommand))]
		[InlineData("redo", typeof(RedoCommand))]
		public void Parse_SimpleCommands_ReturnsCorrectCommandType(string input, Type expectedType)
		{
			// Act
			var command = CommandParser.Parse(input);

			// Assert
			Assert.IsType(expectedType, command);
		}

		[Theory]
		[InlineData("add Buy Milk", "Buy Milk")]
		[InlineData("add \"Buy Milk\"", "Buy Milk")]
		[InlineData("add \"Task with spaces\"", "Task with spaces")]
		[InlineData("add SimpleTask", "SimpleTask")]
		public void Parse_AddCommand_WithText_ReturnsCorrectDescription(string input, string expectedDescription)
		{
			// Act
			var command = CommandParser.Parse(input);

			// Assert
			var addCommand = Assert.IsType<AddCommand>(command);
			Assert.Equal(expectedDescription, addCommand.TaskDescription);
		}

		[Theory]
		[InlineData("view -i", true, false, false)]
		[InlineData("view --index", true, false, false)]
		[InlineData("view -s", false, true, false)]
		[InlineData("view -d", false, false, true)]
		[InlineData("view --update-date", false, false, true)]
		[InlineData("view -i -s", true, true, false)]
		public void Parse_ViewCommand_WithFlags_SetsBooleanProperties(
			string input,
			bool expectedIndex,
			bool expectedStatus,
			bool expectedDate)
		{
			// Act
			var command = CommandParser.Parse(input);

			// Assert
			var viewCommand = Assert.IsType<ViewCommand>(command);
			Assert.Equal(expectedIndex, viewCommand.ShowIndexFlag);
			Assert.Equal(expectedStatus, viewCommand.ShowStatusFlag);
			Assert.Equal(expectedDate, viewCommand.ShowDateFlag);
		}

		[Theory]
		[InlineData("search --contains Milk", "Milk", null, null)]
		[InlineData("search --starts-with Priority", null, "Priority", null)]
		[InlineData("search --ends-with Today", null, null, "Today")]
		[InlineData("search --contains \"Buy Milk\" --starts-with A", "Buy Milk", "A", null)]
		public void Parse_SearchCommand_WithStringParams_PopulatesProperties(
			string input,
			string expectedContains,
			string expectedStartsWith,
			string expectedEndsWith)
		{
			// Act
			var command = CommandParser.Parse(input);

			// Assert
			var searchCommand = Assert.IsType<SearchCommand>(command);
			Assert.Equal(expectedContains, searchCommand.Contains);
			Assert.Equal(expectedStartsWith, searchCommand.StartsWith);
			Assert.Equal(expectedEndsWith, searchCommand.EndsWith);
		}

		[Theory]
		[InlineData("search --status InProgress", TodoStatus.InProgress)]
		[InlineData("search --status Completed", TodoStatus.Completed)]
		[InlineData("search --status failed", TodoStatus.Failed)]
		public void Parse_SearchCommand_WithStatusParam_SetsStatusEnum(string input, TodoStatus expectedStatus)
		{
			// Act
			var command = CommandParser.Parse(input);

			// Assert
			var searchCommand = Assert.IsType<SearchCommand>(command);
			Assert.Equal(expectedStatus, searchCommand.Status);
		}

		[Theory]
		[InlineData("search --from 2023-01-01")]
		[InlineData("search --to 2023-12-31")]
		public void Parse_SearchCommand_WithDateParams_ParsesDatesCorrectly(string input)
		{
			// Act
			var command = CommandParser.Parse(input);

			// Assert
			var searchCommand = Assert.IsType<SearchCommand>(command);

			if (input.Contains("from"))
				Assert.NotNull(searchCommand.FromDate);
			if (input.Contains("to"))
				Assert.NotNull(searchCommand.ToDate);
		}

		[Theory]
		[InlineData("search --unknown param")]
		[InlineData("search --status NotRealStatus")]
		[InlineData("search --from invalid-date")]
		[InlineData("search --to not-a-date")]
		[InlineData("search --top not_a_number")]
		public void Parse_SearchCommand_WithInvalidParams_ThrowsInvalidArgumentException(string input)
		{
			// Act & Assert
			Assert.Throws<InvalidArgumentException>(() => CommandParser.Parse(input));
		}

		[Theory]
		[InlineData("unknown_command")]
		[InlineData("dosomething")]
		[InlineData("test")]
		public void Parse_UnknownCommandString_ThrowsInvalidCommandException(string input)
		{
			// Act & Assert
			Assert.Throws<InvalidCommandException>(() => CommandParser.Parse(input));
		}

		[Theory]
		[InlineData("delete 1", "1")]
		[InlineData("read 5", "5")]
		[InlineData("status 2 InProgress", "2 InProgress")]
		[InlineData("update 3 \"New Text\"", "3 New Text")]
		public void Parse_ActionCommands_WithArguments_PassesArgumentString(string input, string expectedArg)
		{
			// Act
			var command = CommandParser.Parse(input);

			// Assert
			if (command is DeleteCommand del) Assert.Equal(expectedArg, del.Argument);
			else if (command is ReadCommand read) Assert.Equal(expectedArg, read.Argument);
			else if (command is StatusCommand stat) Assert.Equal(expectedArg, stat.Argument);
			else if (command is UpdateCommand upd) Assert.Equal(expectedArg, upd.Argument);
		}
	}
}