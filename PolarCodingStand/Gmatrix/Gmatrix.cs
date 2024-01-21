using System;

namespace GmatrixDLL
{
    public static class Gmatrix
    {
        public static int[,] ConstructionOfGmatrix(int messageLength)
        {
            int[,] F =
            {
                { 1, 0 },
                { 1, 1 }
            };

            int n = (int)Math.Truncate(Math.Log(messageLength, 2));

            int[,] G = new int[messageLength, messageLength];

            if (messageLength == 2)
            {
                G = F;
            }
            else if (messageLength > 2)
            {
                int[,] F1;
                F1 = KroneckerProduct(F, F);

                if (messageLength == 4)
                {
                    G = F1;
                }
                else
                {
                    for (int i = 0; i < n - 2; i++)
                    {
                        F1 = KroneckerProduct(F1, F);
                    }

                    G = F1;
                }
            }

            int[,] B = new int[messageLength, messageLength];

            for (int i = 1; i <= messageLength; i++)
            {
                B[i - 1, i - 1] = 1;
            }

            for (int i = 1; i <= n; i++)
            {
                int r = (int)Math.Pow(2, i);
                int[,] RR = new int[r, r];

                for (int j = 1; j <= Math.Pow(2, i - 1); j++)
                {
                    RR[2 * j - 2, j - 1] = 1;
                    RR[2 * j - 1, (int)Math.Pow(2, i - 1) + j - 1] = 1;
                }

                int[,] R = new int[messageLength, messageLength];

                for (int j = 1; j <= Math.Pow(2, n - i); j++)
                {
                    for (int k = 1; k <= r; k++)
                    {
                        for (int l = 1; l <= r; l++)
                        {
                            R[(j - 1) * r + (k - 1), (j - 1) * r + (l - 1)] = RR[k - 1, l - 1];
                        }
                    }
                }

                // Умножение матриц B и R с применением операции модуля
                B = Mod(MatrixMultiplication(R, B), 2);
            }

            // Умножение матриц B и G с применением операции модуля
            G = Mod(MatrixMultiplication(B, G), 2);

            return G;
        }

        // Метод для вычисления кронекерова произведения двух матриц
        public static int[,] KroneckerProduct(int[,] A, int[,] B)
        {
            int rowsA = A.GetLength(0);
            int colsA = A.GetLength(1);
            int rowsB = B.GetLength(0);
            int colsB = B.GetLength(1);

            int[,] result = new int[rowsA * rowsB, colsA * colsB];

            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsA; j++)
                {
                    for (int k = 0; k < rowsB; k++)
                    {
                        for (int l = 0; l < colsB; l++)
                        {
                            result[i * rowsB + k, j * colsB + l] = A[i, j] * B[k, l];
                        }
                    }
                }
            }

            return result;
        }

        public static int[,] MatrixMultiplication(int[,] A, int[,] B)
        {
            int rowsA = A.GetLength(0);
            int colsA = A.GetLength(1);
            int rowsB = B.GetLength(0);
            int colsB = B.GetLength(1);

            if (colsA != rowsB)
            {
                throw new ArgumentException("Количество столбцов в первой матрице должно быть равно количеству строк во второй матрице.");
            }

            int[,] result = new int[rowsA, colsB];

            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    for (int k = 0; k < colsA; k++)
                    {
                        result[i, j] += A[i, k] * B[k, j];
                    }
                }
            }

            return result;
        }

        public static int[,] Mod(int[,] matrix, int modulus)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] result = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = matrix[i, j] % modulus;
                }
            }

            return result;
        }
    }
}