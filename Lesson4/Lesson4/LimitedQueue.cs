namespace Lesson4;

public class LimitedQueue(int m, int n, double lambda, double mu)
{
    public int M { get; } = m;
    public int N { get; } = n;
    public double Lambda { get; } = lambda;
    public double Mu { get; } = mu;
    public double Ro { get; } = lambda / mu;

    public double GetP0()
    {
        var alpha = Ro / N;
        var b = 1.0 * alpha / (1 - alpha) * (1 - Math.Pow(alpha, M));

        var p0 = 0d;

        var fact = 1;
        for (var i = 0; i <= N; i++)
        {
            if (i != 0)
                fact *= i;

            p0 += 1.0 * Math.Pow(Ro, i) / fact;
        }

        p0 += 1.0 * Math.Pow(Ro, N) / fact * b;
        p0 = Math.Pow(p0, -1);
        return p0;
    }

    public double GetPDecline()
    {
        var p0 = GetP0();
        
        var pDecline = Math.Pow(Ro, M + N) / GetFact(N) / Math.Pow(N, M) * p0;

        return pDecline;
    }

    public double GetQ()
    {
        return 1 - GetPDecline();
    }

    public double GetLambdaEffective()
    {
        return GetQ() * Lambda;
    }

    public double GetL0()
    {
        var alpha = Ro / N;
        
        var numerator = alpha * (1 + (M * alpha - M - 1) * Math.Pow(alpha, M));
        var denominator = Math.Pow(1 - alpha, 2);
        var d = numerator / denominator;
        
        return Math.Pow(Ro, N) / GetFact(N) * GetP0() * d;
    }

    public double GetW0()
    {
        return GetL0() / GetLambdaEffective();
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