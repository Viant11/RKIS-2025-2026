using System;

namespace TodoList;

public class SystemClock : IClock
{
	public DateTime Now => DateTime.Now;
}