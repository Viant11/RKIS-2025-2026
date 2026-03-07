using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

public class FileManager : IDataStorage
{
	private readonly string _dataDir;
	private readonly string _profileFilePath;

	private static readonly byte[] Key = new byte[]
	{
		0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF,
		0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF,
		0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,
		0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x00
	};

	private static readonly byte[] IV = new byte[]
	{
		0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
		0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10
	};

	public FileManager(string dataDir, string profileFilePath)
	{
		_dataDir = dataDir;
		_profileFilePath = profileFilePath;
	}

	public void EnsureDataDirectory()
	{
		if (!Directory.Exists(_dataDir))
		{
			Directory.CreateDirectory(_dataDir);
			Console.WriteLine($"Создана папка: {_dataDir}");
		}
	}

	public void SaveProfiles(IEnumerable<Profile> profiles)
	{
		try
		{
			using (FileStream fs = new FileStream(_profileFilePath, FileMode.Create, FileAccess.Write))
			using (BufferedStream bs = new BufferedStream(fs))
			using (Aes aes = Aes.Create())
			{
				aes.Key = Key;
				aes.IV = IV;

				using (ICryptoTransform encryptor = aes.CreateEncryptor())
				using (CryptoStream cs = new CryptoStream(bs, encryptor, CryptoStreamMode.Write))
				using (StreamWriter sw = new StreamWriter(cs, Encoding.UTF8))
				{
					foreach (var p in profiles)
					{
						string line = $"{p.Id};{p.Login};{p.Password};{p.FirstName};{p.LastName};{p.BirthYear}";
						sw.WriteLine(line);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка сохранения профилей: {ex.Message}");
		}
	}

	public IEnumerable<Profile> LoadProfiles()
	{
		var profiles = new List<Profile>();
		if (!File.Exists(_profileFilePath))
		{
			return profiles;
		}

		try
		{
			using (FileStream fs = new FileStream(_profileFilePath, FileMode.Open, FileAccess.Read))
			using (BufferedStream bs = new BufferedStream(fs))
			using (Aes aes = Aes.Create())
			{
				aes.Key = Key;
				aes.IV = IV;

				using (ICryptoTransform decryptor = aes.CreateDecryptor())
				using (CryptoStream cs = new CryptoStream(bs, decryptor, CryptoStreamMode.Read))
				using (StreamReader sr = new StreamReader(cs, Encoding.UTF8))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						if (string.IsNullOrWhiteSpace(line)) continue;

						string[] parts = line.Split(';');
						if (parts.Length == 6)
						{
							if (Guid.TryParse(parts[0], out Guid id) &&
								int.TryParse(parts[5], out int birthYear))
							{
								var profile = new Profile(
									firstName: parts[3],
									lastName: parts[4],
									birthYear: birthYear,
									login: parts[1],
									password: parts[2],
									id: id
								);
								profiles.Add(profile);
							}
						}
					}
				}
			}
		}
		catch (CryptographicException)
		{
			Console.WriteLine("Ошибка: Не удалось расшифровать файл профилей.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка загрузки профилей: {ex.Message}");
		}
		return profiles;
	}

	public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
	{
		string filePath = Path.Combine(_dataDir, $"todos_{userId}.csv");
		try
		{
			using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			using (BufferedStream bs = new BufferedStream(fs))
			using (Aes aes = Aes.Create())
			{
				aes.Key = Key;
				aes.IV = IV;

				using (ICryptoTransform encryptor = aes.CreateEncryptor())
				using (CryptoStream cs = new CryptoStream(bs, encryptor, CryptoStreamMode.Write))
				using (StreamWriter sw = new StreamWriter(cs, Encoding.UTF8))
				{
					int i = 0;
					foreach (var item in todos)
					{
						string escapedText = item.Text.Replace("\"", "\"\"").Replace("\n", "\\n").Replace("\r", "\\r");
						string line = $"{i};\"{escapedText}\";{item.Status.ToString()};{item.LastUpdate:yyyy-MM-dd HH:mm:ss}";
						sw.WriteLine(line);
						i++;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка сохранения задач для пользователя {userId}: {ex.Message}");
		}
	}

	public IEnumerable<TodoItem> LoadTodos(Guid userId)
	{
		string filePath = Path.Combine(_dataDir, $"todos_{userId}.csv");
		var list = new List<TodoItem>();

		if (!File.Exists(filePath))
		{
			return list;
		}

		try
		{
			using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			using (BufferedStream bs = new BufferedStream(fs))
			using (Aes aes = Aes.Create())
			{
				aes.Key = Key;
				aes.IV = IV;

				using (ICryptoTransform decryptor = aes.CreateDecryptor())
				using (CryptoStream cs = new CryptoStream(bs, decryptor, CryptoStreamMode.Read))
				using (StreamReader sr = new StreamReader(cs, Encoding.UTF8))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						if (string.IsNullOrWhiteSpace(line)) continue;

						string[] parts = ParseCsvLine(line, ';');
						if (parts.Length == 4)
						{
							string text = parts[1].Replace("\"\"", "\"").Replace("\\n", "\n").Replace("\\r", "\r");

							if (Enum.TryParse<TodoStatus>(parts[2], true, out var status) &&
								DateTime.TryParse(parts[3], out DateTime lastUpdate))
							{
								var todoItem = new TodoItem(text, status, lastUpdate);
								list.Add(todoItem);
							}
						}
					}
				}
			}
		}
		catch (CryptographicException)
		{
			Console.WriteLine($"Ошибка дешифровки задач пользователя {userId}.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка загрузки задач для пользователя {userId}: {ex.Message}");
		}
		return list;
	}

	private string[] ParseCsvLine(string line, char separator = ';')
	{
		var parts = new List<string>();
		var currentPart = new StringBuilder();
		bool inQuotes = false;
		for (int i = 0; i < line.Length; i++)
		{
			char c = line[i];
			if (c == '"')
			{
				if (i + 1 < line.Length && line[i + 1] == '"')
				{
					currentPart.Append('"');
					i++;
				}
				else
				{
					inQuotes = !inQuotes;
				}
			}
			else if (c == separator && !inQuotes)
			{
				parts.Add(currentPart.ToString());
				currentPart.Clear();
			}
			else
			{
				currentPart.Append(c);
			}
		}
		parts.Add(currentPart.ToString());
		return parts.ToArray();
	}
}