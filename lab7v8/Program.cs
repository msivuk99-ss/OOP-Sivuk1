using System;
using System.Collections.Generic;
using System.Linq;

namespace lab7v8
{
    // Інтерфейс сутності
    public interface IEntity
    {
        int Id { get; set; }
    }

    // Сутність TaskItem
    public class TaskItem : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        public TaskItem() { }

        public TaskItem(int id, string title, string description, bool isCompleted = false)
        {
            Id = id;
            Title = title;
            Description = description;
            IsCompleted = isCompleted;
        }
    }

    // Інтерфейс репозиторію
    public interface IRepository<T> where T : IEntity
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }

    // Реалізація репозиторію
    public class InMemoryRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly List<T> _items = new List<T>();

        public T? GetById(int id)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return _items.ToList();
        }

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_items.Any(x => x.Id == item.Id))
                throw new InvalidOperationException($"Елемент з ID {item.Id} вже існує.");

            _items.Add(item);
        }

        public void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var existingIndex = _items.FindIndex(x => x.Id == item.Id);
            if (existingIndex == -1)
                throw new KeyNotFoundException($"Елемент з ID {item.Id} не знайдено.");

            _items[existingIndex] = item;
        }

        public void Delete(int id)
        {
            var existingItem = GetById(id);
            if (existingItem == null)
                throw new KeyNotFoundException($"Елемент з ID {id} не знайдено.");

            _items.Remove(existingItem);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IRepository<TaskItem> taskRepo = new InMemoryRepository<TaskItem>();

            Console.WriteLine("=== 1. Додавання елементів ===");
            taskRepo.Add(new TaskItem(1, "Лабораторна 7", "Виконати завдання з Generics", false));
            taskRepo.Add(new TaskItem(2, "Купити продукты", "Хліб, молоко, яблука", true));

            foreach (var task in taskRepo.GetAll())
            {
                Console.WriteLine($"[ID: {task.Id}] {task.Title} - {task.Description} (Завершено: {task.IsCompleted})");
            }

            Console.WriteLine("\n=== 2. Оновлення елемента ===");
            var updatedTask = new TaskItem(1, "Лабораторна 7", "Здати роботу викладачу", true);
            taskRepo.Update(updatedTask);
            var item = taskRepo.GetById(1);
            if (item != null)
            {
                Console.WriteLine($"Оновлено ID 1: {item.Title} (Завершено: {item.IsCompleted})");
            }

            Console.WriteLine("\n=== 3. Видалення елемента ===");
            taskRepo.Delete(2);
            Console.WriteLine($"Кількість елементів після видалення ID 2: {taskRepo.GetAll().Count()}");
        }
    }
}