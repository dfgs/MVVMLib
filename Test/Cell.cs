using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	[Table("Cells")]
	public abstract class Cell
	{
		[ PrimaryKey, IdentityColumn]
		public int  CellID
		{
			get;
			set;
		}
		
		public int BoardID
		{
			get;
			set;
		}
		
		public string Fill
		{
			get;
			set;
		}
		
		public string Stroke
		{
			get;
			set;
		}

	}
}
