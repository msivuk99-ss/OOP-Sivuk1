using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var teams = new List<Team>
{
    new Team { Id = 1, Name = "Lions", City = "Lviv" },
    new Team { Id = 2, Name = "Eagles", City = "Kyiv" },
    new Team { Id = 3, Name = "Wolves", City = "Kharkiv" },
    new Team { Id = 4, Name = "Bears", City = "Dnipro" },
    new Team { Id = 5, Name = "Tigers", City = "Odesa" },
    new Team { Id = 6, Name = "Falcons", City = "Rivne" },
    new Team { Id = 7, Name = "Sharks", City = "Vinnytsia" },
    new Team { Id = 8, Name = "Panthers", City = "Poltava" },
    new Team { Id = 9, Name = "Hawks", City = "Lutsk" },
    new Team { Id = 10, Name = "Ravens", City = "Ternopil" }
};

var players = new List<Player>
{
    new Player { Id = 1, Name = "Олександр Коваленко", TeamId = 1, Age = 21 },
    new Player { Id = 2, Name = "Максим Бондаренко", TeamId = 1, Age = 24 },
    new Player { Id = 3, Name = "Артем Ткаченко", TeamId = 1, Age = 28 },
    new Player { Id = 4, Name = "Денис Мельник", TeamId = 1, Age = 19 },
    new Player { Id = 5, Name = "Іван Шевченко", TeamId = 2, Age = 22 },
    new Player { Id = 6, Name = "Дмитро Кравченко", TeamId = 2, Age = 31 },
    new Player { Id = 7, Name = "Владислав Олійник", TeamId = 3, Age = 23 },
    new Player { Id = 8, Name = "Богдан Поліщук", TeamId = 3, Age = 27 },
    new Player { Id = 9, Name = "Назар Савченко", TeamId = 4, Age = 20 },
    new Player { Id = 10, Name = "Роман Лисенко", TeamId = 4, Age = 33 },
    new Player { Id = 11, Name = "Тарас Мороз", TeamId = 5, Age = 25 },
    new Player { Id = 12, Name = "Юрій Петренко", TeamId = 6, Age = 29 },
    new Player { Id = 13, Name = "Віталій Руденко", TeamId = 7, Age = 21 },
    new Player { Id = 14, Name = "Сергій Сидоренко", TeamId = 8, Age = 30 },
    new Player { Id = 15, Name = "Андрій Марченко", TeamId = 9, Age = 24 },
    new Player { Id = 16, Name = "Олег Павленко", TeamId = 10, Age = 26 }
};

Console.WriteLine("=== Запит 1: Гравці віком до 25 років з команди Lions ===");

var sw1Query = Stopwatch.StartNew();
var youngLionsQuery = (from p in players
                       join t in teams on p.TeamId equals t.Id
                       where t.Name == "Lions" && p.Age < 25
                       select p).ToList();
sw1Query.Stop();

Console.WriteLine($"[Синтаксис запитів] Час: {sw1Query.Elapsed.TotalMilliseconds:F4} мс ({sw1Query.ElapsedTicks} тіків)");
foreach (var p in youngLionsQuery)
{
    Console.WriteLine($" - {p.Name}, вік: {p.Age}");
}

var sw1Method = Stopwatch.StartNew();
var youngLionsMethod = players
    .Join(teams, p => p.TeamId, t => t.Id, (p, t) => new { Player = p, Team = t })
    .Where(x => x.Team.Name == "Lions" && x.Player.Age < 25)
    .Select(x => x.Player)
    .ToList();
sw1Method.Stop();

Console.WriteLine($"[Синтаксис методів] Час: {sw1Method.Elapsed.TotalMilliseconds:F4} мс ({sw1Method.ElapsedTicks} тіків)");
foreach (var p in youngLionsMethod)
{
    Console.WriteLine($" - {p.Name}, вік: {p.Age}");
}

Console.WriteLine("\n=== Запит 2: Об'єднання гравців з їхніми командами ===");

