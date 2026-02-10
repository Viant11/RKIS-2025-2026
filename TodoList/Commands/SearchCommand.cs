using System;
using System.Collections.Generic;
using System.Linq;

public class SearchCommand : ICommand
{
	public string? Contains { get; set; }
	public string? StartsWith { get; set; }
	public string? EndsWith { get; set; }

	public DateTime? FromDate { get; set; }
	public DateTime? ToDate { get; set; }

	public TodoStatus? Status { get; set; }

	public string? SortBy { get; set; }
	public bool IsDesc { get; set; }
	public int? Top { get; set; }

	public void Execute()
	{
		if (AppInfo.Todos == null)
		{
			Console.WriteLine("Ошибка: TodoList не инициализирован");
			return;
		}

		var query = AppInfo.Todos
			.Select((item, index) => new { Item = item, Index = index });

		if (!string.IsNullOrEmpty(Contains))
		{
			query = query.Where(x => x.Item.Text.Contains(Contains, StringComparison.OrdinalIgnoreCase));
		}
		if (!string.IsNullOrEmpty(StartsWith))
		{
			query = query.Where(x => x.Item.Text.Trim().StartsWith(StartsWith, StringComparison.OrdinalIgnoreCase));
		}
		if (!string.IsNullOrEmpty(EndsWith))
		{
			query = query.Where(x => x.Item.Text.Trim().EndsWith(EndsWith, StringComparison.OrdinalIgnoreCase));
		}

		if (FromDate.HasValue)
		{
			query = query.Where(x => x.Item.LastUpdate.Date >= FromDate.Value.Date);
		}
		if (ToDate.HasValue)
		{
			query = query.Where(x => x.Item.LastUpdate.Date <= ToDate.Value.Date);
		}

		if (Status.HasValue)
		{
			query = query.Where(x => x.Item.Status == Status.Value);
		}

		if (!string.IsNullOrEmpty(SortBy))
		{
			if (SortBy.Equals("text", StringComparison.OrdinalIgnoreCase))
			{
				query = IsDesc
					? query.OrderByDescending(x => x.Item.Text)
					: query.OrderBy(x => x.Item.Text);
			}
			else if (SortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
			{
				query = IsDesc
					? query.OrderByDescending(x => x.Item.LastUpdate)
					: query.OrderBy(x => x.Item.LastUpdate);
			}
		}

		if (Top.HasValue && Top.Value > 0)
		{
			query = query.Take(Top.Value);
		}

		var results = query.ToList();

		Console.WriteLine("=== Результаты поиска ===");
		if (results.Count == 0)
		{
			Console.WriteLine("Ничего не найдено по заданным критериям.");
			return;
		}

		string header = "Индекс".PadRight(8) + " | " + "Текст задачи".PadRight(33) + " | " + "Статус".PadRight(12) + " | Дата изменения";
		Console.WriteLine(header);
		Console.WriteLine(new string('-', header.Length));

		foreach (var res in results)
		{
			Console.WriteLine($"#{res.Index + 1}".PadRight(8) + " | " + res.Item.GetShortInfo());
		}

		Console.WriteLine(new string('-', header.Length));
		Console.WriteLine($"Найдено задач: {results.Count}");
	}

	public void Unexecute()
	{
	
	}
}