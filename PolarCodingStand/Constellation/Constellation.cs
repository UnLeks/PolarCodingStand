using System;
using System.Numerics;

namespace ConstellationDLL
{
    public static class Constellation
    {
        public static (Complex[], double[] R) ConstellationConstruction()
        {
            double[] R = { 1, 1.08, 2 };

            Complex[] pointsArray = new Complex[8];

            pointsArray[0] = new Complex(R[0], 0); // точка 100
            pointsArray[1] = new Complex(-R[0], 0); // точка 001
            pointsArray[2] = new Complex(R[1] * Math.Cos(Math.PI / 2), R[1] * Math.Sin(Math.PI / 2)); // точка 000
            pointsArray[3] = new Complex(R[1] * Math.Cos(-Math.PI / 2), R[1] * Math.Sin(-Math.PI / 2)); // точка 101
            pointsArray[4] = new Complex(R[2] * Math.Cos(Math.PI / 4), R[2] * Math.Sin(Math.PI / 4)); // точка 010
            pointsArray[5] = new Complex(R[2] * Math.Cos(-Math.PI / 4), R[2] * Math.Sin(-Math.PI / 4)); // точка 110
            pointsArray[6] = new Complex(R[2] * Math.Cos(3 * Math.PI / 4), R[2] * Math.Sin(3 * Math.PI / 4)); // точка 011
            pointsArray[7] = new Complex(R[2] * Math.Cos(-3 * Math.PI / 4), R[2] * Math.Sin(-3 * Math.PI / 4)); // точка 111

            return (pointsArray, R);
        }
    }
}