using System;
using System.Globalization;

namespace TheSaga.Persistance.SqlServer.Utils
{

    public static class UniConvert
    {
        public static Char? decimalSeparator;
        public static Char DECIMAL_SEPARATOR
        {
            get
            {
                if (!decimalSeparator.HasValue)
                {
                    decimalSeparator = "0.1".Replace("0", "").Replace("1", "")[0];
                }
                return decimalSeparator.Value;
            }
        }

        ////////////////////////////////////

        public static String ToString(Object V)
        {
            if (V == null) return null;
            return (String)Convert.ToString(V, CultureInfo.InvariantCulture);
        }
        public static Boolean? ToBooleanN(Object V)
        {
            if (V == null) return null;
            else if (V is String) return (Boolean?)ParseUniBoolean(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return (Boolean?)Convert.ToBoolean(V, CultureInfo.InvariantCulture);
        }
        public static Boolean ToBoolean(Object V)
        {
            if (V is String) return ParseUniBoolean(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return Convert.ToBoolean(V, CultureInfo.InvariantCulture);
        }
        public static Boolean ParseUniBoolean(String Text)
        {
            return Boolean.Parse(Text);
        }
        public static Boolean TryParseUniBoolean(String Text)
        {
            Boolean lTmp; return TryParseUniBoolean(Text, out lTmp);
        }
        public static Boolean TryParseUniBoolean(String Text, out Boolean Value)
        {
            return Boolean.TryParse(Text, out Value);
        }
        public static String ToUniString(this Boolean Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        ////////////////////////////////////

        public static Guid? ToGuidN(Object V)
        {
            if (V == null) return null;
            else if (V is String) return (Guid?)ParseUniGuid(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return (Guid?)V;
        }
        public static Guid ToGuid(Object V)
        {
            if (V is String) return ParseUniGuid(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return (Guid)V;
        }
        public static Guid ParseUniGuid(String Text)
        {
            return new Guid(Text);
        }
        public static Boolean TryParseUniGuid(String Text)
        {
            Guid lTmp; return TryParseUniGuid(Text, out lTmp);
        }
        public static Boolean TryParseUniGuid(String Text, out Guid Value)
        {
            try { Value = new Guid(Text); return true; }
            catch { Value = Guid.Empty; return false; }
        }
        public static String ToUniString(this Guid Value)
        {
            return Value.ToString();
        }

        ////////////////////////////////////

        public static Int64? ToInt64N(Object V)
        {
            if (V == null) return null;
            else if (V is String) return (Int64?)ParseUniInt64(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return (Int64?)Convert.ToInt64(V, CultureInfo.InvariantCulture);
        }
        public static Int64 ToInt64(Object V)
        {
            if (V is String) return ParseUniInt64(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return Convert.ToInt64(V, CultureInfo.InvariantCulture);
        }
        public static Int64 ParseUniInt64(String Text)
        {
            return Int64.Parse(Text, CultureInfo.InvariantCulture);
        }
        public static Boolean TryParseUniInt64(String Text)
        {
            Int64 lTmp; return TryParseUniInt64(Text, out lTmp);
        }
        public static Boolean TryParseUniInt64(String Text, out Int64 Value)
        {
            return Int64.TryParse(Text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out Value);
        }
        public static String ToUniString(this Int64 Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        ////////////////////////////////////

        public static Int32? ToInt32N(Object V)
        {
            if (V == null) return null;
            else if (V is String) return (Int32?)ParseUniInt32(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return (Int32?)Convert.ToInt32(V, CultureInfo.InvariantCulture);
        }
        public static Int32 ToInt32(Object V)
        {
            if (V is String) return ParseUniInt32(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return Convert.ToInt32(V, CultureInfo.InvariantCulture);
        }
        public static Int32 ParseUniInt32(String Text)
        {
            return Int32.Parse(Text, CultureInfo.InvariantCulture);
        }
        public static Boolean TryParseUniInt32(String Text)
        {
            Int32 lTmp; return TryParseUniInt32(Text, out lTmp);
        }
        public static Boolean TryParseUniInt32(String Text, out Int32 Value)
        {
            return Int32.TryParse(Text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out Value);
        }
        public static String ToUniString(this Int32 Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        ////////////////////////////////////

        public static Double? ToDoubleN(Object V)
        {
            if (V == null) return null;
            else if (V is String) return (Double?)ParseUniDouble(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return (Double?)Convert.ToDouble(V, CultureInfo.InvariantCulture);
        }
        public static Double ToDouble(Object V)
        {
            if (V is String) return ParseUniDouble(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return Convert.ToDouble(V, CultureInfo.InvariantCulture);
        }
        public static Double ParseUniDouble(String Text)
        {
            return Double.Parse(Text, CultureInfo.InvariantCulture);
        }
        public static Boolean TryParseUniDouble(String Text)
        {
            Double lTmp; return TryParseUniDouble(Text, out lTmp);
        }
        public static Boolean TryParseUniDouble(String Text, out Double Value)
        {
            return Double.TryParse(Text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out Value);
        }
        public static String ToUniString(this Double Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        ////////////////////////////////////

        public static Boolean IsNumber(String Text)
        {
            if (Text == null)
            {
                return false;
            }
            else
            {
                decimal v = 0;
                if (Decimal.TryParse(Text, NumberStyles.Number, CultureInfo.InvariantCulture, out v))
                    return true;
                return false;
            }
        }
        public static Decimal? ToNumber(Object V)
        {
            if (V == null) return null;
            else if (V is String)
            {
                if (TryParseUniBoolean((String)V))
                    return ToBooleanN(V) == true ? 1M : 0M;
                return (Decimal?)ParseUniDecimal(Convert.ToString(V, CultureInfo.InvariantCulture));
            }
            else
            {
                return (Decimal?)Convert.ToDecimal(V, CultureInfo.InvariantCulture);
            }
        }
        public static Decimal? ToDecimalN(Object V)
        {
            if (V == null) return null;
            else if (V is String) return (Decimal?)ParseUniDecimal(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return (Decimal?)Convert.ToDecimal(V, CultureInfo.InvariantCulture);
        }
        public static Decimal ToDecimal(Object V)
        {
            if (V is String) return ParseUniDecimal(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return Convert.ToDecimal(V, CultureInfo.InvariantCulture);
        }
        public static Decimal ParseUniDecimal(String Text)
        {
            return Decimal.Parse(Text, CultureInfo.InvariantCulture);
        }
        public static Boolean TryParseUniDecimal(String Text)
        {
            Decimal lTmp; return TryParseUniDecimal(Text, out lTmp);
        }
        public static Boolean TryParseUniDecimal(String Text, out Decimal Value)
        {
            return Decimal.TryParse(Text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out Value);
        }
        public static String ToUniString(this Decimal Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        ////////////////////////////////////

        public static DateTime? ToDateTimeN(Object V)
        {
            if (V == null) return null;
            else if (V is String) return (DateTime?)ParseUniDateTime(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return (DateTime?)Convert.ToDateTime(V, CultureInfo.InvariantCulture);
        }
        public static DateTime ToDateTime(Object V)
        {
            if (V is String) return ParseUniDateTime(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return Convert.ToDateTime(V, CultureInfo.InvariantCulture);
        }
        public static DateTime ParseUniDateTime(String Text)
        {
            return DateTime.Parse(Text, CultureInfo.InvariantCulture);
        }
        public static Boolean TryParseUniDateTime(String Text)
        {
            DateTime lTmp; return TryParseUniDateTime(Text, out lTmp);
        }
        public static Boolean TryParseUniDateTime(String Text, out DateTime Value)
        {
            try { Value = DateTime.Parse(Text, CultureInfo.InvariantCulture); return true; }
            catch { }
            Value = default(DateTime);
            return false;
        }
        public static String ToUniString(this DateTime Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        ////////////////////////////////////

        public static Single? ToSingleN(Object V)
        {
            if (V == null) return null;
            else if (V is String) return (Single?)ParseUniSingle(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return (Single?)Convert.ToSingle(V, CultureInfo.InvariantCulture);
        }
        public static Single ToSingle(Object V)
        {
            if (V is String) return ParseUniSingle(Convert.ToString(V, CultureInfo.InvariantCulture));
            else return Convert.ToSingle(V, CultureInfo.InvariantCulture);
        }
        public static Single ParseUniSingle(String Text)
        {
            return Single.Parse(Text, CultureInfo.InvariantCulture);
        }
        public static Boolean TryParseUniSingle(String Text)
        {
            Single lTmp; return TryParseUniSingle(Text, out lTmp);
        }
        public static Boolean TryParseUniSingle(String Text, out Single Value)
        {
            return Single.TryParse(Text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out Value);
        }
        public static String ToUniString(this Single Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        ////////////////////////////////////

        public static String ToUniDateString(this DateTime Value)
        {
            var lYear = Value.Year.ToString();
            var lMonth = Value.Month.ToString(); if (lMonth.Length == 1) lMonth = "0" + lMonth;
            var lDay = Value.Day.ToString(); if (lDay.Length == 1) lDay = "0" + lDay;
            return String.Format("{0}-{1}-{2}", lYear, lMonth, lDay);
        }

        ////////////////////////////////////

        public static String ToUniString(this Object Value)
        {
            if (Value == null)
            {
                return null;
            }
            else
            {
                if (Value is Single)
                {
                    return ToUniString((Single)Value);
                }
                else if (Value is Single?)
                {
                    return ToUniString(((Single?)Value).Value);
                }
                else if (Value is Double)
                {
                    return ToUniString((Double)Value);
                }
                else if (Value is Double?)
                {
                    return ToUniString(((Double?)Value).Value);
                }
                else if (Value is Decimal)
                {
                    return ToUniString((Decimal)Value);
                }
                else if (Value is Decimal?)
                {
                    return ToUniString(((Decimal?)Value).Value);
                }
                else if (Value is DateTime)
                {
                    return ToUniString((DateTime)Value);
                }
                else if (Value is DateTime?)
                {
                    return ToUniString(((DateTime?)Value).Value);
                }
                else
                {
                    return Convert.ToString(Value, CultureInfo.InvariantCulture);
                }
            }
        }
    }

}