using EconomicModel.Models;
using EconomicModel.Repository;

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
try
{
    var repository =
        new PostgresRepository("Host=itislabs.ru;Username=tsisa110;Password=LeskaPU;Database=tsisa110;Port=5432");

    //var ratios = await repository.GetRatioAsync();
    var ratios = CsvRepository.GetRatio();
    foreach (var item in ratios)
    {
        Console.WriteLine($"Producer Id: {item.ProducerId}, Consumer Id: {item.ConsumerId}, Ratio: {item.Ratio}");
    }

    //var consumerConsumptions = await repository.GetTotalConsumerConsumptionAsync();
    var consumerConsumptions = CsvRepository.GetTotalConsumerConsumption();
    foreach (var item in consumerConsumptions)
    {
        Console.WriteLine($"Company Id: {item.CompanyId}, Total: {item.TotalConsumed}");
    }

    var size = ratios.Max(r => Math.Max(r.ProducerId, r.ConsumerId));

    var matrix = MatrixOperations.MatrixCreate(size, size);
    var consumptionMatrix = MatrixOperations.MatrixCreate(size, 1);

    foreach (var ratio in ratios)
    {
        matrix[ratio.ProducerId - 1][ratio.ConsumerId - 1] = ratio.Ratio;
    }
    Console.WriteLine("base matrix:");
    MatrixOperations.WriteMatrix(matrix);
    Console.WriteLine();

    foreach (var consumption in consumerConsumptions)
    {
        consumptionMatrix[consumption.CompanyId - 1][0] = consumption.TotalConsumed;
    }
    Console.WriteLine("consumption matrix:");
    MatrixOperations.WriteMatrix(consumptionMatrix);
    Console.WriteLine();

    var newConsumptionMatrix = MatrixOperations.MatrixDuplicate(consumptionMatrix);
    newConsumptionMatrix[0][0] = consumptionMatrix[0][0] * 2;
    Console.WriteLine("new consumption matrix:");
    MatrixOperations.WriteMatrix(newConsumptionMatrix);
    Console.WriteLine();
    
    var matrixIdentity = MatrixOperations.MatrixIdentity(size);
    Console.WriteLine("matrix identity:");
    MatrixOperations.WriteMatrix(matrixIdentity);
    Console.WriteLine();

    var matrixSubtract = MatrixOperations.MatrixSubtract(matrixIdentity, matrix);
    Console.WriteLine("matrix subtraction:");
    MatrixOperations.WriteMatrix(matrixSubtract);
    Console.WriteLine();

    var matrixInverse = MatrixOperations.MatrixInverse(matrixSubtract);
    Console.WriteLine("matrix inverse:");
    MatrixOperations.WriteMatrix(matrixInverse);
    Console.WriteLine();

    var result = MatrixOperations.MatrixProduct(matrixInverse, consumptionMatrix);
    Console.WriteLine("result matrix:");
    MatrixOperations.WriteMatrix(result);
    Console.WriteLine();
    
    var newResult = MatrixOperations.MatrixProduct(matrixInverse, newConsumptionMatrix);
    Console.WriteLine("new result matrix:");
    MatrixOperations.WriteMatrix(newResult);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}