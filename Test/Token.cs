using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	[Table("Tokens")]
	public class Token
	{
		[ PrimaryKey, IdentityColumn]
		public int TokenID
		{
			get;
			set;
		}
		
		public int CellID
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

		
		public string Shape
		{
			get;
			set;
		}

		
		public byte? PlayerIndex
		{
			get;
			set;
		}


	}
}
