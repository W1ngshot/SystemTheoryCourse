namespace Lesson4;

public class WithoutQueue(int m, int n, double lambda, double mu)
{
    public int M { get; } = m;
    public int N { get; } = n;
    public double Lambda { get; } = lambda;
    public double Mu { get; } = mu;
    public double Ro { get; } = lambda / mu;
    
    public double GetP0()
    {
        var p0 = 0d;

        var fact = 1;
        for (var i = 0; i <= N; i++)
        {
            if (i != 0)
                fact *= i;

            p0 += 1.0 * Math.Pow(Ro, i) / fact;
        }

        p0 = Math.Pow(p0, -1);
        return p0;
    }

    public double GetPDecline()
    {
        return Math.Pow(Ro, N) / GetFact(N) * GetP0();
    }

    public double GetPBusy()
    {
        return GetPDecline();
    }

    public double GetQ()
    {
        return 1 - GetPDecline();
    }

    public double GetLambdaEffective()
    {
        return Lambda * GetQ();
    }

    public double GetL0()
    {
        return 0;
    }

    public double GetW0()
    {
        return 0;
    }

    public double GetK()
    {
        return GetLambdaEffective() / Mu;
    }

    public double GetLc()
    {
        return GetL0() + GetK();
    }
    
    private long GetFact(int n) => Enumerable.Range(1, n).Aggregate(1, (a, b) => a * b);
}