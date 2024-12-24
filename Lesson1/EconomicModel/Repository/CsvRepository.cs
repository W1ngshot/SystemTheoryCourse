using System.Globalization;
using EconomicModel.Models;

namespace EconomicModel.Repository;

public static class CsvRepository
{
    public static List<MatrixRatio> GetRatio()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, $"../../../../direct_cost_ratio.csv");
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не найден: {filePath}");

        var result = new List<MatrixRatio>();

        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',');

            if (parts.Length != 3)
                throw new FormatException($"Некорректный формат строки: {line}");

            if (!int.TryParse(parts[0], out var producerId))
                throw new FormatException($"Некорректный ProducerId в строке: {line}");

            if (!int.TryParse(parts[1], out var consumerId))
                throw new FormatException($"Некорректный ConsumerId в строке: {line}");

            if (!double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var ratio))
                throw new FormatException($"Некорректный Ratio в строке: {line}");

            result.Add(new MatrixRatio
            {
                ProducerId = producerId,
                ConsumerId = consumerId,
                Ratio = ratio
            });
        }

        return result;
    }
    
    public static List<TotalConsumerConsumption> GetTotalConsumerConsumption()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, $"../../../../total_consumer_consumption.csv");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не найден: {filePath}");

        var result = new List<TotalConsumerConsumption>();

        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',');

            if (parts.Length != 2)
                throw new FormatException($"Некорректный формат строки: {line}");

            if (!int.TryParse(parts[0], out var companyId))
                throw new FormatException($"Некорректный CompanyId в строке: {line}");

            if (!double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var totalConsumed))
                throw new FormatException($"Некорректный TotalConsumed в строке: {line}");

            result.Add(new TotalConsumerConsumption
            {
                CompanyId = companyId,
                TotalConsumed = totalConsumed
            });
        }

        return result;
    }
}