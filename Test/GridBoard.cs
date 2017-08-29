using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	[ Table("GridBoards")]
	public class GridBoard:Board
	{
		[ PrimaryKey, IdentityColumn]
		public int GridBoardID
		{
			get;
			set;
		}
		
		public int Columns
		{
			get;
			set;
		}
		
		public int Rows
		{
			get;
			set;
		}


	}
}
