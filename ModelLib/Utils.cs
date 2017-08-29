using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib
{
	public static class Utils
	{
		public static AttributeType GetCustomAttribute<AttributeType>(Type ComponentType)
			where AttributeType : class
		{
			return ComponentType.GetCustomAttributes(typeof(AttributeType), true).FirstOrDefault() as AttributeType;
		}

		public static AttributeType GetCustomAttribute<AttributeType>(PropertyInfo pi)
			where AttributeType : System.Attribute
		{
			return pi.GetCustomAttribute<AttributeType>();
		}

		public static AttributeType GetCustomAttribute<AttributeType>(FieldInfo fi)
			where AttributeType : System.Attribute
		{
			return fi.GetCustomAttribute<AttributeType>();
		}

		public static string Join<ItemType>(IEnumerable<ItemType> Items, string Separator)
		{
			string result;

			result = "";
			foreach (ItemType item in Items)
			{
				if (result == "") result += item;
				else result += Separator + item;
			}

			return result;
		}

		public static string Join<ItemType>(IEnumerable<ItemType> Items, string Separator, Func<ItemType, string> Transform)
		{
			string result;

			result = "";
			foreach (ItemType item in Items)
			{
				if (result == "") result += Transform(item);
				else result += Separator + Transform(item);
			}

			return result;
		}

	}
}
