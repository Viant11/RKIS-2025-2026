using System;

namespace TodoList.Exceptions;

public class StorageException : Exception
{
	public StorageException(string message, Exception innerException = null)
		: base(message, innerException) { }
}

public class FileSystemException : StorageException
{
	public string FilePath { get; }
	public FileSystemException(string message, string filePath, Exception innerException = null)
		: base(message, innerException)
	{
		FilePath = filePath;
	}
}

public class SecurityStorageException : StorageException
{
	public SecurityStorageException(string message, Exception innerException = null)
		: base(message, innerException) { }
}

public class DataCorruptionException : StorageException
{
	public DataCorruptionException(string message, Exception innerException = null)
		: base(message, innerException) { }
}