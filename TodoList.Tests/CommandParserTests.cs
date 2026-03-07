using Xunit;
using System;
using System.Linq;

namespace ProjectTests
{
	public class CommandParserTests
	{
		[Fact]
		public void Parse_Help_ShouldReturnHelpCommand()
		{
			var cmd = CommandParser.Parse("help");

			Assert.IsType<HelpCommand>(cmd);
		}

		[Fact]
		public void Parse_Add_ShouldReturnAddCommandWithText()
		{
			var cmd = CommandParser.Parse("add Купить молоко");

			var addCmd = Assert.IsType<AddCommand>(cmd);
			Assert.Equal("Купить молоко", addCmd.TaskDescription);
		}

		[Fact]
		public void Parse_Add_WithQuotes_ShouldHandleArguments()
		{
			var cmd = CommandParser.Parse("add \"Сложная задача с пробелами\"");

			var addCmd = Assert.IsType<AddCommand>(cmd);
			Assert.Equal("Сложная задача с пробелами", addCmd.TaskDescription);
		}

		[Fact]
		public void Parse_Delete_ShouldReturnDeleteCommand()
		{
			var cmd = CommandParser.Parse("delete 5");

			var delCmd = Assert.IsType<DeleteCommand>(cmd);
			Assert.Equal("5", delCmd.Argument);
		}

		[Fact]
		public void Parse_View_WithFlags_ShouldSetFlags()
		{
			var cmd = CommandParser.Parse("view --all -i");

			var viewCmd = Assert.IsType<ViewCommand>(cmd);
			Assert.True(viewCmd.ShowAllFlag);
			Assert.True(viewCmd.ShowIndexFlag);
			Assert.False(viewCmd.IncompleteFlag);
		}

		[Fact]
		public void Parse_Search_WithParams_ShouldReturnSearchCommand()
		{
			var cmd = CommandParser.Parse("search --contains \"важно\" --status InProgress");

			var searchCmd = Assert.IsType<SearchCommand>(cmd);
			Assert.Equal("важно", searchCmd.Contains);
			Assert.Equal(TodoStatus.InProgress, searchCmd.Status);
		}

		[Fact]
		public void Parse_Search_InvalidParam_ShouldThrowException()
		{
			Assert.Throws<InvalidArgumentException>(() =>
				CommandParser.Parse("search --invalidparam test"));
		}

		[Fact]
		public void Parse_UnknownCommand_ShouldThrowException()
		{
			Assert.Throws<InvalidCommandException>(() =>
				CommandParser.Parse("blablabla"));
		}

		[Fact]
		public void Parse_Update_ShouldReturnUpdateCommand()
		{
			var cmd = CommandParser.Parse("update 1 \"New Text\"");

			var updateCmd = Assert.IsType<UpdateCommand>(cmd);

			Assert.Equal("1 New Text", updateCmd.Argument);
		}
	}
}