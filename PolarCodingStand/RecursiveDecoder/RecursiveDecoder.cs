using RecursiveFunctionDLL;

namespace RecursiveDecoderDLL
{
    public static class RecursiveDecoder
    {
        public static int[] MessageDecoding(int messageLength, double errorProbability, int[] identificationOfFrozenAndInfoBits, int[] precodedMessage, int[] rightDemodulatedMessage)
        {
            int[] decodedMessage = new int[messageLength]; // задание декодированного сообщения
            double[] L = new double[messageLength]; // задание массива значений после выполнения рекурсивной функции

            for (int i = 0; i < messageLength; i++)
            {
                if (identificationOfFrozenAndInfoBits[i] == 0)
                {
                    decodedMessage[i] = precodedMessage[i];
                }
                else
                {
                    int[] predecodedMessage = new int[i]; // задание промежуточного декодированного сообщения

                    for (int j = 0; j < i; j++)
                    {
                        predecodedMessage[j] = decodedMessage[j];
                    }

                    // вызов рекурсивной функции
                    L[i] = RecursiveFunction.RecursiveFunctionEvaluation(i + 1, messageLength, errorProbability, predecodedMessage, rightDemodulatedMessage);

                    if (L[i] >= 0.9999999)
                    {
                        decodedMessage[i] = 0;
                    }
                    else
                    {
                        decodedMessage[i] = 1;
                    }
                }
            }

            return decodedMessage;
        }
    }
}