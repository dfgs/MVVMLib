
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using DatabaseModelLib;
using ModelLib;

namespace SqlCEDatabaseModelLib
{
	public abstract class SqlCEDatabase:Database<SqlCeConnection ,SqlCeCommand>
	{
		private string fileName;
		public string FileName
		{
			get { return fileName; }
		}
		
		

		public SqlCEDatabase(string FileName)
		{
			this.fileName = FileName;
		}

		
		protected override SqlCeConnection OnCreateConnection()
		{
			//MyData.sdf
			return new SqlCeConnection(@"Data Source="+fileName+";Persist Security Info=False;");
		}


		protected override void OnSetParameter<DataType>(SqlCeCommand Command, string Name, object Value)
		{
			Command.Parameters.AddWithValue(Name, Value);
		}

		protected override SqlCeCommand OnCreateIdentityCommand<DataType>()
		{
			return new SqlCeCommand("select @@identity");
		}

		protected override object OnConvertToDbValue<DataType>(IColumn<DataType> Column, DataType Component)
		{
			object value;

			value = Column.GetValue(Component);
			if (value == null) return DBNull.Value;

			if (Column.DataType == typeof(Text)) return value.ToString();
			else if (Column.DataType == typeof(TimeSpan)) return ((TimeSpan)value).Ticks;

			return value;
		}
		protected override object OnConvertFromDbValue<DataType>(IColumn<DataType> Column, object Value)
		{
			if (Value == DBNull.Value) return null;

			if (Column.DataType == typeof(TimeSpan)) return TimeSpan.FromTicks((long)Value) ;
			return Value;
		}

		

		

		

		

		

		

		


		

		


		



	}


}
