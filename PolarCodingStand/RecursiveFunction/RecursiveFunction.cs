using System;
using GmatrixDLL;

namespace RecursiveFunctionDLL
{
    public static class RecursiveFunction
    {
        public static double RecursiveFunctionEvaluation(int i, int messageLength, double errorProbability, int[] predecodedMessage, int[] rightDemodulatedMessage)
        {
            double L = 0;

            if (i == 2)
            {
                int[,] G = Gmatrix.ConstructionOfGmatrix(messageLength);

                int[] u1 = new int[messageLength - 2];
                int[] u2 = new int[messageLength];

                double w0 = 0, w1 = 0;

                for (int j = 0; j < Math.Pow(2, messageLength - 2); j++)
                {
                    u2[0] = predecodedMessage[0];

                    for (int n = 2; n < messageLength; n++)
                    {
                        u2[n] = u1[n - 2];
                    }

                    u2[1] = 0;
                    int[] v0 = Mod(Multiply(u2, G));
                    u2[1] = 1;
                    int[] v1 = Mod(Multiply(u2, G));
                    double ww0 = 1;
                    double ww1 = 1;

                    for (int p = 0; p < messageLength; p++)
                    {
                        if (v0[p] == rightDemodulatedMessage[p])
                        {
                            ww0 *= 1 - errorProbability;
                        }
                        else
                        {
                            ww0 *= errorProbability;
                        }

                        if (v1[p] == rightDemodulatedMessage[p])
                        {
                            ww1 *= 1 - errorProbability;
                        }
                        else
                        {
                            ww1 *= errorProbability;
                        }
                    }

                    w0 += ww0;
                    w1 += ww1;
                    int z = 1;

                    for (int p = 0; p < messageLength - 2; p++)
                    {
                        z = u1[p] + z;

                        if (z == 2)
                        {
                            u1[p] = 0;
                            z = 1;
                        }
                        else
                        {
                            u1[p] = z;
                            z = 0;
                        }
                    }
                }

                L = w0 / w1;
            }
            else
            {
                int N1 = (int)Math.Floor(messageLength / 2.0);
                int[] rightDemodulatedMessage1 = new int[N1];
                int[] rightDemodulatedMessage2 = new int[N1];

                for (int j = 0; j < N1; j++)
                {
                    rightDemodulatedMessage1[j] = rightDemodulatedMessage[j];
                    rightDemodulatedMessage2[j] = rightDemodulatedMessage[N1 + j];
                }

                int i1;

                if (i % 2 == 0)
                {
                    i1 = (int)Math.Floor(i / 2.0);
                }
                else
                {
                    i1 = (int)Math.Floor((i + 1) / 2.0);
                }

                int[] uuu1 = new int[i1 - 1];
                int[] uuu2 = new int[i1 - 1];

                for (int j = 0; j < i1 - 1; j++)
                {
                    uuu1[j] = (predecodedMessage[2 * j] + predecodedMessage[2 * j + 1]) % 2;
                    uuu2[j] = predecodedMessage[2 * j + 1];
                }

                double L1 = RecursiveFunctionEvaluation(i1, N1, errorProbability, uuu1, rightDemodulatedMessage1);
                double L2 = RecursiveFunctionEvaluation(i1, N1, errorProbability, uuu2, rightDemodulatedMessage2);

                if (i % 2 == 0)
                {
                    L = Math.Pow(L1, 1 - 2 * predecodedMessage[i - 2]) * L2;
                }
                else
                {
                    L = (L1 * L2 + 1) / (L1 + L2);
                }
            }

            return L;
        }

        public static int[] Multiply(int[] vector, int[,] matrix)
        {
            int[] result = new int[vector.Length];

            for (int i = 0; i < vector.Length; i++)
            {
                int sum = 0;

                for (int j = 0; j < vector.Length; j++)
                {
                    sum += vector[j] * matrix[j, i];
                }

                result[i] = sum;
            }

            return result;
        }

        public static int[] Mod(int[] vector)
        {
            int[] result = new int[vector.Length];

            for (int i = 0; i < vector.Length; i++)
            {
                result[i] = vector[i] % 2;
            }

            return result;
        }
    }
}