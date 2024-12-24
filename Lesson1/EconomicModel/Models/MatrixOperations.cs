namespace EconomicModel.Models;

public static class MatrixOperations
{
    public static double[][] MatrixCreate(int rows, int cols)
    {
        var result = new double[rows][];
        for (var i = 0; i < rows; ++i)
            result[i] = new double[cols];
        return result;
    }

    public static double[][] MatrixDuplicate(double[][] originalMatrix)
    {
        ArgumentNullException.ThrowIfNull(originalMatrix);

        var rowCount = originalMatrix.Length;
        if (rowCount == 0)
            return [];

        var columnCount = originalMatrix[0].Length;

        var duplicateMatrix = new double[rowCount][];

        for (var i = 0; i < rowCount; i++)
        {
            duplicateMatrix[i] = new double[columnCount];
            for (var j = 0; j < columnCount; j++)
            {
                duplicateMatrix[i][j] = originalMatrix[i][j];
            }
        }

        return duplicateMatrix;
    }

    public static double[][] MatrixIdentity(int n)
    {
        var result = MatrixCreate(n, n);
        for (var i = 0; i < n; ++i)
            result[i][i] = 1.0;
        return result;
    }

    public static void WriteMatrix(double[][] matrix)
    {
        foreach (var row in matrix)
        {
            Console.WriteLine(string.Join(", ", row.Select(v => v.ToString("F5"))));
        }
    }

    public static double[][] MatrixSubtract(double[][] matrixA, double[][] matrixB)
    {
        if (matrixA == null || matrixB == null)
            throw new ArgumentNullException(matrixA == null ? nameof(matrixA) : nameof(matrixB));

        var rowCountA = matrixA.Length;
        var rowCountB = matrixB.Length;
        if (rowCountA != rowCountB)
            throw new ArgumentException("Матрицы должны иметь одинаковое количество строк.");

        var columnCountA = matrixA[0].Length;
        var columnCountB = matrixB[0].Length;
        if (columnCountA != columnCountB)
            throw new ArgumentException("Матрицы должны иметь одинаковое количество столбцов.");

        var resultMatrix = new double[rowCountA][];
        for (var i = 0; i < rowCountA; i++)
        {
            resultMatrix[i] = new double[columnCountA];
            for (var j = 0; j < columnCountA; j++)
            {
                resultMatrix[i][j] = matrixA[i][j] - matrixB[i][j];
            }
        }

        return resultMatrix;
    }

    public static double[][] MatrixProduct(double[][] matrixA,
        double[][] matrixB)
    {
        var aRows = matrixA.Length;
        var aCols = matrixA[0].Length;
        var bRows = matrixB.Length;
        var bCols = matrixB[0].Length;
        if (aCols != bRows)
            throw new Exception("Non-conformable matrices in MatrixProduct");
        var result = MatrixCreate(aRows, bCols);
        Parallel.For(0, aRows, i =>
            {
                for (var j = 0; j < bCols; ++j)
                for (var k = 0; k < aCols; ++k)
                    result[i][j] += matrixA[i][k] * matrixB[k][j];
            }
        );
        return result;
    }


    private static double[][] MatrixDecompose(double[][] matrix,
        out int[] perm, out int toggle)
    {
        var n = matrix.Length;
        var result = MatrixDuplicate(matrix);
        perm = new int[n];
        for (var i = 0; i < n; ++i)
        {
            perm[i] = i;
        }

        toggle = 1;
        for (var j = 0; j < n - 1; ++j)
        {
            var colMax = Math.Abs(result[j][j]);
            var pRow = j;
            for (var i = j + 1; i < n; ++i)
            {
                if (result[i][j] > colMax)
                {
                    colMax = result[i][j];
                    pRow = i;
                }
            }

            if (pRow != j)
            {
                (result[pRow], result[j]) = (result[j], result[pRow]);
                (perm[pRow], perm[j]) = (perm[j], perm[pRow]);
                toggle = -toggle;
            }

            if (Math.Abs(result[j][j]) < 1.0E-20)
                return null;
            for (var i = j + 1; i < n; ++i)
            {
                result[i][j] /= result[j][j];
                for (var k = j + 1; k < n; ++k)
                    result[i][k] -= result[i][j] * result[j][k];
            }
        }

        return result;
    }

    private static double[] HelperSolve(double[][] luMatrix,
        double[] b)
    {
        var n = luMatrix.Length;
        var x = new double[n];
        b.CopyTo(x, 0);
        for (var i = 1; i < n; ++i)
        {
            var sum = x[i];
            for (var j = 0; j < i; ++j)
                sum -= luMatrix[i][j] * x[j];
            x[i] = sum;
        }

        x[n - 1] /= luMatrix[n - 1][n - 1];
        for (var i = n - 2; i >= 0; --i)
        {
            var sum = x[i];
            for (var j = i + 1; j < n; ++j)
                sum -= luMatrix[i][j] * x[j];
            x[i] = sum / luMatrix[i][i];
        }

        return x;
    }

    public static double[][] MatrixInverse(double[][] matrix)
    {
        var n = matrix.Length;
        var result = MatrixDuplicate(matrix);
        var lum = MatrixDecompose(matrix, out var perm, out _);
        if (lum == null)
            throw new Exception("Unable to compute inverse");
        var b = new double[n];
        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < n; ++j)
            {
                if (i == perm[j])
                    b[j] = 1.0;
                else
                    b[j] = 0.0;
            }

            var x = HelperSolve(lum, b);
            for (var j = 0; j < n; ++j)
                result[j][i] = x[j];
        }

        return result;
    }
}