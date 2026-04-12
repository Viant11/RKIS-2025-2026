using System;
using TodoList.Models;
using TodoList.Services;

namespace TodoList.Commands;

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
load <cnt> <size> - запуск параллельной имитации загрузки
sync [--pull | --push] - синхронизация с сервером
   --pull  - загрузить данные с сервера (по умолчанию)
   --push  - отправить локальные данные на сервер
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
load 5 100 (5 потоков, до 100 у.е.)
sync --push (отправить данные на сервер)
sync --pull (загрузить данные с сервера)
";

		Console.WriteLine(helpText);
	}
}