using System;

namespace TodoList;

public interface IClock
{
	DateTime Now { get; }
}