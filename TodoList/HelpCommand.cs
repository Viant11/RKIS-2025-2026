public class HelpCommand : ICommand
{
    public void Execute()
    {
        string helpText = @"
Доступные команды:
profile - выводит данные о пользователе
add - добавляет новую задачу
view - выводит все задачи из массива
read <idx> - показывает полную информацию о задаче
done <idx> - отмечает задачу выполненной
delete <idx> - удаляет задачу по индексу
update <idx> ""new_text"" - обновляет текст задачи
exit - завершает цикл и останавливает выполнение программы

Флаги для add:
--multiline или -m - многострочный ввод для add

Флаги для view:
--index или -i - показывать индекс задачи
--status или -s - показывать статус задачи
--update-date или -d - показывать дату изменения
--all или -a - показывать все данные
--incomplete или -I - показывать только невыполненные
--statistics или -S - показывать статистику

Примеры: view -isd, view --all, view -i --status";

        Console.WriteLine(helpText);
    }
}