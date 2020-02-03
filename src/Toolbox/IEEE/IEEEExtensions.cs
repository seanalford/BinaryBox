using System;

namespace Toolbox.IEEE
{
    public static class IEEEExtensions
    {
        public const bool DEFAULT_ISLITTLEENDIAN = false;
    }

    public static class IEEEFloatExtensions
    {
        /// <summary>
        /// Returns the specifed single-precision floating point value as a four-byte IEEE hexadecimal string with the specified endianess.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="isLittleEndian">The endianess of the output. Default is false. </param>
        /// <returns>A hexadecimal string with a lenght of 8.</returns>
        public static string ToIEEEString(this float value, bool isLittleEndian = IEEEExtensions.DEFAULT_ISLITTLEENDIAN)
        {
            byte[] arrayValue = BitConverter.GetBytes(value);

            if (!isLittleEndian)
            {
                Array.Reverse(arrayValue);
            }
            return BitConverter.ToString(arrayValue).Replace("-", string.Empty);
        }

        /// <summary>
        /// Returns the four-byte IEEE hexadecimal string with the specified endianess as single-precision floating point value.
        /// </summary>
        /// <param name="value"> The IEEE hexadecimal string to be converted.</param>
        /// <param name="isLittleEndian">The endianess of the input. Default is false.</param>
        /// <returns>A single-precision floating point value.</returns>
        public static float ToFloat(this string value, bool isLittleEndian = IEEEExtensions.DEFAULT_ISLITTLEENDIAN)
        {
            byte[] arrayValue = BitConverter.GetBytes(
                uint.Parse(value, System.Globalization.NumberStyles.AllowHexSpecifier));

            if (isLittleEndian)
            {
                Array.Reverse(arrayValue);
            }
            return BitConverter.ToSingle(arrayValue, 0);
        }
    }

    public static class IEEEDoubleExtensions
    {
        /// <summary>
        /// Returns the specifed double-precision floating point value as a four-byte IEEE hexadecimal string with the specified endianess.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="isLittleEndian">The endianess of the output. Default is false.</param>
        /// <returns>A hexadecimal string with a lenght of 16.</returns>
        public static string ToIEEEString(this double value, bool isLittleEndian = IEEEExtensions.DEFAULT_ISLITTLEENDIAN)
        {
            byte[] arrayValue = BitConverter.GetBytes(value);

            if (!isLittleEndian)
            {
                Array.Reverse(arrayValue);
            }
            return BitConverter.ToString(arrayValue).Replace("-", string.Empty);
        }

        /// <summary>
        /// Returns the eight-byte IEEE hexadecimal string with the specified endianess as double-precision floating point value.
        /// </summary>
        /// <param name="value"> The IEEE hexadecimal string to be converted.</param>
        /// <param name="isLittleEndian">The endianess of the input. Default is false.</param>
        /// <returns>A double-precision floating point value.</returns>
        public static double ToDouble(this string value, bool isLittleEndian = IEEEExtensions.DEFAULT_ISLITTLEENDIAN)
        {
            byte[] arrayValue = BitConverter.GetBytes(
                ulong.Parse(value, System.Globalization.NumberStyles.AllowHexSpecifier));

            if (isLittleEndian)
            {
                Array.Reverse(arrayValue);
            }
            return BitConverter.ToDouble(arrayValue, 0);
        }
    }
}