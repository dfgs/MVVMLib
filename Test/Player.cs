using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	[Table("Players")]
	public class Player
	{
		[ PrimaryKey, IdentityColumn]
		public int  PlayerID
		{
			get;
			set;
		}
		
		public int PartyID
		{
			get;
			set;
		}
		
		public int UserID
		{
			get;
			set;
		}
		
		public byte Index
		{
			get;
			set;
		}

	}
}
