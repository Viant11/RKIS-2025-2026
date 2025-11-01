public class HelpCommand : ICommand
{
    public void Execute()
    {
        string helpText = @"
��������� �������:
profile - ������� ������ � ������������
add - ��������� ����� ������
view - ������� ��� ������ �� �������
read <idx> - ���������� ������ ���������� � ������
done <idx> - �������� ������ �����������
delete <idx> - ������� ������ �� �������
update <idx> ""new_text"" - ��������� ����� ������
exit - ��������� ���� � ������������� ���������� ���������

����� ��� add:
--multiline ��� -m - ������������� ���� ��� add

����� ��� view:
--index ��� -i - ���������� ������ ������
--status ��� -s - ���������� ������ ������
--update-date ��� -d - ���������� ���� ���������
--all ��� -a - ���������� ��� ������
--incomplete ��� -I - ���������� ������ �������������
--statistics ��� -S - ���������� ����������

�������: view -isd, view --all, view -i --status";

        Console.WriteLine(helpText);
    }
}