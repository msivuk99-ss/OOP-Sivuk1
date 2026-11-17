using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace lab9v8
{
    public class Transaction
    {
        public string Id { get; }
        public string Sender { get; }
        public string Receiver { get; }
        public decimal Amount { get; }

        public Transaction(string id, string sender, string receiver, decimal amount)
        {
            Id = id;
            Sender = sender;
            Receiver = receiver;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"[{Id}] {Sender} -> {Receiver}: {Amount:F2} UAH";
        }
    }

    public class TransactionManager
    {
        private readonly HashSet<string> _uniqueIds = new HashSet<string>();
        private readonly Queue<Transaction> _pendingQueue = new Queue<Transaction>();
        private readonly Stack<Transaction> _completedHistory = new Stack<Transaction>();

        public int PendingCount => _pendingQueue.Count;
        public int HistoryCount => _completedHistory.Count;

        public void AddTransaction(Transaction transaction)
        {
            if (_uniqueIds.Add(transaction.Id))
            {
                _pendingQueue.Enqueue(transaction);
                Console.WriteLine($"Додано до реєстру та черги: {transaction}");
            }
            else
            {
                Console.WriteLine($"Відхилено дублікат ID: {transaction.Id}");
            }
        }

        public void CheckIdExists(string id)
        {
            bool exists = _uniqueIds.Contains(id);
            Console.WriteLine($"Перевірка наявності ID '{id}' у HashSet: {(exists ? "знайдено" : "не знайдено")}");
        }

        public void RemoveId(string id)
        {
            bool removed = _uniqueIds.Remove(id);
            Console.WriteLine($"Видалення ID '{id}' з HashSet: {(removed ? "успішно" : "ID відсутній")}");
        }

        public void FindDuplicateBatchIds(IEnumerable<string> externalBatch)
        {
            var commonIds = new HashSet<string>(_uniqueIds);
            commonIds.IntersectWith(externalBatch);
            Console.WriteLine($"Спільні ID із зовнішнім пакетом (IntersectWith): {string.Join(", ", commonIds)}");
        }

        public void PeekQueue()
        {
            if (_pendingQueue.Count > 0)
            {
                Console.WriteLine($"Перша транзакція в черзі (Peek): {_pendingQueue.Peek()}");
            }
            else
            {
                Console.WriteLine("Черга порожня.");
            }
        }

        public void ProcessNext()
        {
            if (_pendingQueue.Count > 0)
            {
                Transaction current = _pendingQueue.Dequeue();
                _completedHistory.Push(current);
                Console.WriteLine($"Оброблено транзакцію (FIFO Dequeue -> Stack Push): {current}");
            }
            else
            {
                Console.WriteLine("Немає транзакцій для обробки.");
            }
        }

        public void PeekHistory()
        {
            if (_completedHistory.Count > 0)
            {
                Console.WriteLine($"Остання виконана транзакція на вершині стеку (Peek): {_completedHistory.Peek()}");
            }
            else
            {
                Console.WriteLine("Історія виконаних транзакцій порожня.");
            }
        }

        public void UndoLastTransaction()
        {
            if (_completedHistory.Count > 0)
            {
                Transaction reverted = _completedHistory.Pop();
                Console.WriteLine($"Відмінено транзакцію (LIFO Pop): {reverted}");
            }
            else
            {
                Console.WriteLine("Немає транзакцій для відміни.");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var manager = new TransactionManager();

            Console.WriteLine("=== 1. Робота з HashSet<T> (Унікальні ID транзакцій) ===");
            var t1 = new Transaction("TX-101", "Іван", "Олена", 1500.00m);
            var t2 = new Transaction("TX-102", "Олена", "Максим", 3200.50m);
            var t3 = new Transaction("TX-103", "Максим", "Анна", 750.00m);
            var tDuplicate = new Transaction("TX-101", "Іван", "Олена", 1500.00m);

            manager.AddTransaction(t1);
            manager.AddTransaction(t2);
            manager.AddTransaction(t3);
            manager.AddTransaction(tDuplicate);

            manager.CheckIdExists("TX-102");
            manager.CheckIdExists("TX-999");
            manager.FindDuplicateBatchIds(new[] { "TX-102", "TX-103", "TX-500" });
            manager.RemoveId("TX-999");

            Console.WriteLine("\n=== 2. Робота з Queue<T> (Черга на обробку - FIFO) ===");
            manager.PeekQueue();
            while (manager.PendingCount > 0)
            {
                manager.ProcessNext();
            }
            manager.PeekQueue();

            Console.WriteLine("\n=== 3. Робота з Stack<T> (Історія для відміни - LIFO) ===");
            manager.PeekHistory();
            while (manager.HistoryCount > 0)
            {
                manager.UndoLastTransaction();
            }
            manager.PeekHistory();

            Console.WriteLine("\n=== 4. Порівняння продуктивності пошуку: HashSet<T> проти List<T> ===");
            CompareSearchPerformance();
        }

        static void CompareSearchPerformance()
        {
            const int count = 100000;
            var list = new List<string>(count);
            var hashSet = new HashSet<string>(count);

            for (int i = 0; i < count; i++)
            {
                string id = $"TX-{i}";
                list.Add(id);
                hashSet.Add(id);
            }

            string target = $"TX-{count - 1}";

            var sw = Stopwatch.StartNew();
            bool foundInList = list.Contains(target);
            sw.Stop();
            long listTicks = sw.ElapsedTicks;

            sw.Restart();
            bool foundInSet = hashSet.Contains(target);
            sw.Stop();
            long setTicks = sw.ElapsedTicks;

            Console.WriteLine($"Пошук останнього елемента серед {count} записів:");
            Console.WriteLine($"List<string>.Contains: знайдено = {foundInList}, час = {listTicks} тіків");
            Console.WriteLine($"HashSet<string>.Contains: знайдено = {foundInSet}, час = {setTicks} тіків");
        }
    }
}