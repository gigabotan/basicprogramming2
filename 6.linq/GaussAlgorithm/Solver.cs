using System;
using System.Linq;

namespace GaussAlgorithm;

public class Solver
{
    public double[] Solve(double[][] matrix, double[] freeMembers)
    {
        int m = matrix.Length;
        if (m == 0) return [];

        int n = matrix[0].Length;

        if (matrix.Any(row => row.Length != n))
            throw new ArgumentException("All matrix rows must have the same length");
        if (freeMembers.Length != m)
            throw new ArgumentException("Matrix rows and free members size mismatch");

        int[] pivotCol = Enumerable.Repeat(-1, m).ToArray();
        int currentRow = 0;

        for (int col = 0; col < n && currentRow < m; col++)
        {
            int maxRow = Enumerable.Range(currentRow, m - currentRow)
                .MaxBy(k => Math.Abs(matrix[k][col]));

            if (Math.Abs(matrix[maxRow][col]) < 1e-10) continue;

            (matrix[currentRow], matrix[maxRow]) = (matrix[maxRow], matrix[currentRow]);
            (freeMembers[currentRow], freeMembers[maxRow]) = (freeMembers[maxRow], freeMembers[currentRow]);

            pivotCol[currentRow] = col;

            foreach (int j in Enumerable.Range(0, m).Where(j => j != currentRow))
            {
                double factor = matrix[j][col] / matrix[currentRow][col];
                for (int c = col; c < n; c++)
                    matrix[j][c] -= factor * matrix[currentRow][c];
                freeMembers[j] -= factor * freeMembers[currentRow];
            }

            currentRow++;
        }

        if (Enumerable.Range(currentRow, m - currentRow)
            .Any(i => matrix[i].All(v => Math.Abs(v) < 1e-10)
                   && Math.Abs(freeMembers[i]) > 1e-10))
            throw new NoSolutionException("System has no solution");

        double[] x = new double[n];
        for (int i = currentRow - 1; i >= 0; i--)
        {
            int col = pivotCol[i];
            x[col] = (freeMembers[i] - Enumerable.Range(col + 1, n - col - 1)
                .Sum(j => matrix[i][j] * x[j])) / matrix[i][col];
        }

        return x;
    }
}