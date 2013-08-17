﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colourful.Conversion;
using Colourful.RGBWorkingSpaces;

namespace Colourful.Colors
{
    /// <summary>
    /// RGB color with specified working space
    /// </summary>
    public class RGBColor : RGBColorBase
    {
        #region Other

        /// <summary>
        /// Used when working space is not specified explicitly
        /// </summary>
        public static readonly IRGBWorkingSpace ImplicitWorkingSpace = new sRGBWorkingSpace();

        #endregion

        #region Constructor

        /// <remarks>
        /// Uses <see cref="ImplicitWorkingSpace"/> as working space
        /// </remarks>
        public RGBColor(double r, double g, double b)
            : this(r, g, b, ImplicitWorkingSpace)
        {
        }

        public RGBColor(double r, double g, double b, IRGBWorkingSpace workingSpace)
            : base(r, g, b)
        {
            WorkingSpace = workingSpace;
        }

        #endregion

        #region Attributes

        /// <summary>
        /// The color space of RGB color
        /// </summary>
        public IRGBWorkingSpace WorkingSpace { get; private set; }

        #endregion

        #region Equality

        protected bool Equals(RGBColor other)
        {
            return base.Equals(other) && WorkingSpace.Equals(other.WorkingSpace);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RGBColor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ WorkingSpace.GetHashCode();
            }
        }

        public static bool operator ==(RGBColor left, RGBColor right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RGBColor left, RGBColor right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region Conversions

        #region From

        public static RGBColor FromColor(Color color, IRGBWorkingSpace workingSpace)
        {
            double r = color.R / 255d;
            double g = color.G / 255d;
            double b = color.B / 255d;
            return new RGBColor(r, g, b, workingSpace);
        }

        public static RGBColor FromColor(Color color)
        {
            double r = color.R / 255d;
            double g = color.G / 255d;
            double b = color.B / 255d;
            return new RGBColor(r, g, b);
        }

        #endregion

        #region To

        public Color ToColor()
        {
            var r = (byte) Math.Round(R * 255);
            var g = (byte) Math.Round(G * 255);
            var b = (byte) Math.Round(B * 255);
            return Color.FromArgb(r, g, b);
        }


        /// <remarks>
        /// Reference white of output is taken from RGB working space.
        /// </remarks>
        public XYZColor ToXYZ()
        {
            var converter = new RGBToXYZConverter();
            XYZColor result = converter.Convert(this);
            return result;
        }

        /// <remarks>
        /// Output color is adjusted to the given reference white (Bradford adaptation).
        /// </remarks>
        public XYZColor ToXYZ(XYZColorBase referenceWhite)
        {
            var converter = new RGBToXYZConverter();
            XYZColor result = converter.Convert(this, referenceWhite);
            return result;
        }

        #endregion

        #region Implicit

        public static implicit operator Color(RGBColor rgbColorBase)
        {
            return rgbColorBase.ToColor();
        }

        public static implicit operator RGBColor(Color color)
        {
            return FromColor(color);
        }

        public static implicit operator XYZColor(RGBColor color)
        {
            return color.ToXYZ();
        }

        #endregion

        #endregion
    }
}