using System;

namespace Lab3V8
{
    public class TemporaryFile : IDisposable
    {
        private readonly string _tempFilePath;
        private bool _fileExists;
        private bool _disposed = false;

        public string TempFilePath => _tempFilePath;
        public bool FileExists => _fileExists;

        public TemporaryFile(string tempFilePath)
        {
            _tempFilePath = tempFilePath;
            _fileExists = true;
            Console.WriteLine($"[Конструктор]: Тимчасовий файл '{_tempFilePath}' створено.");
        }

        public void Write(string content)
        {
            if (_disposed || !_fileExists)
            {
                throw new ObjectDisposedException(nameof(TemporaryFile), "Неможливо виконати запис: файл вже видалено або ресурс звільнено!");
            }

            Console.WriteLine($"[Запис у файл '{_tempFilePath}']: {content}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine($"[Dispose(true)]: Звільнення керованих ресурсів для '{_tempFilePath}'.");
                }

                if (_fileExists)
                {
                    Console.WriteLine($"[Dispose]: Видаляємо тимчасовий файл '{_tempFilePath}'.");
                    _fileExists = false;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TemporaryFile()
        {
            Console.WriteLine($"[Деструктор]: Автоматична фіналізація для '{_tempFilePath}'.");
            Dispose(false);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("================ Сценарій 1: Використання оператора using ================");
            using (var file1 = new TemporaryFile("temp_log_1.tmp"))
            {
                file1.Write("Тестові дані для першого файла.");
            }
            Console.WriteLine();


            Console.WriteLine("================ Сценарій 2: Явний виклик Dispose() ================");
            var file2 = new TemporaryFile("temp_log_2.tmp");
            file2.Write("Тестові дані для другого файла.");
            file2.Dispose();
            Console.WriteLine();


            Console.WriteLine("================ Сценарій 3: Без Dispose() (робота деструктора) ================");
            CreateAndForgetFile();

            Console.WriteLine("Запускаємо GC.Collect() та чекаємо фіналізаторів...");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\n=== Всі сценарії виконано успішно ===");
        }


        static void CreateAndForgetFile()
        {
            var file3 = new TemporaryFile("temp_log_3.tmp");
            file3.Write("Цей файл буде видалено через Garbage Collector.");
        }
    }
}