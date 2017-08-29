using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseModelLib
{
	public interface IColumn:IRevision
	{
		string Name
		{
			get;
		}
		string TableName
		{
			get;
		}
		bool IsPrimaryKey
		{
			get;
		}
		bool IsIdentity
		{
			get;
		}
		Type DataType
		{
			get;
		}
		TypeConverter Converter
		{
			get;
		}
		bool IsNullable
		{
			get;
		}
		object DefaultValue
		{
			get;
		}
	
		
	}

	public interface IColumn<DataType>:IColumn
	{
		
		object GetValue(DataType Component);
		void SetValue(DataType Component,object Value);
		//object GetDbValue(DataType Component);
	}
}
