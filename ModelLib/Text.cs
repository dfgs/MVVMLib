using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib
{
	[TypeConverter(typeof(TextTypeConverter)),DataContract]
	public struct Text : IConvertible, IComparable<Text>, IComparable
	{

		[DataMember]
		private string value;
		public string Value
		{
			get { return value; }
		}

		

		public Text(string Value)
		{
			this.value = Value;
		}

		public static explicit operator string(Text Value)
		{
			return Value.Value;
		}

		public static implicit operator Text(string Value)
		{
			return new Text(Value);
		}

		public override string ToString()
		{
			return value;
		}


		#region IConvertible
		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			if (value == null) return false;
			return ((IConvertible)value).ToBoolean(provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			if (value == null) return 0;
			return ((IConvertible)value).ToByte(provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			if (value == null) return '\0';
			return ((IConvertible)value).ToChar(provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			if (value == null) return DateTime.MinValue;
			return ((IConvertible)value).ToDateTime(provider);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			if (value == null) return 0;
			return ((IConvertible)value).ToDecimal(provider);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			if (value == null) return 0;
			return ((IConvertible)value).ToDouble(provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			if (value == null) return 0;
			return ((IConvertible)value).ToInt16(provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			if (value == null) return 0;
			return ((IConvertible)value).ToInt32(provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			if (value == null) return 0;
			return ((IConvertible)value).ToInt64(provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			if (value == null) return 0;
			return ((IConvertible)value).ToSByte(provider);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			if (value == null) return 0;
			return ((IConvertible)value).ToSingle(provider);
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			if (value == null) return null;
			return ((IConvertible)value).ToString(provider);
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			if (value == null) return null;
			return ((IConvertible)value).ToType(conversionType, provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			if (value == null) return 0;
			return ((IConvertible)value).ToUInt16(provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			if (value == null) return 0;
			return ((IConvertible)value).ToUInt32(provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			if (value == null) return 0;
			return ((IConvertible)value).ToUInt64(provider);
		}
		#endregion


		public int CompareTo(Text Other)
		{
			return NaturalStringComparer.Compare(this.Value, Other.Value);
		}



		public int CompareTo(object obj)
		{
			if (obj is Text) return CompareTo((Text)obj);
			else return 1;
		}

	}
}
