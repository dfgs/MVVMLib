using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatabaseModelLib
{
	public abstract class BaseColumn<ModelType,ValueType>: IColumn<ModelType>
	{
		public string TableName
		{
			get { return Schema<ModelType>.TableName; }
		}

		private string name;
		public string Name
		{
			get { return name; }
		}

		private bool isIdentity;
		public bool IsIdentity
		{
			get { return isIdentity; }
			set { isIdentity = value; }
		}

		private bool isNullable;
		public bool IsNullable
		{
			get { return isNullable; }
			set { isNullable = value; }
		}

		private ValueType defaultValue;
		public ValueType DefaultValue
		{
			get { return defaultValue; }
			set { defaultValue = value; }
		}

		object IColumn.DefaultValue
		{
			get { return defaultValue; }
		}

		private bool isPrimaryKey;
		public bool IsPrimaryKey
		{
			get { return isPrimaryKey; }
			set { isPrimaryKey = value; }
		}

		private bool isVirtual;
		public bool IsVirtual
		{
			get { return isVirtual; }
			set { isVirtual = value; }
		}

		
		public abstract Type DataType
		{
			get;
		}

		int IRevision.Revision
		{
			get;
			set;
		}


		private TypeConverter converter;
		public TypeConverter Converter
		{
			get { return converter; }
			set { converter = value; }
		}

		private static Regex nameRegex = new Regex(@"^(.*)Column$");


		/*private PropertyInfo propertyInfo;
		public PropertyInfo PropertyInfo
		{
			get { return propertyInfo; }
		}*/

		private Dictionary<ModelType, ValueType> values;


		public BaseColumn(string Name)
		{
			Match match;

			//dataType = typeof(ValueType);

			values = new Dictionary<ModelType, ValueType>();

			match = nameRegex.Match(Name);
			if (match.Success) name = match.Groups[1].Value;
			else name = Name;

			converter = TypeDescriptor.GetConverter(typeof(ValueType));

			defaultValue = default(ValueType);
		}



		public override string ToString()
		{
			return name;
		}


		object IColumn<ModelType>.GetValue(ModelType Component)
		{
			return GetValue(Component);
		}





		void IColumn<ModelType>.SetValue(ModelType Component, object Value)
		{
			object convertedValue;

			if ((Value != null) && (Value.GetType() == DataType)) convertedValue = Value;
			else convertedValue = Converter.ConvertFrom(Value);
			SetValue(Component, (ValueType)convertedValue);


		}

		public void SetValue(ModelType Component, ValueType Value)
		{
			if (Component == null) return;
			if (values.ContainsKey(Component)) values[Component] = Value;
			else values.Add(Component, Value);
		}
		public ValueType GetValue(ModelType Component)
		{
			ValueType value;
			if (Component == null) return default(ValueType);
			if (!values.TryGetValue(Component, out value)) return defaultValue;
			return value;
		}


	}
}
