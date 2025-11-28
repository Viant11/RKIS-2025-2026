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
status <idx> <status> - изменяет статус задачи (статусы: {string.Join(", ", Enum.GetNames(typeof(TodoStatus)))})
delete <idx> - удаляет задачу по индексу
update <idx> ""new_text"" - обновляет текст задачи
undo - отменить последнее действие
redo - вернуть отмененное действие
exit - сохраняет данные и завершает программу

Флаги для add:
--multiline или -m - многострочный ввод для add

Флаги для view:
--index или -i - показывать индекс задачи
--status или -s - показывать статус задачи
--update-date или -d - показывать дату изменения
--all или -a - показывать все данные
--incomplete или -I - показывать только невыполненные
--statistics или -S - показывать статистику

Примеры: view -isd, view --all, status 1 InProgress";

		Console.WriteLine(helpText);
	}

	public void Unexecute() { }
}