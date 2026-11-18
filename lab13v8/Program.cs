using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

string connectionString = "Server=localhost;Database=AppDb;Trusted_Connection=True;";
var dataService = new DataService(connectionString, isConnected: false);

Console.WriteLine("=== 1. Робота з власним винятком (GetData без з'єднання) ===");
try
{
    var records = dataService.GetData("SELECT * FROM Users");
    foreach (var record in records)
    {
        Console.WriteLine($" - {record}");
    }
}
catch (DatabaseConnectionException ex)
{
    Console.WriteLine($"[Catch] Перехоплено DatabaseConnectionException: {ex.Message}");
    Console.WriteLine($"[Catch] Рядок підключення: {ex.ConnectionString}");
}
catch (Exception ex)
{
    Console.WriteLine($"[Catch] Загальна помилка: {ex.Message}");
}

Console.WriteLine("\n=== 2. Успішне отримання даних (GetData після підключення) ===");
dataService.Connect();
try
{
    var records = dataService.GetData("SELECT * FROM Users");
    Console.WriteLine($"Отримано записів: {records.Count}");
    foreach (var record in records)
    {
        Console.WriteLine($" - {record}");
    }
}
catch (DatabaseConnectionException ex)
{
    Console.WriteLine($"[Catch] Помилка підключення: {ex.Message}");
}

Console.WriteLine("\n=== 3. Патерн Result<T>: спроба зберегти невалідні дані (порожній рядок) ===");
var emptySaveResult = dataService.SaveData("");
if (emptySaveResult.IsSuccess)
{
    Console.WriteLine($"[Result.Success] Дані збережено: {emptySaveResult.Value}");
}
else
{
    Console.WriteLine($"[Result.Failure] Помилка збереження: {emptySaveResult.Error}");
}

Console.WriteLine("\n=== 4. Патерн Result<T>: спроба зберегти невалідні дані (занадто короткі) ===");
var shortSaveResult = dataService.SaveData("abc");
if (shortSaveResult.IsSuccess)
{
    Console.WriteLine($"[Result.Success] Дані збережено: {shortSaveResult.Value}");
}
else
{
    Console.WriteLine($"[Result.Failure] Помилка збереження: {shortSaveResult.Error}");
}

Console.WriteLine("\n=== 5. Патерн Result<T>: успішне збереження валідних даних ===");
var validSaveResult = dataService.SaveData("{\"id\": 8, \"username\": \"student_v8\"}");
if (validSaveResult.IsSuccess)
{
    Console.WriteLine($"[Result.Success] Дані успішно збережено! Результат: {validSaveResult.Value}");
}
else
{
    Console.WriteLine($"[Result.Failure] Помилка збереження: {validSaveResult.Error}");
}

[Serializable]
public class DatabaseConnectionException : Exception
{
    public string ConnectionString { get; }

    public DatabaseConnectionException()
        : base("Помилка підключення до бази даних.")
    {
        ConnectionString = string.Empty;
    }

    public DatabaseConnectionException(string connectionString)
        : base($"Не вдалося встановити з'єднання з базою даних за рядком підключення: '{connectionString}'.")
    {
        ConnectionString = connectionString;
    }

    public DatabaseConnectionException(string connectionString, string message)
        : base(message)
    {
        ConnectionString = connectionString;
    }

    public DatabaseConnectionException(string message, Exception innerException)
        : base(message, innerException)
    {
        ConnectionString = string.Empty;
    }

    public DatabaseConnectionException(string connectionString, string message, Exception innerException)
        : base(message, innerException)
    {
        ConnectionString = connectionString;
    }

    [Obsolete("Formatter-based serialization is obsolete.", DiagnosticId = "SYSLIB0051")]
    protected DatabaseConnectionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ConnectionString = info.GetString(nameof(ConnectionString)) ?? string.Empty;
    }
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    private Result(T value, bool isSuccess, string error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(default!, false, error);
    }

    public static Result<T> Failure(Exception ex)
    {
        return new Result<T>(default!, false, ex.Message);
    }
}

public class DataService
{
    private readonly string _connectionString;
    private bool _isConnected;

    public DataService(string connectionString, bool isConnected = false)
    {
        _connectionString = connectionString;
        _isConnected = isConnected;
    }

    public void Connect()
    {
        _isConnected = true;
    }

    public void Disconnect()
    {
        _isConnected = false;
    }

    public List<string> GetData(string query)
    {
        if (!_isConnected)
        {
            throw new DatabaseConnectionException(_connectionString, $"Не вдалося виконати запит '{query}': відсутнє з'єднання з базою даних.");
        }

        return new List<string>
        {
            $"Результат 1 для запиту [{query}]",
            $"Результат 2 для запиту [{query}]",
            $"Результат 3 для запиту [{query}]"
        };
    }

    public Result<bool> SaveData(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return Result<bool>.Failure("Дані для збереження не можуть бути порожніми.");
        }

        if (data.Length < 5)
        {
            return Result<bool>.Failure("Дані занадто короткі (мінімум 5 символів).");
        }

        if (!_isConnected)
        {
            return Result<bool>.Failure(new DatabaseConnectionException(_connectionString));
        }

        return Result<bool>.Success(true);
    }
}