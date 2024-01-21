using System.Numerics;

namespace ModulatorDLL
{
    public static class Modulator
    {
        public static Complex[] MessageModulating(int messageLength, int modulatingPointsArray, Complex[] pointsArray, int[] codedMessage)
        {
            int[] X = new int[messageLength + 1];

            for (int k = 0; k < messageLength; k++)
            {
                X[k] = codedMessage[k];
            }

            Complex[] modulatedMessage = new Complex[modulatingPointsArray];

            for (int k = 0; k < modulatingPointsArray; k++)
            {
                int firstBit = X[3 * k];
                int secondBit = X[3 * k + 1];
                int thirdBit = X[3 * k + 2];

                string part = firstBit.ToString() + secondBit.ToString() + thirdBit.ToString();

                switch (part)
                {
                    case "100":
                        modulatedMessage[k] = pointsArray[0];
                        break;
                    case "001":
                        modulatedMessage[k] = pointsArray[1];
                        break;
                    case "000":
                        modulatedMessage[k] = pointsArray[2];
                        break;
                    case "101":
                        modulatedMessage[k] = pointsArray[3];
                        break;
                    case "010":
                        modulatedMessage[k] = pointsArray[4];
                        break;
                    case "110":
                        modulatedMessage[k] = pointsArray[5];
                        break;
                    case "011":
                        modulatedMessage[k] = pointsArray[6];
                        break;
                    case "111":
                        modulatedMessage[k] = pointsArray[7];
                        break;
                }
            }

            return modulatedMessage;
        }
    }
}