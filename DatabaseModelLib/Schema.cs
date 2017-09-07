using ModelLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Reflection;


namespace DatabaseModelLib
{
    public static class Schema<DataType>
    {
		private static IColumn<DataType> primaryKey;
		public static IColumn<DataType> PrimaryKey
		{
			get { return primaryKey; }
		}

		private static IColumn<DataType> identityColumn;
		public static IColumn<DataType> IdentityColumn
		{
			get { return identityColumn; }
		}


		private static string tableName;
		public static string TableName
		{
			get { return tableName; }
		}

		private static List<IColumn<DataType>> columns;
		public static IEnumerable<IColumn<DataType>> Columns
		{
			get { return columns; }
		}


		static Schema()
		{
			FieldInfo[] fis;
			TableAttribute tableAttribute;
			RevisionAttribute revisionAttribute;
			int revision;
			IColumn<DataType> column;
			Type rowType;

			rowType = typeof(DataType);

			columns = new List<IColumn<DataType>>();

			tableAttribute = Utils.GetCustomAttribute<TableAttribute>(rowType);
			tableName = tableAttribute?.Name ?? rowType.Name;

			fis = rowType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo fi in fis)
			{
				revisionAttribute = Utils.GetCustomAttribute<RevisionAttribute>(fi);
				revision = revisionAttribute?.Value ?? 0;

				column = fi.GetValue(null) as IColumn<DataType>;
				if (column == null) continue;
				column.Revision = revision;
				if (column.IsPrimaryKey) primaryKey = column;
				if (column.IsIdentity) identityColumn = column;
				columns.Add(column);
			}
			
			if (primaryKey == null)
				throw (new NotSupportedException("Missing primary key"));

		}

		public static void Clone(DataType Source,DataType Destination)
		{
			object value;
			foreach(IColumn<DataType> column in columns)
			{
				if ((column.IsIdentity) || (column.IsVirtual)) continue;
				value = column.GetValue(Source);
				column.SetValue(Destination, value);
			}
				
		}
		public static bool AreEquals(DataType Source, DataType Destination)
		{
			object value1,value2;
			foreach (IColumn<DataType> column in columns)
			{
				if ((column.IsIdentity) || (column.IsVirtual)) continue;
				value1 = column.GetValue(Source);
				value2 = column.GetValue(Destination);
				if (!ValueType.Equals(value1, value2))
					return false;
			}
			return true;
		}


	}
}
