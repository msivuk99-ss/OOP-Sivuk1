using System;
using System.Collections.Generic;

namespace lab6v8
{
    public interface IParser<T>
    {
        T Parse(string input);
    }

    public class DateParser : IParser<DateTime>
    {
        public DateTime Parse(string input)
        {
            if (DateTime.TryParse(input, out DateTime result))
            {
                return result.Date;
            }
            throw new FormatException($"Не вдалося перетворити '{input}' у формат Date.");
        }
    }

    public class TimeParser : IParser<DateTime>
    {
        public DateTime Parse(string input)
        {
            if (DateTime.TryParse(input, out DateTime result))
            {
                return result;
            }
            throw new FormatException($"Не вдалося перетворити '{input}' у формат Time.");
        }
    }

    public abstract class Renderer
    {
        public abstract void RenderHeader();
        public abstract void RenderBody();

        public void RenderFooter()
        {
            Console.WriteLine("--- Підніжжя документу (Footer): Згенеровано автоматично ---");
        }

        public void RenderDocument()
        {
            RenderHeader();
            RenderBody();
            RenderFooter();
        }
    }

    public class HtmlRenderer : Renderer
    {
        public override void RenderHeader()
        {
            Console.WriteLine("<header><h1>HTML Заголовок Документа</h1></header>");
        }

        public override void RenderBody()
        {
            Console.WriteLine("<main><p>Це основне тіло документу в форматі HTML.</p></main>");
        }
    }

    public class MarkdownRenderer : Renderer
    {
        public override void RenderHeader()
        {
            Console.WriteLine("# Markdown Заголовок Документа");
        }

        public override void RenderBody()
        {
            Console.WriteLine("Це основне тіло документу в форматі **Markdown**.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== 1. Демонстрація поліморфізму через Інтерфейс (IParser) ===\n");

            List<IParser<DateTime>> parsers = new List<IParser<DateTime>>
            {
                new DateParser(),
                new TimeParser()
            };

            string dateInput = "2026-10-24";
            string timeInput = "14:30:00";

            foreach (var parser in parsers)
            {
                if (parser is DateParser)
                {
                    DateTime date = parser.Parse(dateInput);
                    Console.WriteLine($"[DateParser] Результат обробки '{dateInput}': {date:yyyy-MM-dd}");
                }
                else if (parser is TimeParser)
                {
                    DateTime time = parser.Parse(timeInput);
                    Console.WriteLine($"[TimeParser] Результат обробки '{timeInput}': {time:HH:mm:ss}");
                }
            }

            Console.WriteLine("\n=== 2. Демонстрація поліморфізму через Абстрактний клас (Renderer) ===\n");

            List<Renderer> renderers = new List<Renderer>
            {
                new HtmlRenderer(),
                new MarkdownRenderer()
            };

            foreach (var renderer in renderers)
            {
                Console.WriteLine($"--- Відображення за допомогою: {renderer.GetType().Name} ---");
                renderer.RenderDocument();
                Console.WriteLine();
            }
        }
    }
}