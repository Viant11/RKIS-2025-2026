using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ApiDataStorage : IDataStorage
{
	private readonly string _baseUrl;
	private readonly HttpClient _httpClient;
	private readonly byte[] _key;
	private readonly byte[] _iv;

	public ApiDataStorage(string baseUrl = "http://localhost:5000/")
	{
		_baseUrl = baseUrl;
		_httpClient = new HttpClient();
		_httpClient.Timeout = TimeSpan.FromSeconds(30);

		_key = new byte[]
		{
			0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF,
			0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF,
			0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,
			0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x00
		};

		_iv = new byte[]
		{
			0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
			0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10
		};
	}

	public void SaveProfiles(IEnumerable<Profile> profiles)
	{
		var task = SaveProfilesAsync(profiles);
		task.Wait();
	}

	public IEnumerable<Profile> LoadProfiles()
	{
		var task = LoadProfilesAsync();
		task.Wait();
		return task.Result;
	}

	public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
	{
		var task = SaveTodosAsync(userId, todos);
		task.Wait();
	}

	public IEnumerable<TodoItem> LoadTodos(Guid userId)
	{
		var task = LoadTodosAsync(userId);
		task.Wait();
		return task.Result;
	}

	public async Task<bool> IsServerAvailableAsync()
	{
		try
		{
			var response = await _httpClient.GetAsync($"{_baseUrl}health");
			return response.IsSuccessStatusCode;
		}
		catch
		{
			return false;
		}
	}

	private async Task SaveProfilesAsync(IEnumerable<Profile> profiles)
	{
		var profilesList = profiles.ToList();
		var json = JsonSerializer.Serialize(profilesList, new JsonSerializerOptions
		{
			WriteIndented = true
		});

		var encryptedData = Encrypt(json);

		using var content = new ByteArrayContent(encryptedData);
		content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

		var response = await _httpClient.PostAsync($"{_baseUrl}profiles", content);
		response.EnsureSuccessStatusCode();
	}

	private async Task<List<Profile>> LoadProfilesAsync()
	{
		var response = await _httpClient.GetAsync($"{_baseUrl}profiles");
		response.EnsureSuccessStatusCode();

		var encryptedData = await response.Content.ReadAsByteArrayAsync();

		if (encryptedData == null || encryptedData.Length == 0)
		{
			return new List<Profile>();
		}

		var json = Decrypt(encryptedData);

		var profiles = JsonSerializer.Deserialize<List<Profile>>(json);
		return profiles ?? new List<Profile>();
	}

	private async Task SaveTodosAsync(Guid userId, IEnumerable<TodoItem> todos)
	{
		var todosList = todos.ToList();
		var json = JsonSerializer.Serialize(todosList, new JsonSerializerOptions
		{
			WriteIndented = true
		});

		var encryptedData = Encrypt(json);

		using var content = new ByteArrayContent(encryptedData);
		content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

		var response = await _httpClient.PostAsync($"{_baseUrl}todos/{userId}", content);
		response.EnsureSuccessStatusCode();
	}

	private async Task<List<TodoItem>> LoadTodosAsync(Guid userId)
	{
		var response = await _httpClient.GetAsync($"{_baseUrl}todos/{userId}");
		response.EnsureSuccessStatusCode();

		var encryptedData = await response.Content.ReadAsByteArrayAsync();

		if (encryptedData == null || encryptedData.Length == 0)
		{
			return new List<TodoItem>();
		}

		var json = Decrypt(encryptedData);

		var todos = JsonSerializer.Deserialize<List<TodoItem>>(json);
		return todos ?? new List<TodoItem>();
	}

	private byte[] Encrypt(string plainText)
	{
		using var aes = Aes.Create();
		aes.Key = _key;
		aes.IV = _iv;

		using var ms = new MemoryStream();
		using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
		using (var sw = new StreamWriter(cs, Encoding.UTF8))
		{
			sw.Write(plainText);
		}
		return ms.ToArray();
	}

	private string Decrypt(byte[] encryptedData)
	{
		using var aes = Aes.Create();
		aes.Key = _key;
		aes.IV = _iv;

		using var ms = new MemoryStream(encryptedData);
		using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
		using var sr = new StreamReader(cs, Encoding.UTF8);
		return sr.ReadToEnd();
	}
}