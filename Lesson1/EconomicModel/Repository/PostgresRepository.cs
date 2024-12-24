using Dapper;
using EconomicModel.Models;
using Npgsql;

namespace EconomicModel.Repository;

public class PostgresRepository(string connectionString)
{
    private NpgsqlConnection GetDbConnection() => new(connectionString);

    public async Task<List<MatrixRatio>> GetRatioAsync()
    {
        var sqlQuery = LoadSqlQuery("direct_cost_ratio.sql");

        await using var connection = GetDbConnection();
        await connection.OpenAsync();

        return connection.Query<MatrixRatio>(sqlQuery).ToList();
    }
    
    public async Task<List<TotalConsumerConsumption>> GetTotalConsumerConsumptionAsync()
    {
        var sqlQuery = LoadSqlQuery("total_consumer_consumption.sql");

        await using var connection = GetDbConnection();
        await connection.OpenAsync();

        return connection.Query<TotalConsumerConsumption>(sqlQuery).ToList();
    }

    private static string LoadSqlQuery(string resourceName)
    {
        var pathToFile = Path.Combine(AppContext.BaseDirectory, $"../../../../{resourceName}");
        return File.ReadAllText(pathToFile);
    }
}