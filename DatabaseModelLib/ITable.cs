using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseModelLib
{
	public interface ITable:IRevision
	{
		string Name
		{
			get;
		}
		IColumn PrimaryKey
		{
			get;
		}
		IColumn IdentityColumn
		{
			get;
		}

		IEnumerable<IColumn> Columns
		{
			get;
		}
		
	}
}
