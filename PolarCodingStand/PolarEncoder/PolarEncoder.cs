namespace PolarEncoderDLL
{
    public static class PolarEncoder
    {
        public static (int[] codedMessage, int[] precodedMessage) PolarMessageEncoding(int messageLength, int[] sentInfoWord, int[] frozenBitsIndices, int[] infoBitsIndices, int[,] G)
        {
            int[] frozenBitsValues = new int[messageLength / 2]; // задание массива значений замороженных бит
            int[] precodedMessage = new int[messageLength]; // задание промежуточного закодированного слова

            for (int k = 0; k < messageLength / 2; k++)
            {
                precodedMessage[frozenBitsIndices[k]] = frozenBitsValues[k]; // расстановка замороженных бит
            }

            for (int k = 0; k < messageLength / 2; k++)
            {
                precodedMessage[infoBitsIndices[k]] = sentInfoWord[k]; // расстановка информационных бит
            }

            int[] codedMessage = Mod(Multiply(precodedMessage, G));

            return (codedMessage, precodedMessage);
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