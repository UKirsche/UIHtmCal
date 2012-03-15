using System;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;

namespace Transmute.Windows
{
    public static class TransmuteWindowExtensions
    {
        #region Color Extensions

        private readonly static char[] hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        /// <summary>
        /// Converts an XNA Color to Hex Value
        /// </summary>
        public static string ToHex(this Color color)
        {
            byte[] bytes = new byte[4];
            bytes[0] = color.R;
            bytes[1] = color.G;
            bytes[2] = color.B;
            bytes[3] = color.A;
            char[] chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return "#" + new string(chars);
        }

        /// <summary>
        /// Converts a Hex string to an XNA Color
        /// </summary>
        /// <param name="strHex">A hex string: "FFFFFFFF", "#FFFFFFFF" [RGBA Format]</param>
        public static Color ToColor(this string strHex)
        {
            var ret = Color.Transparent;

            string hc = ExtractHexDigits(strHex);

            if (hc.Length != 8) return ret;

            string r = hc.Substring(0, 2);
            string g = hc.Substring(2, 2);
            string b = hc.Substring(4, 2);
            string a = hc.Substring(6, 2);

            try
            {
                int ri = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
                int gi = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
                int bi = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
                int ai = Int32.Parse(a, System.Globalization.NumberStyles.HexNumber);
                ret = new Color(ri, gi, bi, ai);
            }
            catch
            {
                return ret;
            }

            return ret;
        }

        public static string ExtractHexDigits(string input)
        {
            Regex isHexDigit = new Regex("[abcdefABCDEF\\d]+", RegexOptions.Compiled);
            string newnum = "";
            foreach (char c in input)
            {
                if (isHexDigit.IsMatch(c.ToString())) newnum += c.ToString();
            }
            return newnum;
        }

        #endregion

        #region Object Extensions

        public static bool IsNumeric(this Object Expression)
        {
            if (Expression == null || Expression is DateTime) return false;

            if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean) return true;

            try
            {
                if (Expression is string)
                {
                    Double.Parse(Expression as string);
                }
                else
                {
                    Double.Parse(Expression.ToString());
                }
                return true;
            }
            catch { }

            return false;
        }

        #endregion
    }
}
