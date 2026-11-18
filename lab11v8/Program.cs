using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var menus = new List<RestaurantMenu>
{
    new RestaurantMenu
    {
        Id = 1,
        Name = "Основне меню",
        Categories = new List<DishCategory>
        {
            new DishCategory
            {
                Id = 1,
                Name = "Перші страви",
                Dishes = new List<Dish>
                {
                    new Dish { Id = 1, Name = "Борщ український", Price = 140m },
                    new Dish { Id = 2, Name = "Бограч закарпатський", Price = 180m },
                    new Dish { Id = 3, Name = "Крем-суп грибний", Price = 130m }
                }
            },
            new DishCategory
            {
                Id = 2,
                Name = "Основні страви",
                Dishes = new List<Dish>
                {
                    new Dish { Id = 4, Name = "Банош з бринзою", Price = 160m },
                    new Dish { Id = 5, Name = "Вареники з картоплею", Price = 110m },
                    new Dish { Id = 6, Name = "Стейк зі свинини", Price = 270m }
                }
            }
        }
    },
    new RestaurantMenu
    {
        Id = 2,
        Name = "Італійська кухня",
        Categories = new List<DishCategory>
        {
            new DishCategory
            {
                Id = 3,
                Name = "Піца",
                Dishes = new List<Dish>
                {
                    new Dish { Id = 7, Name = "Піца Маргарита", Price = 210m },
                    new Dish { Id = 8, Name = "Піца Пепероні", Price = 250m },
                    new Dish { Id = 9, Name = "Піца Чотири сири", Price = 290m }
                }
            },
            new DishCategory
            {
                Id = 4,
                Name = "Паста",
                Dishes = new List<Dish>
                {
                    new Dish { Id = 10, Name = "Паста Карбонара", Price = 220m },
                    new Dish { Id = 11, Name = "Паста Болоньєзе", Price = 230m }
                }
            }
        }
    },
    new RestaurantMenu
    {
        Id = 3,
        Name = "Десерти та напої",
        Categories = new List<DishCategory>
        {
            new DishCategory
            {
                Id = 5,
                Name = "Десерти",
                Dishes = new List<Dish>
                {
                    new Dish { Id = 12, Name = "Сирник львівський", Price = 120m },
                    new Dish { Id = 13, Name = "Тірамісу", Price = 150m },
                    new Dish { Id = 14, Name = "Наполеон", Price = 110m }
                }
            },
            new DishCategory
            {
                Id = 6,
                Name = "Напої",
                Dishes = new List<Dish>
                {
                    new Dish { Id = 15, Name = "Узвар домашній", Price = 50m },
                    new Dish { Id = 16, Name = "Лимонад цитрусовий", Price = 90m }
                }
            }
        }
    }
};

Console.WriteLine("=== 1. SelectMany: Усі страви з усіх категорій усіх меню ===");
var allDishes = menus
    .SelectMany(menu => menu.Categories)
    .SelectMany(category => category.Dishes)
    .ToList();

Console.WriteLine($"Загальна кількість страв: {allDishes.Count}");
foreach (var dish in allDishes)
{
    Console.WriteLine($" - [{dish.Id}] {dish.Name} — {dish.Price:F2} грн");
}

Console.WriteLine("\n=== 2. Пагінація: Категорії страв по 2 на сторінку ===");
var allCategories = menus.SelectMany(menu => menu.Categories).ToList();
int pageSize = 2;
int totalPages = (int)Math.Ceiling((double)allCategories.Count / pageSize);

for (int page = 1; page <= totalPages; page++)
{
    var pageItems = allCategories
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    Console.WriteLine($"Сторінка {page} з {totalPages}:");
    foreach (var category in pageItems)
    {
        Console.WriteLine($" - {category.Name} (страв у категорії: {category.Dishes.Count})");
    }
}

Console.WriteLine("\n=== 3. Custom Extension: AverageDishPrice() ===");
Console.WriteLine($"Середня ціна всіх страв у ресторані: {allCategories.AverageDishPrice():F2} грн");

foreach (var menu in menus)
{
    Console.WriteLine($" - {menu.Name}: середня ціна страв = {menu.Categories.AverageDishPrice():F2} грн");
}

public class RestaurantMenu
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<DishCategory> Categories { get; set; } = new();
}

public class DishCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Dish> Dishes { get; set; } = new();
}

public class Dish
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public static class DishCategoryExtensions
{
    public static decimal AverageDishPrice(this IEnumerable<DishCategory> categories)
    {
        var dishes = categories.SelectMany(c => c.Dishes).ToList();
        return dishes.Count > 0 ? dishes.Average(d => d.Price) : 0m;
    }
}