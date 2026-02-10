using System.Collections.Generic;

public struct CommandData
{
	public string Command;
	public string Argument;

	public bool MultilineFlag;
	public bool IncompleteFlag;
	public bool StatisticsFlag;
	public bool ShowIndexFlag;
	public bool ShowStatusFlag;
	public bool ShowDateFlag;
	public bool ShowAllFlag;
	public bool LogoutFlag;

	public Dictionary<string, string> Parameters;
}