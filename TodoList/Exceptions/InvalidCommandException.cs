using System;

public class InvalidCommandException : Exception
{
	public InvalidCommandException(string message) : base(message) { }
}