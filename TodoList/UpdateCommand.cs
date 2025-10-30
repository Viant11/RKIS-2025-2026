public class UpdateCommand : ICommand
{
    public TodoList TodoList { get; set; }
    public string Argument { get; set; }

    public void Execute()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Argument))
            {
                Console.WriteLine("������: ������� ������ � ����� ����� ������. ������: update 1 \"����� �����\"");
                return;
            }

            if (!Argument.Contains(" "))
            {
                Console.WriteLine("������: �������� ������ �������. ������: update 1 \"����� �����\"");
                return;
            }

            int firstSpaceIndex = Argument.IndexOf(' ');
            string indexStr = Argument.Substring(0, firstSpaceIndex);
            string newText = Argument.Substring(firstSpaceIndex + 1).Trim();

            if (!int.TryParse(indexStr, out int index) || index <= 0 || index > TodoList.Count)
            {
                Console.WriteLine("������: �������� ������ ������. ������: update 1 \"����� �����\"");
                return;
            }

            if (newText.StartsWith("\"") && newText.EndsWith("\""))
            {
                newText = newText.Substring(1, newText.Length - 2);
            }

            if (string.IsNullOrWhiteSpace(newText))
            {
                Console.WriteLine("������: ����� ����� ������ �� ����� ���� ������");
                return;
            }

            TodoList.UpdateText(index - 1, newText);
            Console.WriteLine($"������ {index} ���������");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"������ ��� ���������� ������: {ex.Message}");
        }
    }
}