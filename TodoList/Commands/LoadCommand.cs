using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Models;
using TodoList.Services;

namespace TodoList.Commands;

public class LoadCommand : ICommand
{
	public string Argument { get; set; }

	private static readonly object _consoleLock = new object();

	public void Execute()
	{
		try
		{
			RunAsync().Wait();
		}
		catch (AggregateException ae)
		{
			foreach (var e in ae.InnerExceptions)
			{
				if (e is InvalidArgumentException)
					throw e;
			}
			throw;
		}
	}

	private async Task RunAsync()
	{
		if (string.IsNullOrWhiteSpace(Argument))
			throw new InvalidArgumentException("Аргументы не указаны. Пример: load 3 100");

		string[] parts = Argument.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

		if (parts.Length != 2)
			throw new InvalidArgumentException("Неверный формат. Ожидалось: load <количество> <размер>");

		if (!int.TryParse(parts[0], out int count) || count <= 0)
			throw new InvalidArgumentException("Количество должно быть числом > 0.");

		if (!int.TryParse(parts[1], out int size) || size <= 0)
			throw new InvalidArgumentException("Размер должен быть числом > 0.");

		int startRow = Console.CursorTop;

		for (int i = 0; i < count; i++)
		{
			Console.WriteLine();
		}

		bool wasCursorVisible = Console.CursorVisible;
		Console.CursorVisible = false;

		try
		{
			var tasks = new List<Task>();

			for (int i = 0; i < count; i++)
			{
				tasks.Add(DownloadAsync(i, size, startRow));
			}

			await Task.WhenAll(tasks);
		}
		finally
		{
			lock (_consoleLock)
			{
				Console.SetCursorPosition(0, startRow + count);
				Console.CursorVisible = wasCursorVisible;
				Console.WriteLine("Все загрузки завершены.");
			}
		}
	}

	private async Task DownloadAsync(int index, int totalSize, int startRow)
	{
		var random = new Random(Guid.NewGuid().GetHashCode());

		for (int current = 0; current <= totalSize; current++)
		{
			DrawProgressBar(index, current, totalSize, startRow);

			await Task.Delay(random.Next(20, 100));
		}
	}

	private void DrawProgressBar(int index, int current, int total, int startRow)
	{
		const int barWidth = 20;

		double percent = (double)current / total;

		int filled = (int)(percent * barWidth);

		if (filled > barWidth) filled = barWidth;
		if (filled < 0) filled = 0;

		string bar = "[" + new string('#', filled) + new string('-', barWidth - filled) + "]";

		string status = $"{bar} {(int)(percent * 100)}%";

		lock (_consoleLock)
		{
			try
			{
				int row = startRow + index;

				if (row < Console.BufferHeight)
				{
					Console.SetCursorPosition(0, row);
					Console.Write(status);
				}
			}
			catch (ArgumentOutOfRangeException) { }
		}
	}
}