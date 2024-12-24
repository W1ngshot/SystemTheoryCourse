using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

static List<Record> ReadCsv(string filePath)
{
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = false
    };

    using var reader = new StreamReader(filePath);
    using var csv = new CsvReader(reader, config);
    var records = new List<Record>();

    while (csv.Read())
    {
        var timestampString = csv.GetField<string>(0);
        if (DateTime.TryParse(timestampString, null, DateTimeStyles.RoundtripKind, out var timestamp))
        {
            records.Add(new Record { Timestamp = timestamp });
        }
    }

    return records;
}

var records = ReadCsv("order_time.csv");

var filteredRecords = records
    .Where(r => r.Timestamp >= new DateTime(2024, 1, 1) && r.Timestamp < new DateTime(2024, 2, 1))
    .Select(r => new { Timestamp = r.Timestamp, Count = 1 })
    .ToList();

//1440
var timeIntervals = new List<TimeSpan>();
for (var i = 5; i <= 10080; i+=5)
{
    timeIntervals.Add(TimeSpan.FromMinutes(i));
}
/*
var timeIntervals = new List<TimeSpan>
{
    TimeSpan.FromMinutes(5),
    TimeSpan.FromMinutes(10),
    TimeSpan.FromMinutes(15),
    TimeSpan.FromMinutes(30),
    TimeSpan.FromHours(1),
    TimeSpan.FromHours(2),
    TimeSpan.FromHours(3),
    TimeSpan.FromHours(6),
    TimeSpan.FromHours(12),
    TimeSpan.FromDays(1)
};
*/
var minVariance = double.MaxValue;
var minVarianceMean = 0d;
var bestInterval = TimeSpan.Zero;

foreach (var interval in timeIntervals)
{
    var groupedRecords = filteredRecords
        .GroupBy(r => new DateTime((r.Timestamp.Ticks / interval.Ticks) * interval.Ticks)) // Округление времени
        .Select(g => g.Count())
        .ToList();

    var mean = groupedRecords.Average();
    var variance = groupedRecords
        .Select(x => Math.Pow(x - mean, 2))
        .Sum() / (groupedRecords.Count - 1);

    Console.WriteLine($"Интервал: {interval}");
    //Console.WriteLine($"Количество заявок: {string.Join(", ", groupedRecords)}");
    Console.WriteLine($"Средняя для {interval}: {mean}");
    Console.WriteLine($"Дисперсия для {interval}: {variance}\n");

    // Проверка на минимальную дисперсию
    if (variance < minVariance)
    {
        minVariance = variance;
        minVarianceMean = mean;
        bestInterval = interval;
    }
}

Console.WriteLine($"Минимальная дисперсия для интервала {bestInterval}: {minVariance}");
Console.WriteLine($"Выборочная средняя для минимальной дисперсии: {minVarianceMean}");

public class Record
{
    public DateTime Timestamp { get; set; }
}