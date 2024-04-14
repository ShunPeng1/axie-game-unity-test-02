using System;
using System.Numerics;
using UnityEngine;
/// <summary>
/// Copy and Gpt to C# from
/// https://github.com/NKrvavica/fqs
/// </summary>
public class PolynomialSolver
{
    public static Tuple<Complex, Complex> SingleQuadratic(double a0, double b0, double c0)
    {
        double a = b0 / a0;
        double b = c0 / a0;

        double a0_ = -0.5 * a;
        double delta = a0_ * a0_ - b;
        Complex sqrtDelta = Complex.Sqrt(delta);

        Complex r1 = a0_ - sqrtDelta;
        Complex r2 = a0_ + sqrtDelta;

        return Tuple.Create(r1, r2);
    }

    public static Tuple<Complex, Complex, Complex> SingleCubic(double a0, double b0, double c0, double d0)
    {
        double a = b0 / a0;
        double b = c0 / a0;
        double c = d0 / a0;

        double third = 1.0 / 3.0;
        double a13 = a * third;
        double a2 = a13 * a13;
        double sqrt3 = Math.Sqrt(3);

        double f = third * b - a2;
        double g = a13 * (2 * a2 - b) + c;
        double h = 0.25 * g * g + f * f * f;

        Complex cubicRoot(Complex x)
        {
            return x.Real >= 0 ? Complex.Pow(x, third) : -Complex.Pow(-x, third);
        }

        if (f == g && g == h && h == 0)
        {
            Complex r1 = -cubicRoot(c);
            return Tuple.Create(r1, r1, r1);
        }
        else if (h <= 0)
        {
            double j = Math.Sqrt(-f);
            double k = Math.Acos(-0.5 * g / (j * j * j));
            double m = Math.Cos(third * k);
            double n = sqrt3 * Math.Sin(third * k);
            Complex r1 = 2 * j * m - a13;
            Complex r2 = -j * (m + n) - a13;
            Complex r3 = -j * (m - n) - a13;
            return Tuple.Create(r1, r2, r3);
        }
        else
        {
            Complex sqrtH = Complex.Sqrt(h);
            Complex S = cubicRoot(-0.5 * g + sqrtH);
            Complex U = cubicRoot(-0.5 * g - sqrtH);
            Complex SPlusU = S + U;
            Complex SMinusU = S - U;
            Complex r1 = SPlusU - a13;
            Complex r2 = -0.5 * SPlusU - a13 + SMinusU * sqrt3 * 0.5 * Complex.ImaginaryOne;
            Complex r3 = -0.5 * SPlusU - a13 - SMinusU * sqrt3 * 0.5 * Complex.ImaginaryOne;
            return Tuple.Create(r1, r2, r3);
        }
    }

    public static Complex SingleCubicOne(double a0, double b0, double c0, double d0)
    {
        double a = b0 / a0;
        double b = c0 / a0;
        double c = d0 / a0;

        double third = 1.0 / 3.0;
        double a13 = a * third;
        double a2 = a13 * a13;

        double f = third * b - a2;
        double g = a13 * (2 * a2 - b) + c;
        double h = 0.25 * g * g + f * f * f;

        Complex cubicRoot(Complex x)
        {
            return x.Real >= 0 ? Complex.Pow(x, third) : -Complex.Pow(-x, third);
        }

        if (f == g && g == h && h == 0)
            return -cubicRoot(c);
        else if (h <= 0)
        {
            double j = Math.Sqrt(-f);
            double k = Math.Acos(-0.5 * g / (j * j * j));
            double m = Math.Cos(third * k);
            return 2 * j * m - a13;
        }
        else
        {
            Complex sqrtH = Complex.Sqrt(h);
            Complex S = cubicRoot(-0.5 * g + sqrtH);
            Complex U = cubicRoot(-0.5 * g - sqrtH);
            return S + U - a13;
        }
    }

    public static Tuple<Complex, Complex, Complex, Complex> SingleQuartic(double a0, double b0, double c0, double d0, double e0)
    {
        double a = b0 / a0;
        double b = c0 / a0;
        double c = d0 / a0;
        double d = e0 / a0;

        double a0_ = 0.25 * a;
        double a02 = a0_ * a0_;

        double p = 3 * a02 - 0.5 * b;
        double q = a * a02 - b * a0_ + 0.5 * c;
        double r = 3 * a02 * a02 - b * a02 + c * a0_ - d;

        Complex z0 = SingleCubicOne(1, p, r, p * r - 0.5 * q * q);

        Complex s = Complex.Sqrt(2 * p + 2 * z0.Real);
        Complex t = s == 0 ? z0 * z0 + r : -q / s;

        Tuple<Complex, Complex> pair1 = SingleQuadratic(1, s.Real, (z0 + t).Real);
        Tuple<Complex, Complex> pair2 = SingleQuadratic(1, -s.Real, (z0 - t).Real);

        return Tuple.Create(pair1.Item1 - a0_, pair1.Item2 - a0_, pair2.Item1 - a0_, pair2.Item2 - a0_);
    }

    // Example usage
    public static void Main()
    {
        // Example usage of SingleQuadratic
        var quadraticRoots = SingleQuadratic(1.0, 2.0, 1.0);
        Debug.Log($"Quadratic roots: {quadraticRoots.Item1}, {quadraticRoots.Item2}");

        // Example usage of SingleCubic
        var cubicRoots = SingleCubic(1.0, 0.0, 0.0, -1.0);
        Debug.Log($"Cubic roots: {cubicRoots.Item1}, {cubicRoots.Item2}, {cubicRoots.Item3}");

        // Example usage of SingleCubicOne
        var cubicOneRoot = SingleCubicOne(1.0, 0.0, 0.0, -1.0);
        Debug.Log($"Cubic one root: {cubicOneRoot}");

        // Example usage of SingleQuartic
        var quarticRoots = SingleQuartic(1.0, 0.0, 0.0, 0.0, -1.0);
        Debug.Log($"Quartic roots: {quarticRoots.Item1}, {quarticRoots.Item2}, {quarticRoots.Item3}, {quarticRoots.Item4}");
    }
}