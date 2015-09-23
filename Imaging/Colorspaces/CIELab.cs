﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Claw.Imaging.Colorspaces
{
    public class CIELab
    {
        public double L { get; set; }
        public double a { get; set; }
        public double b { get; set; }

        public CIELab(byte R, byte G, byte B)
        {
            double[] lab = RGBtoLAB(R, G, B);
            L = lab[0];
            a = lab[1];
            b = lab[2];
        }

        public CIELab(Color Color)
            : this(Color.R, Color.G, Color.B)
        {
        }

        public Color ToRGB()
        {
            byte[] rgb = LABtoRGB(L, a, b);
            return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
        }

        public static double CalculateDeltaE(CIELab p, CIELab v)
        {
            //return Math.Sqrt(Math.Pow(p.L - v.L, 2) + Math.Pow(p.a - v.a, 2) + Math.Pow(p.b - v.b, 2));
            return ((p.L - v.L) * (p.L - v.L) + (p.a - v.a) * (p.a - v.a) + (p.b - v.b) * (p.b - v.b)); // faster
        }

        public double CalculateDeltaE(CIELab v)
        {
            return CalculateDeltaE(this, v);
        }


        // The code below is a modified version of dvs's code from http://imagej.1557.x6.nabble.com/RGB-to-L-a-b-Conversion-tp3703729p3703730.html

        /** 
         * reference white in XYZ coordinates 
         */
        private static double[] D65 = { 95.047, 100.0, 108.883 };

        /** 
         * sRGB to XYZ conversion matrix 
         */
        private static double[,] M = {{0.412424, 0.212656,  0.0193324}, 
                                   {0.357579, 0.715158,  0.119193 }, 
                                   {0.180464, 0.0721856, 0.950444 }};

        /** 
         * XYZ to sRGB conversion matrix 
         */
        private static double[,] Mi = {{3.24071,  -0.969258,  0.0556352}, 
                                   {-1.53726,  1.87599,  -0.203996 }, 
                                   {-0.498571, 0.0415557, 1.05707  }};

        /** 
         * Convert LAB to RGB. 
         * @param L 
         * @param a 
         * @param b 
         * @return RGB values 
         */
        public static byte[] LABtoRGB(double L, double a, double b)
        {
            return XYZtoRGB(LABtoXYZ(L, a, b));
        }

        /** 
         * @param Lab 
         * @return RGB values 
         */
        public static byte[] LABtoRGB(double[] Lab)
        {
            return XYZtoRGB(LABtoXYZ(Lab));
        }

        /** 
         * Convert LAB to XYZ. 
         * @param L 
         * @param a 
         * @param b 
         * @return XYZ values 
         */
        public static double[] LABtoXYZ(double L, double a, double b)
        {
            double[] result = new double[3];

            double y = (L + 16.0) / 116.0;
            double y3 = Math.Pow(y, 3.0);
            double x = (a / 500.0) + y;
            double x3 = Math.Pow(x, 3.0);
            double z = y - (b / 200.0);
            double z3 = Math.Pow(z, 3.0);

            if (y3 > 0.008856) {
                y = y3;
            } else {
                y = (y - (16.0 / 116.0)) / 7.787;
            }
            if (x3 > 0.008856) {
                x = x3;
            } else {
                x = (x - (16.0 / 116.0)) / 7.787;
            }
            if (z3 > 0.008856) {
                z = z3;
            } else {
                z = (z - (16.0 / 116.0)) / 7.787;
            }

            result[0] = x * D65[0];
            result[1] = y * D65[1];
            result[2] = z * D65[2];

            return result;
        }

        /** 
         * Convert LAB to XYZ. 
         * @param Lab 
         * @return XYZ values 
         */
        public static double[] LABtoXYZ(double[] Lab)
        {
            return LABtoXYZ(Lab[0], Lab[1], Lab[2]);
        }

        /** 
         * @param R 
         * @param G 
         * @param B 
         * @return Lab values 
         */
        public static double[] RGBtoLAB(byte R, byte G, byte B)
        {
            return XYZtoLAB(RGBtoXYZ(R, G, B));
        }

        /** 
         * @param RGB 
         * @return Lab values 
         */
        public static double[] RGBtoLAB(byte[] RGB)
        {
            return XYZtoLAB(RGBtoXYZ(RGB));
        }

        /** 
         * Convert RGB to XYZ 
         * @param R 
         * @param G 
         * @param B 
         * @return XYZ in double array. 
         */
        public static double[] RGBtoXYZ(byte R, byte G, byte B)
        {
            double[] result = new double[3];

            // convert 0..255 into 0..1 
            double r = R / 255.0;
            double g = G / 255.0;
            double b = B / 255.0;

            // assume sRGB 
            if (r <= 0.04045) {
                r = r / 12.92;
            } else {
                r = Math.Pow(((r + 0.055) / 1.055), 2.4);
            }
            if (g <= 0.04045) {
                g = g / 12.92;
            } else {
                g = Math.Pow(((g + 0.055) / 1.055), 2.4);
            }
            if (b <= 0.04045) {
                b = b / 12.92;
            } else {
                b = Math.Pow(((b + 0.055) / 1.055), 2.4);
            }

            r *= 100.0;
            g *= 100.0;
            b *= 100.0;

            // [X Y Z] = [r g b][M] 
            result[0] = (r * M[0, 0]) + (g * M[1, 0]) + (b * M[2, 0]);
            result[1] = (r * M[0, 1]) + (g * M[1, 1]) + (b * M[2, 1]);
            result[2] = (r * M[0, 2]) + (g * M[1, 2]) + (b * M[2, 2]);

            return result;
        }

        /** 
         * Convert RGB to XYZ 
         * @param RGB 
         * @return XYZ in double array. 
         */
        public static double[] RGBtoXYZ(byte[] RGB)
        {
            return RGBtoXYZ(RGB[0], RGB[1], RGB[2]);
        }

        /** 
         * @param x 
         * @param y 
         * @param Y 
         * @return XYZ values 
         */
        private static double[] xyYtoXYZ(double x, double y, double Y)
        {
            double[] result = new double[3];
            if (y == 0) {
                result[0] = 0;
                result[1] = 0;
                result[2] = 0;
            } else {
                result[0] = (x * Y) / y;
                result[1] = Y;
                result[2] = ((1 - x - y) * Y) / y;
            }
            return result;
        }

        /** 
         * @param xyY 
         * @return XYZ values 
         */
        private static double[] xyYtoXYZ(double[] xyY)
        {
            return xyYtoXYZ(xyY[0], xyY[1], xyY[2]);
        }

        /** 
         * Convert XYZ to LAB. 
         * @param X 
         * @param Y 
         * @param Z 
         * @return Lab values 
         */
        public static double[] XYZtoLAB(double X, double Y, double Z)
        {

            double x = X / D65[0];
            double y = Y / D65[1];
            double z = Z / D65[2];

            if (x > 0.008856) {
                x = Math.Pow(x, 1.0 / 3.0);
            } else {
                x = (7.787 * x) + (16.0 / 116.0);
            }
            if (y > 0.008856) {
                y = Math.Pow(y, 1.0 / 3.0);
            } else {
                y = (7.787 * y) + (16.0 / 116.0);
            }
            if (z > 0.008856) {
                z = Math.Pow(z, 1.0 / 3.0);
            } else {
                z = (7.787 * z) + (16.0 / 116.0);
            }

            double[] result = new double[3];

            result[0] = (116.0 * y) - 16.0;
            result[1] = 500.0 * (x - y);
            result[2] = 200.0 * (y - z);

            return result;
        }

        /** 
         * Convert XYZ to LAB. 
         * @param XYZ 
         * @return Lab values 
         */
        public static double[] XYZtoLAB(double[] XYZ)
        {
            return XYZtoLAB(XYZ[0], XYZ[1], XYZ[2]);
        }

        /** 
         * Convert XYZ to RGB. 
         * @param X 
         * @param Y 
         * @param Z 
         * @return RGB in int array. 
         */
        public static byte[] XYZtoRGB(double X, double Y, double Z)
        {
            byte[] result = new byte[3];

            double x = X / 100.0;
            double y = Y / 100.0;
            double z = Z / 100.0;

            // [r g b] = [X Y Z][Mi] 
            double r = (x * Mi[0, 0]) + (y * Mi[1, 0]) + (z * Mi[2, 0]);
            double g = (x * Mi[0, 1]) + (y * Mi[1, 1]) + (z * Mi[2, 1]);
            double b = (x * Mi[0, 2]) + (y * Mi[1, 2]) + (z * Mi[2, 2]);

            // assume sRGB 
            if (r > 0.0031308) {
                r = ((1.055 * Math.Pow(r, 1.0 / 2.4)) - 0.055);
            } else {
                r = (r * 12.92);
            }
            if (g > 0.0031308) {
                g = ((1.055 * Math.Pow(g, 1.0 / 2.4)) - 0.055);
            } else {
                g = (g * 12.92);
            }
            if (b > 0.0031308) {
                b = ((1.055 * Math.Pow(b, 1.0 / 2.4)) - 0.055);
            } else {
                b = (b * 12.92);
            }

            r = (r < 0) ? 0 : r;
            g = (g < 0) ? 0 : g;
            b = (b < 0) ? 0 : b;

            // convert 0..1 into 0..255 
            result[0] = (byte)Math.Round(r * 255);
            result[1] = (byte)Math.Round(g * 255);
            result[2] = (byte)Math.Round(b * 255);

            return result;
        }

        /** 
         * Convert XYZ to RGB 
         * @param XYZ in a double array. 
         * @return RGB in int array. 
         */
        public static byte[] XYZtoRGB(double[] XYZ)
        {
            return XYZtoRGB(XYZ[0], XYZ[1], XYZ[2]);
        }

        /** 
         * @param X 
         * @param Y 
         * @param Z 
         * @return xyY values 
         */
        private static double[] XYZtoxyY(double X, double Y, double Z)
        {
            double[] result = new double[3];
            if ((X + Y + Z) == 0) {
                result[0] = D65[0];
                result[1] = D65[1];
                result[2] = D65[2];
            } else {
                result[0] = X / (X + Y + Z);
                result[1] = Y / (X + Y + Z);
                result[2] = Y;
            }
            return result;
        }

        /** 
         * @param XYZ 
         * @return xyY values 
         */
        private double[] XYZtoxyY(double[] XYZ)
        {
            return XYZtoxyY(XYZ[0], XYZ[1], XYZ[2]);
        }
    }
}