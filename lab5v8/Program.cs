using System;
using System.Collections.Generic;

namespace lab5v8
{
    public class Transaction
    {
        public decimal Amount { get; set; }

        public Transaction(decimal amount)
        {
            Amount = amount;
        }

        public virtual void Execute()
        {
            Console.WriteLine($"Виконується базова транзакція на суму: {Amount:C}");
        }
    }

    public class Deposit : Transaction
    {
        public string DestinationAccount { get; set; }

        public Deposit(decimal amount, string destinationAccount)
            : base(amount)
        {
            DestinationAccount = destinationAccount;
        }

        public override void Execute()
        {
            Console.WriteLine($"[Депозит] Поповнено рахунок {DestinationAccount} на суму {Amount:C}");
        }
    }

    public class Withdrawal : Transaction
    {
        public string SourceAccount { get; set; }

        public Withdrawal(decimal amount, string sourceAccount)
            : base(amount)
        {
            SourceAccount = sourceAccount;
        }

        public override void Execute()
        {
            Console.WriteLine($"[Зняття] Знято кошти з рахунку {SourceAccount} на суму {Amount:C}");
        }
    }

    public class Transfer : Transaction
    {
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }

        public Transfer(decimal amount, string fromAccount, string toAccount)
            : base(amount)
        {
            FromAccount = fromAccount;
            ToAccount = toAccount;
        }

        public override void Execute()
        {
            Console.WriteLine($"[Переказ] Переказано {Amount:C} з рахунку {FromAccount} на рахунок {ToAccount}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Transaction> transactions = new List<Transaction>
            {
                new Deposit(1500.50m, "UA1234567890"),
                new Withdrawal(200.00m, "UA1234567890"),
                new Transfer(500.75m, "UA1234567890", "UA0987654321"),
                new Deposit(3000.00m, "UA0987654321"),
                new Withdrawal(150.25m, "UA0987654321")
            };

            Console.WriteLine("=== Виконання транзакцій (Поліморфізм) ===");

            foreach (var tx in transactions)
            {
                tx.Execute();
            }

            decimal totalAmount = 0;
            foreach (var tx in transactions)
            {
                totalAmount += tx.Amount;
            }

            Console.WriteLine("\n=== Результати агрегації ===");
            Console.WriteLine($"Всього оброблено транзакцій: {transactions.Count}");
            Console.WriteLine($"Загальна сума всіх транзакцій: {totalAmount:C}");
        }
    }
}