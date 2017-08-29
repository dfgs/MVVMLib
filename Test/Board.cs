using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	[Table("Boards")]
	public abstract class Board
	{
		[ PrimaryKey, IdentityColumn]
		public int BoardID
		{
			get;
			set;
		}
		
		public int PartyID
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
