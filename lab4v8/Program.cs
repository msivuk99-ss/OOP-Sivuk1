using System;

namespace lab6v8
{
    public class Device
    {
        private string brand;
        private string model;

        public string Brand
        {
            get => brand;
            set => brand = value;
        }

        public string Model
        {
            get => model;
            set => model = value;
        }

        public Device(string brand, string model)
        {
            this.brand = brand;
            this.model = model;
        }

        public virtual void PowerOn()
        {
            Console.WriteLine($"Пристрій {Brand} {Model} вмикається...");
        }

        public string GetDeviceType()
        {
            return "Загальний пристрій (Device)";
        }
    }

    public class Smartphone : Device
    {
        public double ScreenSize { get; set; }

        public Smartphone(string brand, string model, double screenSize)
            : base(brand, model)
        {
            ScreenSize = screenSize;
        }

        public override void PowerOn()
        {
            Console.WriteLine($"Смартфон {Brand} {Model} з екраном {ScreenSize}\" завантажує ОС...");
        }

        public void MakeCall()
        {
            Console.WriteLine($"Здійснюється дзвінок зі смартфона {Brand} {Model}...");
        }

        public new string GetDeviceType()
        {
            return "Смартфон (Smartphone)";
        }
    }

    public class Laptop : Device
    {
        public string Processor { get; set; }

        public Laptop(string brand, string model, string processor)
            : base(brand, model)
        {
            Processor = processor;
        }

        public override void PowerOn()
        {
            Console.WriteLine($"Ноутбук {Brand} {Model} ({Processor}) запускає системні служби...");
        }

        public void RunProgram()
        {
            Console.WriteLine($"Запуск важкої програми на ноутбуці {Brand} {Model}...");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== 1. Створення об'єктів ===");
            Device baseDevice = new Device("Generic", "D-100");
            Smartphone phone = new Smartphone("Apple", "iPhone 15", 6.1);
            Laptop laptop = new Laptop("ASUS", "ROG Zephyrus", "Intel Core i9");

            Console.WriteLine("\n=== 2. Поліморфічна поведінка (виклики через посилання на базовий клас Device) ===");
            Device[] devices = new Device[] { baseDevice, phone, laptop };
            foreach (var dev in devices)
            {
                dev.PowerOn();
            }

            Console.WriteLine("\n=== 3. Виклик унікальних методів ===");
            phone.MakeCall();
            laptop.RunProgram();

            Console.WriteLine("\n=== 4. Демонстрація різниці між override та new ===");

            Console.WriteLine($"Тип через посилання Smartphone: {phone.GetDeviceType()}");

            Device phoneAsDevice = phone;
            Console.WriteLine($"Тип через посилання Device (приховування new): {phoneAsDevice.GetDeviceType()}");

            Console.Write("PowerOn() через посилання Device: ");
            phoneAsDevice.PowerOn();
        }
    }
}