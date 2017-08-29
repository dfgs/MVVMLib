using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	[Table("Games")]
	public class Game
	{
		[ PrimaryKey, IdentityColumn]
		public int GameID
		{
			get;
			set;
		}
		
		public string Name
		{
			get;
			set;
		}
		
		public string Description
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
		public string PluginPath
		{
			get;
			set;
		}

	}
}
