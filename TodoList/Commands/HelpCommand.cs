using System;

public class HelpCommand : ICommand
{
	public void Execute()
	{
		string helpText = @$"
Доступные команды:
profile - выводит данные о пользователе
add - добавляет новую задачу
view - выводит все задачи из массива
read <idx> - показывает полную информацию о задаче
status <idx> <status> - изменяет статус задачи
delete <idx> - удаляет задачу по индексу
update <idx> ""new_text"" - обновляет текст задачи
undo - отменить последнее действие
redo - вернуть отмененное действие
exit - сохраняет данные и завершает программу

SEARCH - расширенный поиск:
search --contains ""текст""   : содержит текст
search --starts-with ""текст"" : начинается с текста
search --ends-with ""текст""   : заканчивается текстом
search --status <Status>     : поиск по статусу
search --from yyyy-MM-dd     : изменен не раньше даты
search --to yyyy-MM-dd       : изменен не позже даты
search --sort <text|date>    : сортировка
search --desc                : по убыванию
search --top <N>             : показать топ-N результатов

Примеры:
search --contains ""важно"" --status InProgress
search --sort date --desc --top 5
search --from 2023-01-01 --to 2023-12-31";

		Console.WriteLine(helpText);
	}
}