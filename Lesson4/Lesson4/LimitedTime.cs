namespace Lesson4;

public class LimitedTime(int m, int n, double lambda, double mu, double r)
{
    public int M { get; } = m;
    public int N { get; } = n;
    public double Lambda { get; } = lambda;
    public double Mu { get; } = mu;
    public double Ro { get; } = lambda / mu;
    public double R { get; } = r;
    
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

        var a = N / Ro;
        var b = Lambda * R;
        var c = Math.Pow(Math.E, b * (1 - a));

        double B;
        if (Math.Abs(Ro - N) < 0.001)
            B = 1 + b;
        else
            B = (c - 1) / (1 - a);
        
        p0 += Math.Pow(Ro, N) / fact * B;
        p0 = Math.Pow(p0, -1);
        
        return p0 < 0 ? 0 : p0;
    }

    public double GetPDecline()
    {
        var a = N / Ro;
        var b = Lambda * R;
        var c = Math.Pow(Math.E, b * (1 - a));
        
        return Math.Pow(Ro, N) / GetFact(N) * c * GetP0();
    }

    public double GetPBusy()
    {
        return -1;
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
        var a = N / Ro;
        var b = Lambda * R;
        var c = Math.Pow(Math.E, b * (1 - a));

        double D;

        if (Ro == N)
            D = 1 + b / 2;
        else
            D = (a - c * (a + b * (a - 1))) / (b * (a - 1) * (a - 1));
        
        return R * Math.Pow(Ro, N) / GetFact(N) * GetP0() * D;
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