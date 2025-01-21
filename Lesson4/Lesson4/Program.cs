using Lesson4;
/*
Console.WriteLine("Enter n: ");
var n = int.Parse(Console.ReadLine()!);
Console.WriteLine("Enter m: ");
var m = int.Parse(Console.ReadLine()!);
Console.WriteLine("Enter lambda: ");
var lambda = double.Parse(Console.ReadLine()!);
Console.WriteLine("Enter mu: ");
var mu = double.Parse(Console.ReadLine()!);
*/

var n = 1;
var lambda = 3d;
var mu = 0.5d;
var r = 0.5d;

var result = new LimitedTime(0, n, lambda, mu, r);
Console.WriteLine($"Po: {result.GetP0()}");
Console.WriteLine($"P decline: {result.GetPDecline()}");
Console.WriteLine($"lambda effective: {result.GetLambdaEffective()}");
Console.WriteLine($"Wo: {result.GetW0()}");
Console.WriteLine($"K: {result.GetK()}");
Console.WriteLine();

for (var i = 0; i < 20; i++)
{
    var model = new LimitedTime(0, i, lambda, mu, r);

    Console.WriteLine($"N: {i}");
    Console.WriteLine($"Lambda eff {model.GetLambdaEffective()}");
    var cost = 5000 * model.GetLambdaEffective() * 16 * 30 - 280000 * i;
    Console.WriteLine($"Cost: {cost}");
    Console.WriteLine();
}