using System;

namespace Lab2V8
{
    public class Laptop
    {
        private string _brand;
        private string _model;
        private decimal _price;

        public string Brand
        {
            get => _brand;
            set => _brand = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public string Model
        {
            get => _model;
            set => _model = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (value < 0)
                {
                    Console.WriteLine("Помилка: Ціна не може бути від'ємною! Встановлено 0.");
                    _price = 0;
                }
                else
                {
                    _price = value;
                }
            }
        }

        public Laptop() : this("Unknown Brand", "Unknown Model", 0)
        {
            Console.WriteLine("-> Викликано конструктор за замовчуванням (через this())");
        }

        public Laptop(string brand, string model, decimal price)
        {
            Brand = brand;
            Model = model;
            Price = price;
            Console.WriteLine($"-> Викликано параметризований конструктор для {Brand} {Model}");
        }

        public string GetInfo()
        {
            return $"Ноутбук: {Brand} {Model} | Ціна: {_price:C}";
        }

        ~Laptop()
        {
            Console.WriteLine($"[Деструктор]: Об'єкт Laptop ({_brand} {_model}) знищено з пам'яті.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Створення об'єктів ===");

            Laptop laptop1 = new Laptop();
            Console.WriteLine(laptop1.GetInfo());
            Console.WriteLine();

            Laptop laptop2 = new Laptop("ASUS", "ROG Zephyrus", 55000);
            Console.WriteLine(laptop2.GetInfo());
            Console.WriteLine();

            Laptop laptop3 = new Laptop("Lenovo", "IdeaPad", -15000);
            Console.WriteLine(laptop3.GetInfo());
            Console.WriteLine();

            Console.WriteLine("=== Завершення роботи Main та виклик GC ===");

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("=== Програму завершено ===");
        }
    }
}