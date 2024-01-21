using System;

namespace DirectDecoderDLL
{
    public class DirectDecoder
    {
        public static int[] MessageDecoding(int messageLength, double errorProbability, int[] identificationOfFrozenAndInfoBits, int[,] G, int[] rightDemodulatedMessage)
        {
            int[] decodedMessage = new int[messageLength]; // задание декодированного сообщения
            int[] frozenBitsValues = new int[messageLength / 2]; // задание массива значений замороженных бит
            int[] uG = new int[messageLength];
            double[] hi = new double[messageLength];
            int f = 1;

            for (int i = 1; i <= messageLength; i++)
            {
                if (identificationOfFrozenAndInfoBits[i - 1] == 0)
                {
                    decodedMessage[i - 1] = frozenBitsValues[f - 1];
                    f += 1;
                }
                else
                {
                    for (int j = i + 1; j <= messageLength; j++)
                    {
                        decodedMessage[j - 1] = 0;
                    }

                    double w1 = 0;
                    decodedMessage[i - 1] = 0;

                    // цикл для расчёта первой суммы
                    for (int j = 1; j <= Math.Pow(2, messageLength - i); j++)
                    {
                        uG = Mod(Multiply(decodedMessage, G));
                        double w = 1;

                        for (int k = 1; k <= messageLength; k++)
                        {
                            if (rightDemodulatedMessage[k - 1] == uG[k - 1])
                            {
                                w *= (1 - errorProbability);
                            }
                            else
                            {
                                w *= errorProbability;
                            }
                        }

                        w1 += w / Math.Pow(2, messageLength - 1);
                        int a = 1;
                        int b = 1;
                        int q = messageLength;

                        while (b == 1 && q != 0 && i != messageLength && j != Math.Pow(2, messageLength - i))
                        {
                            a += decodedMessage[q - 1];

                            if (a == 1)
                            {
                                b = 0;
                                decodedMessage[q - 1] = a;
                            }
                            else
                            {
                                decodedMessage[q - 1] = 0;
                                q--;
                                a = 1;
                            }
                        }
                    }

                    for (int j = i + 1; j <= messageLength; j++)
                    {
                        decodedMessage[j - 1] = 0;
                    }

                    double w2 = 0;
                    decodedMessage[i - 1] = 1;

                    // цикл для расчёта второй суммы
                    for (int j = 1; j <= Math.Pow(2, messageLength - i); j++)
                    {
                        uG = Mod(Multiply(decodedMessage, G));
                        double w = 1;

                        for (int k = 1; k <= messageLength; k++)
                        {
                            if (rightDemodulatedMessage[k - 1] == uG[k - 1])
                            {
                                w *= (1 - errorProbability);
                            }
                            else
                            {
                                w *= errorProbability;
                            }
                        }

                        w2 += w / Math.Pow(2, messageLength - 1);
                        int a = 1;
                        int b = 1;
                        int q = messageLength;

                        while (b == 1 && q != 0 && i != messageLength && j != Math.Pow(2, messageLength - i))
                        {
                            a += decodedMessage[q - 1];

                            if (a == 1)
                            {
                                b = 0;
                                decodedMessage[q - 1] = a;
                            }
                            else
                            {
                                decodedMessage[q - 1] = 0;
                                q--;
                                a = 1;
                            }
                        }
                    }

                    hi[i - 1] = Math.Round(w1 / w2, 6);

                    if (hi[i - 1] >= 1)
                    {
                        decodedMessage[i - 1] = 0;
                    }
                    else
                    {
                        decodedMessage[i - 1] = 1;
                    }
                }
            }

            return decodedMessage;
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