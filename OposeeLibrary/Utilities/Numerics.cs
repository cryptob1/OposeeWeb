using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace OposeeLibrary.Utilities
{
    public static class Numerics
    {
        public static int GetInt(this object input)
        {
            if (input == null || input + "" == "") return 0;
            try
            {
                return Convert.ToInt32(input);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static string IntToString(this int number)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";
            return number.ToString("#,##0");
        }
        public static decimal AddDecimal(params decimal?[] values)
        {
            decimal sum = 0;
            foreach (var item in values)
            {
                if (item.HasValue) sum += item.Value;
            }
            return sum;
        }
        public static string DecimalToString(decimal? value, int decimalPoints)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";
            nfi.NumberDecimalDigits = decimalPoints;
            if (value.HasValue)
            {
                if (value.Value < 0)
                    return "(" + (value.Value * -1).ToString("N", nfi) + ")";
                else
                    return value.Value.ToString("N", nfi);
            }
            return "";
        }
        public static string DecimalToString(decimal? value, int decimalPoints, string zeroChar)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";
            nfi.NumberDecimalDigits = decimalPoints;
            if (value.HasValue)
            {
                if (value.Value == 0) return zeroChar;
                else if (value.Value < 0)
                    return "(" + (value.Value * -1).ToString("N", nfi) + ")";
                else
                    return value.Value.ToString("N", nfi);
            }
            return "";
        }
        public static string DecimalToString(decimal? value)
        {
            return DecimalToString(value, 2);
        }
        public static string DecimalToString(decimal? value, string zeroChar)
        {
            return DecimalToString(value, 2, zeroChar);
        }
        public static string DecimalToString(decimal? debit, decimal? credit)
        {
            if (debit.HasValue && debit.Value > 0)
                return DecimalToString(debit);
            return DecimalToString(credit);
        }
        public static decimal GetDecimal(this object input)
        {
            if (input == null || input + "" == "") return 0;
            try
            {
                return Convert.ToDecimal(input);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static double GetDouble(this object input)
        {
            if (input == null || input + "" == "") return 0;
            try
            {
                return Convert.ToDouble(input);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static bool GetBool(this string input)
        {
            if (input == null || input + "" == "") return false;
            try
            {
                return Convert.ToBoolean(input);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string DateToString(this DateTime? dt)
        {
            if (dt.HasValue)
                return dt.Value.ToString("MM/dd/yyyy");
            return "";
        }
        public static DateTime? GetFromDateTime(this string datetime)
        {
            DateTime _value;
            if (DateTime.TryParse(datetime, out _value))
            {
                DateTime _date = Convert.ToDateTime(datetime + " 00:00:00");
                return _date;
            }
            else
                return null;
        }
        public static DateTime? GetToDateTime(this string datetime)
        {
            DateTime _value;
            if (DateTime.TryParse(datetime, out _value))
            {
                DateTime _date = Convert.ToDateTime(datetime + " 23:59:59");
                return _date;
            }
            else
                return null;
        }
    }
}
