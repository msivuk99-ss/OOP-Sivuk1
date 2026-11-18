using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;

Console.OutputEncoding = Encoding.UTF8;

Func<Exception, bool> shouldRetry = ex => ex is IOException || ex is HttpRequestException;

var fileProcessor = new FileProcessor();
var networkClient = new NetworkClient();
string userData = "{\"id\": 8, \"name\": \"Олена Ковальчук\", \"role\": \"Admin\"}";

Console.WriteLine("=== Сценарій 1: Збереження даних у файл (FileProcessor) ===");
try
{
    RetryHelper.ExecuteWithRetry(() =>
    {
        fileProcessor.SaveUserData("user_data.json", userData);
        return true;
    }, retryCount: 5, initialDelay: TimeSpan.FromMilliseconds(100), shouldRetry: shouldRetry);
}
catch (Exception ex)
{
    Console.WriteLine($"[Main] Критична помилка запису у файл: {ex.Message}");
}
finally
{
    Console.WriteLine("[Main] Завершено операцію роботи з файловою системою.\n");
}

Console.WriteLine("=== Сценарій 2: Відправка даних на сервер (NetworkClient) ===");
try
{
    bool isSent = RetryHelper.ExecuteWithRetry(
        () => networkClient.PostUserData("https://api.example.com/users", userData),
        retryCount: 4,
        initialDelay: TimeSpan.FromMilliseconds(100),
        shouldRetry: shouldRetry
    );
    Console.WriteLine($"[Main] Статус відправки на сервер: {isSent}");
}
catch (Exception ex)
{
    Console.WriteLine($"[Main] Критична мережева помилка: {ex.Message}");
}
finally
{
    Console.WriteLine("[Main] Завершено мережевий запит.\n");
}

Console.WriteLine("=== Сценарій 3: Вибіркова обробка винятків через shouldRetry ===");
try
{
    RetryHelper.ExecuteWithRetry(() =>
    {
        fileProcessor.SaveUserData("", userData);
        return true;
    }, retryCount: 3, initialDelay: TimeSpan.FromMilliseconds(100), shouldRetry: shouldRetry);
}
catch (Exception ex)
{
    Console.WriteLine($"[Main] Перехоплено виняток без повторних спроб ({ex.GetType().Name}): {ex.Message}");
}
finally
{
    Console.WriteLine("[Main] Завершено перевірку вибіркової обробки винятків.");
}

public class FileProcessor
{
    private int _attempts = 0;

    public void SaveUserData(string path, string userData)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(path));
        }

        _attempts++;
        if (_attempts <= 4)
        {
            throw new IOException($"Файл '{path}' тимчасово зайнятий іншим процесом (збій #{_attempts}).");
        }

        Console.WriteLine($"[FileProcessor] Дані користувача успішно збережено у файл '{path}': {userData}");
    }
}

public class NetworkClient
{
    private int _attempts = 0;

    public bool PostUserData(string url, string userData)
    {
        _attempts++;
        if (_attempts <= 2)
        {
            throw new HttpRequestException($"Сервер '{url}' тимчасово недоступний: 503 Service Unavailable (збій #{_attempts}).");
        }

        Console.WriteLine($"[NetworkClient] Дані користувача успішно надіслано на '{url}': {userData}");
        return true;
    }
}

public static class RetryHelper
{
    public static T ExecuteWithRetry<T>(
        Func<T> operation,
        int retryCount = 3,
        TimeSpan initialDelay = default,
        Func<Exception, bool>? shouldRetry = null)
    {
        if (initialDelay == default)
        {
            initialDelay = TimeSpan.FromMilliseconds(200);
        }

        int attempt = 0;

        while (true)
        {
            try
            {
                Console.WriteLine($"[RetryHelper] Виконання спроби #{attempt + 1}...");
                return operation();
            }
            catch (Exception ex)
            {
                bool canRetry = attempt < retryCount && (shouldRetry == null || shouldRetry(ex));

                if (!canRetry)
                {
                    if (shouldRetry != null && !shouldRetry(ex))
                    {
                        Console.WriteLine($"[RetryHelper] Виняток {ex.GetType().Name} не підлягає повторній обробці згідно з shouldRetry.");
                    }
                    else
                    {
                        Console.WriteLine($"[RetryHelper] Вичерпано максимальну кількість повторних спроб ({retryCount}).");
                    }
                    throw;
                }

                TimeSpan delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, attempt));
                Console.WriteLine($"[RetryHelper] Помилка на спробі #{attempt + 1}: {ex.GetType().Name} — {ex.Message}");
                Console.WriteLine($"[RetryHelper] Очікування {delay.TotalMilliseconds} мс перед наступною спробою...");

                Thread.Sleep(delay);
                attempt++;
            }
        }
    }
}