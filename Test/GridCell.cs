using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	[ Table("GridCells")]
	public class GridCell:Cell
	{
		[ PrimaryKey, IdentityColumn]
		public int GridCellID
		{
			get;
			set;
		}
		
		public int Column
		{
			get;
			set;
		}
		
		public int Row
		{
			get;
			set;
		}


	}
}
