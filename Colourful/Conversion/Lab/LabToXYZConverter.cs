﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colourful.Colors;

namespace Colourful.Conversion
{
    /// <summary>
    /// Converts from <see cref="LabColor"/> to <see cref="XYZColor"/>.
    /// </summary>
    public class LabToXYZConverter : XYZAndLabConverterBase, IColorConverter<LabColor, XYZColor>
    {
        public XYZColor Convert(LabColor input)
        {
            if (input == null) throw new ArgumentNullException("input");

            // conversion algorithm described here: http://www.brucelindbloom.com/index.html?Eqn_Lab_to_XYZ.html
            double L = input.L, a = input.a, b = input.b;
            double fy = (L + 16) / 116d;
            double fx = a / 500d + fy;
            double fz = fy - b / 200d;

            double fx3 = Math.Pow(fx, 3);
            double fz3 = Math.Pow(fz, 3);

            double xr = fx3 > Epsilon ? fx3 : (116 * fx - 16) / Kappa;
            double yr = L > Kappa * Epsilon ? Math.Pow((L + 16) / 116d, 3) : L / Kappa;
            double zr = fz3 > Epsilon ? fz3 : (116 * fz - 16) / Kappa;

            double Xr = input.WhitePoint.X, Yr = input.WhitePoint.Y, Zr = input.WhitePoint.Z;

            // avoids XYZ coordinates out range (restricted by 0 and XYZ reference white)
            xr = xr.CropRange(0, 1);
            yr = yr.CropRange(0, 1);
            zr = zr.CropRange(0, 1);

            double X = xr * Xr;
            double Y = yr * Yr;
            double Z = zr * Zr;

            var result = new XYZColor(X, Y, Z);
            return result;
        }

        #region Overrides

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public static bool operator ==(LabToXYZConverter left, LabToXYZConverter right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LabToXYZConverter left, LabToXYZConverter right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}