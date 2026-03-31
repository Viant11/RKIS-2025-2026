using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TodoList.Server
{
	class Program
	{
		private static readonly string DataDir = "server_data";
		private static readonly ConcurrentDictionary<string, byte[]> _cache = new();

		static async Task Main(string[] args)
		{
			Console.WriteLine("TodoList Server v1.0");
			Console.WriteLine("Сервер запускается...");

			if (!Directory.Exists(DataDir))
			{
				Directory.CreateDirectory(DataDir);
			}

			using var listener = new HttpListener();
			listener.Prefixes.Add("http://localhost:5000/");
			listener.Start();

			Console.WriteLine("Сервер запущен на http://localhost:5000/");
			Console.WriteLine("Нажмите Ctrl+C для остановки");

			Console.CancelKeyPress += (sender, e) =>
			{
				Console.WriteLine("\nЗавершение работы...");
				listener.Stop();
				Environment.Exit(0);
			};

			while (true)
			{
				try
				{
					var context = await listener.GetContextAsync();
					_ = Task.Run(() => ProcessRequestAsync(context));
				}
				catch (HttpListenerException)
				{
					break;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Ошибка: {ex.Message}");
				}
			}
		}

		private static async Task ProcessRequestAsync(HttpListenerContext context)
		{
			var request = context.Request;
			var response = context.Response;

			try
			{
				Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {request.HttpMethod} {request.Url.PathAndQuery}");

				var path = request.Url.AbsolutePath;
				var method = request.HttpMethod;

				if (path == "/profiles" && method == "GET")
				{
					await HandleGetProfiles(response);
				}
				else if (path == "/profiles" && method == "POST")
				{
					await HandlePostProfiles(request, response);
				}
				else if (path.StartsWith("/todos/") && method == "GET")
				{
					var userId = path.Substring("/todos/".Length);
					await HandleGetTodos(userId, response);
				}
				else if (path.StartsWith("/todos/") && method == "POST")
				{
					var userId = path.Substring("/todos/".Length);
					await HandlePostTodos(userId, request, response);
				}
				else if (path == "/health" && method == "GET")
				{
					await HandleHealthCheck(response);
				}
				else
				{
					response.StatusCode = (int)HttpStatusCode.NotFound;
					await WriteResponse(response, "Not Found");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка обработки запроса: {ex.Message}");
				response.StatusCode = (int)HttpStatusCode.InternalServerError;
				await WriteResponse(response, "Internal Server Error");
			}
			finally
			{
				response.Close();
			}
		}

		private static async Task HandleHealthCheck(HttpListenerResponse response)
		{
			response.StatusCode = (int)HttpStatusCode.OK;
			response.ContentType = "text/plain";
			await WriteResponse(response, "OK");
		}

		private static async Task HandleGetProfiles(HttpListenerResponse response)
		{
			var filePath = Path.Combine(DataDir, "profiles.dat");

			if (File.Exists(filePath))
			{
				var data = await File.ReadAllBytesAsync(filePath);
				response.StatusCode = (int)HttpStatusCode.OK;
				response.ContentType = "application/octet-stream";
				await response.OutputStream.WriteAsync(data, 0, data.Length);
			}
			else
			{
				response.StatusCode = (int)HttpStatusCode.OK;
				response.ContentType = "application/octet-stream";
				await WriteResponse(response, "");
			}
		}

		private static async Task HandlePostProfiles(HttpListenerRequest request, HttpListenerResponse response)
		{
			var filePath = Path.Combine(DataDir, "profiles.dat");
			var data = await ReadRequestBody(request);

			await File.WriteAllBytesAsync(filePath, data);
			response.StatusCode = (int)HttpStatusCode.OK;
			await WriteResponse(response, "OK");
		}

		private static async Task HandleGetTodos(string userId, HttpListenerResponse response)
		{
			if (!Guid.TryParse(userId, out _))
			{
				response.StatusCode = (int)HttpStatusCode.BadRequest;
				await WriteResponse(response, "Invalid userId format");
				return;
			}

			var filePath = Path.Combine(DataDir, $"todos_{userId}.dat");

			if (File.Exists(filePath))
			{
				var data = await File.ReadAllBytesAsync(filePath);
				response.StatusCode = (int)HttpStatusCode.OK;
				response.ContentType = "application/octet-stream";
				await response.OutputStream.WriteAsync(data, 0, data.Length);
			}
			else
			{
				response.StatusCode = (int)HttpStatusCode.OK;
				response.ContentType = "application/octet-stream";
				await WriteResponse(response, "");
			}
		}

		private static async Task HandlePostTodos(string userId, HttpListenerRequest request, HttpListenerResponse response)
		{
			if (!Guid.TryParse(userId, out _))
			{
				response.StatusCode = (int)HttpStatusCode.BadRequest;
				await WriteResponse(response, "Invalid userId format");
				return;
			}

			var filePath = Path.Combine(DataDir, $"todos_{userId}.dat");
			var data = await ReadRequestBody(request);

			await File.WriteAllBytesAsync(filePath, data);
			response.StatusCode = (int)HttpStatusCode.OK;
			await WriteResponse(response, "OK");
		}

		private static async Task<byte[]> ReadRequestBody(HttpListenerRequest request)
		{
			using var ms = new MemoryStream();
			await request.InputStream.CopyToAsync(ms);
			return ms.ToArray();
		}

		private static async Task WriteResponse(HttpListenerResponse response, string content)
		{
			var buffer = System.Text.Encoding.UTF8.GetBytes(content);
			await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
		}
	}
}