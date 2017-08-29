using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	[Table("Parties")]
	public class Party
	{
		[ PrimaryKey, IdentityColumn]
		public int  PartyID
		{
			get;
			set;
		}
		
		public int GameID
		{
			get;
			set;
		}
		
		public string Description
		{
			get;
			set;
		}
		
		public int UserID
		{
			get;
			set;
		}
		
		public byte MinPlayers
		{
			get;
			set;
		}
		
		public byte MaxPlayers
		{
			get;
			set;
		}

		
		public string State
		{
			get;
			set;
		}

	}
}
