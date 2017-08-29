using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	[Table("Users")]
	public class User
	{
		[PrimaryKey, IdentityColumn]
		public int UserID
		{
			get;
			set;
		}

		public string Login
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		
		public string Pseudo
		{
			get;
			set;
		}

		public string eMail
		{
			get;
			set;
		}

		public bool IsEnabled
		{
			get;
			set;
		}

	}
}
