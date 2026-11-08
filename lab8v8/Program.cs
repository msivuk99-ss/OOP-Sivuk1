using System;
using System.Collections.Generic;

namespace lab8v8;

class Program
{
    static void Main(string[] args)
    {
        List<int> numbers = new List<int> { 1, 5, -3, 8, 12 };
        bool hasNegative = CollectionHelper.Any(numbers, x => x < 0);
        Console.WriteLine($"Перевірка List<int> (чи є від'ємні числа): {hasNegative}");

        List<Product> products = new List<Product>
        {
            new Product("Ноутбук", "Електроніка", 25000),
            new Product("Футболка", "Одяг", 500),
            new Product("Мишка", "Електроніка", 800)
        };

        bool hasElectronics = CollectionHelper.Any(products, p => p.Category == "Електроніка");
        Console.WriteLine($"Перевірка List<Product> (чи є категорія 'Електроніка'): {hasElectronics}");

        bool hasBooks = CollectionHelper.Any(products, p => p.Category == "Книги");
        Console.WriteLine($"Перевірка List<Product> (чи є категорія 'Книги'): {hasBooks}");
    }
}