var sw2Query = Stopwatch.StartNew();
var playerTeamsQuery = (from p in players
                        join t in teams on p.TeamId equals t.Id
                        select new { PlayerName = p.Name, TeamName = t.Name }).ToList();
sw2Query.Stop();

Console.WriteLine($"[Синтаксис запитів] Час: {sw2Query.Elapsed.TotalMilliseconds:F4} мс ({sw2Query.ElapsedTicks} тіків)");
foreach (var item in playerTeamsQuery)
{
    Console.WriteLine($" - {item.PlayerName} -> {item.TeamName}");
}

var sw2Method = Stopwatch.StartNew();
var playerTeamsMethod = players
    .Join(teams, p => p.TeamId, t => t.Id, (p, t) => new { PlayerName = p.Name, TeamName = t.Name })
    .ToList();
sw2Method.Stop();

Console.WriteLine($"[Синтаксис методів] Час: {sw2Method.Elapsed.TotalMilliseconds:F4} мс ({sw2Method.ElapsedTicks} тіків)");
foreach (var item in playerTeamsMethod)
{
    Console.WriteLine($" - {item.PlayerName} -> {item.TeamName}");
}

Console.WriteLine("\n=== Запит 3: Середній вік гравців у кожній команді ===");

var sw3Query = Stopwatch.StartNew();
var avgAgeQuery = (from p in players
                   join t in teams on p.TeamId equals t.Id
                   group p by t.Name into teamGroup
                   select new
                   {
                       TeamName = teamGroup.Key,
                       AverageAge = teamGroup.Average(p => p.Age)
                   }).ToList();
sw3Query.Stop();

Console.WriteLine($"[Синтаксис запитів] Час: {sw3Query.Elapsed.TotalMilliseconds:F4} мс ({sw3Query.ElapsedTicks} тіків)");
foreach (var item in avgAgeQuery)
{
    Console.WriteLine($" - {item.TeamName}: {item.AverageAge:F1} років");
}

var sw3Method = Stopwatch.StartNew();
var avgAgeMethod = players
    .Join(teams, p => p.TeamId, t => t.Id, (p, t) => new { TeamName = t.Name, p.Age })
    .GroupBy(x => x.TeamName)
    .Select(g => new
    {
        TeamName = g.Key,
        AverageAge = g.Average(x => x.Age)
    })
    .ToList();
sw3Method.Stop();

Console.WriteLine($"[Синтаксис методів] Час: {sw3Method.Elapsed.TotalMilliseconds:F4} мс ({sw3Method.ElapsedTicks} тіків)");
foreach (var item in avgAgeMethod)
{
    Console.WriteLine($" - {item.TeamName}: {item.AverageAge:F1} років");
}

Console.WriteLine("\n=== Запит 4: Найстарший гравець ===");

var sw4Query = Stopwatch.StartNew();
var oldestPlayerQuery = (from p in players
                         orderby p.Age descending
                         select p).FirstOrDefault();
sw4Query.Stop();

Console.WriteLine($"[Синтаксис запитів] Час: {sw4Query.Elapsed.TotalMilliseconds:F4} мс ({sw4Query.ElapsedTicks} тіків)");
if (oldestPlayerQuery != null)
{
    Console.WriteLine($" - {oldestPlayerQuery.Name}, вік: {oldestPlayerQuery.Age}");
}

var sw4Method = Stopwatch.StartNew();
var oldestPlayerMethod = players.MaxBy(p => p.Age);
sw4Method.Stop();

Console.WriteLine($"[Синтаксис методів] Час: {sw4Method.Elapsed.TotalMilliseconds:F4} мс ({sw4Method.ElapsedTicks} тіків)");
if (oldestPlayerMethod != null)
{
    Console.WriteLine($" - {oldestPlayerMethod.Name}, вік: {oldestPlayerMethod.Age}");
}

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public int Age { get; set; }
}