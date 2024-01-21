using System;
using System.Collections.Generic;
using GmatrixDLL;

namespace BhattacharyaParametersDLL
{
    public static class BhattacharyaParameters
    {
        public static (double[], int[], double[], double[], int[], int[]) FindingBhattacharyaParameters(int messageLength, double errorProbability)
        {
            messageLength /= 2;
            int[,] G = Gmatrix.ConstructionOfGmatrix(messageLength);

            double[,] bk = new double[1, messageLength];
            double[,] zk = new double[1, messageLength * 2];
            double[] valuesOfFrozenBits = new double[messageLength];
            int[] frozenBitsIndices = new int[messageLength];
            double[] valuesOfInfoBits = new double[messageLength];
            int[] infoBitsIndices = new int[messageLength];

            for (int i = 1; i <= messageLength; i++)
            {
                int[,] y = new int[1, messageLength];

                for (int j = 1; j <= Math.Pow(2, i - 1); j++)
                {
                    int[,] yy = Mod(MatrixMultiplication(y, G), 2);
                    double w0 = 0;
                    int[,] u = new int[1, messageLength];

                    for (int k = 1; k <= Math.Pow(2, messageLength - i); k++)
                    {
                        int[,] uu = Mod(MatrixMultiplication(u, G), 2);
                        double w = 1;

                        for (int q = 1; q <= messageLength; q++)
                        {
                            if (yy[0, q - 1] == uu[0, q - 1])
                            {
                                w *= 1 - errorProbability;
                            }
                            else
                            {
                                w *= errorProbability;
                            }
                        }

                        w0 += w;
                        int a = 1;
                        int qq = messageLength;
                        int b = 1;

                        while (b == 1 && qq != 0)
                        {
                            a += u[0, qq - 1];

                            if (a == 2)
                            {
                                u[0, qq - 1] = 0;
                                a = 1;
                            }
                            else
                            {
                                u[0, qq - 1] = a;
                                b = 0;
                            }

                            qq -= 1;
                        }
                    }

                    double w1 = 0;

                    for (int k = 1; k <= messageLength; k++)
                    {
                        u[0, k - 1] = 0;
                    }

                    u[0, i - 1] = 1;

                    for (int k = 1; k <= Math.Pow(2, messageLength - i); k++)
                    {
                        int[,] uu = Mod(MatrixMultiplication(u, G), 2);
                        double w = 1;

                        for (int q = 1; q <= messageLength; q++)
                        {
                            if (yy[0, q - 1] == uu[0, q - 1])
                            {
                                w *= 1 - errorProbability;
                            }
                            else
                            {
                                w *= errorProbability;
                            }
                        }

                        w1 += w;
                        int a = 1;
                        int qq = messageLength;
                        int b = 1;

                        while (b == 1 && qq != 0)
                        {
                            a += u[0, qq - 1];

                            if (a == 2)
                            {
                                u[0, qq - 1] = 0;
                                a = 1;
                            }
                            else
                            {
                                u[0, qq - 1] = a;
                                b = 0;
                            }

                            qq -= 1;
                        }
                    }

                    bk[0, i - 1] += 2 * Math.Sqrt(w0 * w1);
                    int aa = 1;
                    int qqq = 1;
                    int bb = 1;

                    while (bb == 1 && qqq != messageLength + 1)
                    {
                        aa += y[0, qqq - 1];

                        if (aa == 2)
                        {
                            y[0, qqq - 1] = 0;
                            aa = 1;
                        }
                        else
                        {
                            y[0, qqq - 1] = aa;
                            bb = 0;
                        }

                        qqq += 1;
                    }
                }
            }

            for (int i = 1; i <= messageLength; i++)
            {
                zk[0, 2 * i - 2] = 2 * bk[0, i - 1] - bk[0, i - 1] * bk[0, i - 1];
                zk[0, 2 * i - 1] = bk[0, i - 1] * bk[0, i - 1];
            }

            // Создаем словарь, где ключи - значения из исходного массива, 
            // а значения - их индексы в исходном массиве
            Dictionary<double, int> indexDict = new Dictionary<double, int>();

            // Заполняем словарь индексами исходного массива
            for (int i = 0; i < zk.Length; i++)
            {
                indexDict.Add(zk[0, i], i);
            }

            // Получаем первую строку из двумерного массива
            double[] _BhattacharyaParameters = new double[zk.GetLength(1)];
            for (int i = 0; i < zk.GetLength(1); i++)
            {
                _BhattacharyaParameters[i] = zk[0, i];
            }

            // Сортируем исходный массив по возрастанию
            Array.Sort(_BhattacharyaParameters);

            // Создаем массив с индексами согласно отсортированному массиву
            int[] sortedIndices = new int[_BhattacharyaParameters.Length];

            for (int i = 0; i < _BhattacharyaParameters.Length; i++)
            {
                // Получаем индекс значения из словаря и заполняем массив индексов
                sortedIndices[i] = indexDict[_BhattacharyaParameters[i]];
            }

            for (int i = 0; i < messageLength; i++)
            {
                valuesOfFrozenBits[i] = _BhattacharyaParameters[i + messageLength];
                frozenBitsIndices[i] = sortedIndices[i + messageLength];

                valuesOfInfoBits[i] = _BhattacharyaParameters[i];
                infoBitsIndices[i] = sortedIndices[i];
            }

            Array.Sort(frozenBitsIndices);
            Array.Sort(infoBitsIndices);

            return (_BhattacharyaParameters, sortedIndices, valuesOfFrozenBits, valuesOfInfoBits, frozenBitsIndices, infoBitsIndices);
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