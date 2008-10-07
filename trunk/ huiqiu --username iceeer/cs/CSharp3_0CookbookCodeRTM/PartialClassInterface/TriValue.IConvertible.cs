using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PartialClassInterfaces
{
    /// Partial class that implements IConvertible
    public partial class TriValue : IConvertible
    {
        public bool ToBoolean(IFormatProvider provider)
        {
            if (Average > 0)
                return true;
            else
                return false;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(Average);
        }

        public char ToChar(IFormatProvider provider)
        {
            decimal val = Average;
            if (val > char.MaxValue)
                val = char.MaxValue;
            if (val < char.MinValue)
                val = char.MinValue;
            return Convert.ToChar((ulong)val);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Average;
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(Average);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(Average);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(Average);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(Average);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(Average);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(Average);
        }

        public string ToString(IFormatProvider provider)
        {
            return string.Format(provider,
                "({0},{1},{2})",
                First.ToString(provider), 
                Second.ToString(provider), 
                Third.ToString(provider));
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Average, conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Average, provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Average,provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Average,provider);
        }
    }
}
