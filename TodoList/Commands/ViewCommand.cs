using System;

public class ViewCommand : ICommand
{
	public bool ShowIndexFlag { get; set; }
	public bool ShowStatusFlag { get; set; }
	public bool ShowDateFlag { get; set; }
	public bool ShowAllFlag { get; set; }
	public bool IncompleteFlag { get; set; }
	public bool StatisticsFlag { get; set; }

	public void Execute()
	{
		if (AppInfo.Todos == null)
		{
			Console.WriteLine("Ошибка: TodoList не инициализирован");
			return;
		}

		if (AppInfo.Todos.Count == 0)
		{
			Console.WriteLine("Список задач пуст.");
			return;
		}

		bool showIndex = ShowIndexFlag;
		bool showStatus = ShowStatusFlag;
		bool showDate = ShowDateFlag;

		if (ShowAllFlag)
		{
			showIndex = true;
			showStatus = true;
			showDate = true;
		}

		bool showOnlyText = !showIndex && !showStatus && !showDate;

		if (showOnlyText)
		{
			for (int i = 0; i < AppInfo.Todos.Count; i++)
			{
				if (IncompleteFlag && AppInfo.Todos[i].Status == TodoStatus.Completed)
					continue;

				string shortInfo = AppInfo.Todos[i].GetShortInfo();
				string[] parts = shortInfo.Split(new[] { " | " }, StringSplitOptions.None);
				Console.WriteLine(parts.Length > 0 ? parts[0].Trim() : shortInfo);
			}
		}
		else
		{
			bool showDone = !IncompleteFlag;
			AppInfo.Todos.View(showIndex, showDone, showDate, showStatus);
		}

		if (StatisticsFlag)
		{
			int completedCount = AppInfo.Todos.GetCompletedCount();
			int totalCount = AppInfo.Todos.Count;
			int displayedCount = IncompleteFlag ? totalCount - completedCount : totalCount;

			Console.WriteLine("\n=== Статистика ===");
			Console.WriteLine($"Всего задач: {totalCount}");
			Console.WriteLine($"Выполнено: {completedCount}");
			Console.WriteLine($"Не выполнено: {totalCount - completedCount}");
			Console.WriteLine($"Показано: {displayedCount}");

			if (totalCount > 0)
			{
				double completionRate = (double)completedCount / totalCount * 100;
				Console.WriteLine($"Процент выполнения: {completionRate:F1}%");
			}
		}
	}
}