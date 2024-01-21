using System;
using System.Numerics;

namespace DemodulatorDLL
{
    public static class Demodulator
    {
        public static int[] MessageDemodulating(int messageLength, double sig, Complex[] pointsArray, double[] a, double[] b, Complex[] noisyMessage)
        {
            int[] demodulatedMessage = new int[messageLength + 1]; // демодулированное сообщение с дополнительным битом в конце

            double llr_10 = 0, llr_20 = 0, llr_30 = 0;
            double llr_11 = 0, llr_21 = 0, llr_31 = 0;
            double[] llr_ = new double[3];

            for (int i = 0; i < noisyMessage.Length; i++)
            {
                double x = noisyMessage[i].Real; // получение действительной части комплексного числа
                double y = noisyMessage[i].Imaginary; // получение мнимой части комплексного числа

                // проверка первого бита
                // проверка первого нулевого бита
                // проверка попадания принятой точки в область D1
                if ((y <= a[0] * x + b[0]) && (y <= a[1] * x + b[1]) && (y >= a[2] * x + b[2]))
                {
                    llr_10 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[2].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D2
                else if ((y >= a[3] * x + b[3]) && (y > a[0] * x + b[0]) && (x <= 0))
                {
                    llr_10 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[6].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D3
                else if ((x > 0) && (y > a[1] * x + b[1]) && (y >= a[4] * x + b[4]))
                {
                    llr_10 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[4].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D4
                else if ((y < a[3] * x + b[3]) && (y < a[2] * x + b[2]) && (y < a[4] * x + b[4]))
                {
                    llr_10 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[1].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }

                // проверка первого единичного бита
                // проверка попадания принятой точки в область D1"
                if ((y <= a[5] * x + b[5]) && (y >= a[6] * x + b[6]) && (y >= a[7] * x + b[7]))
                {
                    llr_11 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[3].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D2"
                else if ((y >= a[8] * x + b[8]) && (y > a[5] * x + b[5]) && (y >= a[9] * x + b[9]))
                {
                    llr_11 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[0].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D3"
                else if ((y < a[8] * x + b[8]) && (y < a[6] * x + b[6]) && (x <= 0))
                {
                    llr_11 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[7].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D4"
                else if ((x > 0) && (y < a[7] * x + b[7]) && (y < a[9] * x + b[9]))
                {
                    llr_11 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[5].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }

                llr_[0] = llr_10 - llr_11;

                if (llr_[0] < 0)
                {
                    demodulatedMessage[3 * i] = 1;
                }

                // проверка второго бита
                // проверка второго нулевого бита
                // проверка попадания принятой точки в область D1
                if ((y >= a[10] * x + b[10]) && (y >= a[11] * x + b[11]))
                {
                    llr_20 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[2].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D2
                else if ((y < a[10] * x + b[10]) && (x <= 0) && (y >= a[13] * x + b[13]))
                {
                    llr_20 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[1].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D3
                else if ((y < a[11] * x + b[11]) && (x > 0) && (y >= a[12] * x + b[12]))
                {
                    llr_20 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[0].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D4
                else if ((y < a[13] * x + b[13]) && (y < a[12] * x + b[12]))
                {
                    llr_20 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[3].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }

                // проверка второго единичного бита
                // проверка попадания принятой точки в область D1"
                if ((x <= 0) && (y >= 0))
                {
                    llr_21 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[6].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D2"
                else if ((x > 0) && (y >= 0))
                {
                    llr_21 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[4].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D3"
                else if ((y < 0) && (x <= 0))
                {
                    llr_21 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[7].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D4"
                else if ((x > 0) && (y < 0))
                {
                    llr_21 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[5].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }

                llr_[1] = llr_20 - llr_21;

                if (llr_[1] < 0)
                {
                    demodulatedMessage[3 * i + 1] = 1;
                }

                // проверка третьего бита
                // проверка третьего нулевого бита
                // проверка попадания принятой точки в область D1
                if ((y <= a[14] * x + b[14]) && (y <= a[15] * x + b[15]) && (y >= a[16] * x + b[16]))
                {
                    llr_30 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[0].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D2
                else if ((y >= a[18] * x + b[18]) && (y > a[14] * x + b[14]) && (y <= a[17] * x + b[17]))
                {
                    llr_30 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[2].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D3
                else if ((y > a[17] * x + b[17]) && (y > a[15] * x + b[15]) && (y >= 0))
                {
                    llr_30 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[4].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D4
                else if ((y < a[18] * x + b[18]) && (y < a[16] * x + b[16]) && (y < 0))
                {
                    llr_30 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[5].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }

                // проверка третьего единичного бита
                // проверка попадания принятой точки в область D1"
                if ((y <= a[19] * x + b[19]) && (y >= a[20] * x + b[20]) && (y >= a[21] * x + b[21]))
                {
                    llr_31 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[1].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D2"
                else if ((y >= 0) && (y > a[19] * x + b[19]) && (y >= a[22] * x + b[22]))
                {
                    llr_31 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[6].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D3"
                else if ((y < 0) && (y < a[20] * x + b[20]) && (y <= a[23] * x + b[23]))
                {
                    llr_31 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[7].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }
                // проверка попадания принятой точки в область D4"
                else if ((y > a[23] * x + b[23]) && (y < a[21] * x + b[21]) && (y < a[22] * x + b[22]))
                {
                    llr_31 = -(Math.Pow(Math.Abs(noisyMessage[i].Magnitude - pointsArray[3].Magnitude), 2)) / (2 * Math.Pow(sig, 2));
                }

                llr_[2] = llr_30 - llr_31;

                if (llr_[2] < 0)
                {
                    demodulatedMessage[3 * i + 2] = 1;
                }
            }

            return demodulatedMessage;
        }
    }
}