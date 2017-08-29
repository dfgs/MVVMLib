using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatabaseModelLib
{
	public class Table<DataType>:ITable
	{

		public string Name
		{
			get { return Schema<DataType>.TableName; }
		}

		public IColumn<DataType> PrimaryKey
		{
			get { return Schema<DataType>.PrimaryKey; }
		}

		public IColumn<DataType> IdentityColumn
		{
			get { return Schema<DataType>.IdentityColumn; }
		}


		public IEnumerable<IColumn<DataType>> Columns
		{
			get { return Schema<DataType>.Columns; }
		}

		#region explicit interface
		IColumn ITable.PrimaryKey
		{
			get { return Schema<DataType>.PrimaryKey; }
		}

		IColumn ITable.IdentityColumn
		{
			get { return Schema<DataType>.IdentityColumn; }
		}

		IEnumerable<IColumn> ITable.Columns
		{
			get { return Schema<DataType>.Columns; }
		}
		int IRevision.Revision
		{
			get;
			set;
		}
		#endregion

		public Table()
		{

		}

	}
}
