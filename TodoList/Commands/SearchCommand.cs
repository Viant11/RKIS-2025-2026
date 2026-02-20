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
		if (!AppInfo.CurrentProfileId.HasValue)
			throw new AuthenticationException("Вы не авторизованы.");

		if (AppInfo.Todos == null)
			throw new InvalidOperationException("TodoList не инициализирован.");

		var query = AppInfo.Todos.AsEnumerable();

		if (!string.IsNullOrEmpty(Contains))
			query = query.Where(x => x.Text.Contains(Contains, StringComparison.OrdinalIgnoreCase));

		if (!string.IsNullOrEmpty(StartsWith))
			query = query.Where(x => x.Text.Trim().StartsWith(StartsWith, StringComparison.OrdinalIgnoreCase));

		if (!string.IsNullOrEmpty(EndsWith))
			query = query.Where(x => x.Text.Trim().EndsWith(EndsWith, StringComparison.OrdinalIgnoreCase));

		if (FromDate.HasValue)
			query = query.Where(x => x.LastUpdate.Date >= FromDate.Value.Date);

		if (ToDate.HasValue)
			query = query.Where(x => x.LastUpdate.Date <= ToDate.Value.Date);

		if (Status.HasValue)
			query = query.Where(x => x.Status == Status.Value);

		if (!string.IsNullOrEmpty(SortBy))
		{
			if (SortBy.Equals("text", StringComparison.OrdinalIgnoreCase))
			{
				query = IsDesc
					? query.OrderByDescending(x => x.Text)
					: query.OrderBy(x => x.Text);
			}
			else if (SortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
			{
				query = IsDesc
					? query.OrderByDescending(x => x.LastUpdate)
					: query.OrderBy(x => x.LastUpdate);
			}
			else
			{
				throw new InvalidArgumentException($"Неверное поле сортировки: {SortBy}. Доступно: text, date.");
			}
		}

		if (Top.HasValue && Top.Value > 0)
		{
			query = query.Take(Top.Value);
		}

		var resultsList = query.ToList();

		if (resultsList.Count == 0)
		{
			Console.WriteLine("Ничего не найдено");
			return;
		}

		Console.WriteLine("=== Результаты поиска ===");
		var tempTodoList = new TodoList(resultsList);
		tempTodoList.View(true, true, true, true);

		Console.WriteLine($"Найдено задач: {resultsList.Count}");
	}
}