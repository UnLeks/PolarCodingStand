using System.Numerics;

namespace StraightLinesDLL
{
    public static class StraightLines
    {
        public static (double[], double[]) GettingPointCoordinatesForStraightLines(Complex[] pointsArray)
        {
            double[] a = new double[24];
            double[] b = new double[24];

            double x1 = 0, y1 = 0, x2 = 0, y2 = 0;

            // задания областей Вороного (уравнение прямой y = ax+b)
            // задание областей Вороного для первого нулевого бита (задаётся шестью прямыми)
            // прямая между седьмой (011) и третьей (000) точками:
            x1 = pointsArray[6].Real;
            y1 = pointsArray[6].Imaginary;
            x2 = pointsArray[2].Real;
            y2 = pointsArray[2].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 0);

            // прямая между третьей (000) и пятой (010) точками:
            x1 = pointsArray[2].Real;
            y1 = pointsArray[2].Imaginary;
            x2 = pointsArray[4].Real;
            y2 = pointsArray[4].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 1);

            // прямая между третьей (000) и второй (001) точками:
            x1 = pointsArray[2].Real;
            y1 = pointsArray[2].Imaginary;
            x2 = pointsArray[1].Real;
            y2 = pointsArray[1].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 2);

            // прямая между седьмой (011) и пятой (010) точками: x==0

            // прямая между седьмой (011) и второй (001) точками:
            x1 = pointsArray[6].Real;
            y1 = pointsArray[6].Imaginary;
            x2 = pointsArray[1].Real;
            y2 = pointsArray[1].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 3);

            // прямая между второй (001) и пятой (010) точками:
            x1 = pointsArray[1].Real;
            y1 = pointsArray[1].Imaginary;
            x2 = pointsArray[4].Real;
            y2 = pointsArray[4].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 4);

            // задание областей Вороного для первого единичного бита (задаётся шестью прямыми)
            // прямая между четвёртой (101) и первой (100) точками:

            x1 = pointsArray[3].Real;
            y1 = pointsArray[3].Imaginary;
            x2 = pointsArray[0].Real;
            y2 = pointsArray[0].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 5);

            // прямая между восьмой (111) и четвёртой (101) точками:
            x1 = pointsArray[7].Real;
            y1 = pointsArray[7].Imaginary;
            x2 = pointsArray[3].Real;
            y2 = pointsArray[3].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 6);

            // прямая между четвёртой (101) и шестой (110) точками:
            x1 = pointsArray[3].Real;
            y1 = pointsArray[3].Imaginary;
            x2 = pointsArray[5].Real;
            y2 = pointsArray[5].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 7);

            // прямая между восьмой (111) и первой (100) точками:
            x1 = pointsArray[7].Real;
            y1 = pointsArray[7].Imaginary;
            x2 = pointsArray[0].Real;
            y2 = pointsArray[0].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 8);

            // прямая между первой (100) и шестой (110) точками:
            x1 = pointsArray[0].Real;
            y1 = pointsArray[0].Imaginary;
            x2 = pointsArray[5].Real;
            y2 = pointsArray[5].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 9);

            // прямая между восьмой (111) и шестой (110) точками: x==0

            // задание областей Вороного для второго нулевого бита (задаётся пятью прямыми)
            // прямая между второй (001) и первой (100) точками: x==0

            // прямая между второй (001) и третьей (000) точками:
            x1 = pointsArray[1].Real;
            y1 = pointsArray[1].Imaginary;
            x2 = pointsArray[2].Real;
            y2 = pointsArray[2].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 10);

            // прямая между третьей (000) и первой (100) точками:
            x1 = pointsArray[2].Real;
            y1 = pointsArray[2].Imaginary;
            x2 = pointsArray[0].Real;
            y2 = pointsArray[0].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 11);

            // прямая между первой (100) и четвёртой (101) точками:
            x1 = pointsArray[0].Real;
            y1 = pointsArray[0].Imaginary;
            x2 = pointsArray[3].Real;
            y2 = pointsArray[3].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 12);

            // прямая между четвёртой (101) и второй (001) точками:
            x1 = pointsArray[3].Real;
            y1 = pointsArray[3].Imaginary;
            x2 = pointsArray[1].Real;
            y2 = pointsArray[1].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 13);

            // задание областей Вороного для второго единичного бита (задаётся четыремя прямыми)
            // прямая между седьмой (011) и пятой (010) точками: x==0
            // прямая между пятой (010) и шестой (110) точками: y==0
            // прямая между восьмой (111) и шестой (110) точками: x==0
            // прямая между седьмой (011) и восьмой (111) точками: y==0

            // задание областей Вороного для третьего нулевого бита (задаётся шестью прямыми)
            // прямая между третьей (000) и первой (100) точками:
            x1 = pointsArray[2].Real;
            y1 = pointsArray[2].Imaginary;
            x2 = pointsArray[0].Real;
            y2 = pointsArray[0].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 14);

            // прямая между пятой (010) и первой (100) точками:
            x1 = pointsArray[4].Real;
            y1 = pointsArray[4].Imaginary;
            x2 = pointsArray[0].Real;
            y2 = pointsArray[0].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 15);

            // прямая между первой (100) и шестой (110) точками:
            x1 = pointsArray[0].Real;
            y1 = pointsArray[0].Imaginary;
            x2 = pointsArray[5].Real;
            y2 = pointsArray[5].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 16);

            // прямая между третьей (000) и пятой (010) точками:
            x1 = pointsArray[2].Real;
            y1 = pointsArray[2].Imaginary;
            x2 = pointsArray[4].Real;
            y2 = pointsArray[4].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 17);

            // прямая между пятой (010) и шестой (110) точками: y==0

            // прямая между третьей (000) и шестой (110) точками:
            x1 = pointsArray[2].Real;
            y1 = pointsArray[2].Imaginary;
            x2 = pointsArray[5].Real;
            y2 = pointsArray[5].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 18);

            // задание областей Вороного для третьего единичного бита (задаётся шестью прямыми)
            // прямая между седьмой (011) и второй (001) точками:
            x1 = pointsArray[6].Real;
            y1 = pointsArray[6].Imaginary;
            x2 = pointsArray[1].Real;
            y2 = pointsArray[1].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 19);

            // прямая между второй (001) и восьмой (111) точками:
            x1 = pointsArray[1].Real;
            y1 = pointsArray[1].Imaginary;
            x2 = pointsArray[7].Real;
            y2 = pointsArray[7].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 20);

            // прямая между второй (001) и четвёртой (101) точками:
            x1 = pointsArray[1].Real;
            y1 = pointsArray[1].Imaginary;
            x2 = pointsArray[3].Real;
            y2 = pointsArray[3].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 21);

            // прямая между седьмой (011) и восьмой (111) точками: y==0

            // прямая между седьмой (011) и четвёртой (101) точками:
            x1 = pointsArray[6].Real;
            y1 = pointsArray[6].Imaginary;
            x2 = pointsArray[3].Real;
            y2 = pointsArray[3].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 22);

            // прямая между восьмой (111) и четвёртой (101) точками:
            x1 = pointsArray[7].Real;
            y1 = pointsArray[7].Imaginary;
            x2 = pointsArray[3].Real;
            y2 = pointsArray[3].Imaginary;
            CalculateVoronoiCoefficients(x1, y1, x2, y2, a, b, 23);

            return (a, b);
        }

        static void CalculateVoronoiCoefficients(double x1, double y1, double x2, double y2, double[] a, double[] b, int index)
        {
            a[index] = (x1 - x2) / (y2 - y1);
            b[index] = (y1 + y2) / 2 - ((x1 * x1) - (x2 * x2)) / (2 * (y2 - y1));
        }
    }
}