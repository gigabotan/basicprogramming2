using System;
using System.Linq;

namespace GaussAlgorithm;

public class Solver
{
    private const double Epsilon = 1e-10;

    public double[] Solve(double[][] matrix, double[] freeMembers)
    {
        ValidateInput(matrix, freeMembers);

        var rowCount = matrix.Length;
        if (rowCount == 0) return [];

        var columnCount = matrix[0].Length;
        var pivotColumns = Enumerable.Repeat(-1, rowCount).ToArray();

        var pivotRowCount = ReduceMatrix(matrix, freeMembers, pivotColumns);

        CheckNoSolution(matrix, freeMembers, pivotRowCount);

        return BuildSolution(matrix, freeMembers, pivotColumns, pivotRowCount, columnCount);
    }

    private static void ValidateInput(double[][] matrix, double[] freeMembers)
    {
        var rowCount = matrix.Length;
        if (rowCount == 0) return;

        var columnCount = matrix[0].Length;

        if (matrix.Any(row => row.Length != columnCount))
            throw new ArgumentException("All matrix rows must have the same length");

        if (freeMembers.Length != rowCount)
            throw new ArgumentException("Matrix rows and free members size mismatch");
    }

    private static int ReduceMatrix(
        double[][] matrix,
        double[] freeMembers,
        int[] pivotColumns)
    {
        var rowCount = matrix.Length;
        var columnCount = matrix[0].Length;
        var currentRow = 0;

        for (var column = 0; column < columnCount && currentRow < rowCount; column++)
        {
            var pivotRow = FindPivotRow(matrix, currentRow, column);

            if (Math.Abs(matrix[pivotRow][column]) < Epsilon)
                continue;

            SwapRows(matrix, freeMembers, currentRow, pivotRow);

            pivotColumns[currentRow] = column;

            EliminateColumn(matrix, freeMembers, currentRow, column);

            currentRow++;
        }

        return currentRow;
    }

    private static int FindPivotRow(
        double[][] matrix,
        int startRow,
        int column)
    {
        var rowCount = matrix.Length;

        return Enumerable.Range(startRow, rowCount - startRow)
            .MaxBy(row => Math.Abs(matrix[row][column]));
    }

    private static void SwapRows(
        double[][] matrix,
        double[] freeMembers,
        int firstRow,
        int secondRow)
    {
        (matrix[firstRow], matrix[secondRow]) = (matrix[secondRow], matrix[firstRow]);

        (freeMembers[firstRow], freeMembers[secondRow]) =
            (freeMembers[secondRow], freeMembers[firstRow]);
    }

    private static void EliminateColumn(
        double[][] matrix,
        double[] freeMembers,
        int pivotRow,
        int pivotColumn)
    {
        var rowCount = matrix.Length;

        foreach (var row in Enumerable.Range(0, rowCount).Where(row => row != pivotRow))
            EliminateRow(matrix, freeMembers, row, pivotRow, pivotColumn);
    }

    private static void EliminateRow(
        double[][] matrix,
        double[] freeMembers,
        int row,
        int pivotRow,
        int pivotColumn)
    {
        var columnCount = matrix[0].Length;
        var factor = matrix[row][pivotColumn] / matrix[pivotRow][pivotColumn];

        for (var column = pivotColumn; column < columnCount; column++)
            matrix[row][column] -= factor * matrix[pivotRow][column];

        freeMembers[row] -= factor * freeMembers[pivotRow];
    }

    private static void CheckNoSolution(
        double[][] matrix,
        double[] freeMembers,
        int pivotRowCount)
    {
        var rowCount = matrix.Length;

        var hasContradiction = Enumerable.Range(pivotRowCount, rowCount - pivotRowCount)
            .Any(row => IsZeroRow(matrix[row]) && Math.Abs(freeMembers[row]) > Epsilon);

        if (hasContradiction)
            throw new NoSolutionException("System has no solution");
    }

    private static bool IsZeroRow(double[] row)
    {
        return row.All(value => Math.Abs(value) < Epsilon);
    }

    private static double[] BuildSolution(
        double[][] matrix,
        double[] freeMembers,
        int[] pivotColumns,
        int pivotRowCount,
        int columnCount)
    {
        var solution = new double[columnCount];

        for (var row = pivotRowCount - 1; row >= 0; row--)
            CalculateVariable(matrix, freeMembers, pivotColumns, solution, row);

        return solution;
    }

    private static void CalculateVariable(
        double[][] matrix,
        double[] freeMembers,
        int[] pivotColumns,
        double[] solution,
        int row)
    {
        var pivotColumn = pivotColumns[row];
        var columnCount = matrix[0].Length;
        var sum = CalculateKnownPart(matrix, solution, row, pivotColumn, columnCount);

        solution[pivotColumn] =
            (freeMembers[row] - sum) / matrix[row][pivotColumn];
    }

    private static double CalculateKnownPart(
        double[][] matrix,
        double[] solution,
        int row,
        int pivotColumn,
        int columnCount)
    {
        return Enumerable.Range(pivotColumn + 1, columnCount - pivotColumn - 1)
            .Sum(column => matrix[row][column] * solution[column]);
    }
}
