using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseModelLib
{
	public interface IRelation:IRevision
	{
		string Name
		{
			get;
		}

		IColumn PrimaryColumn
		{
			get;
		}

		IColumn ForeignColumn
		{
			get;
		}

		DeleteReferentialAction DeleteReferentialAction
		{
			get;
		}

		
	}
}
