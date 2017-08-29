using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseModelLib
{
	[AttributeUsage(AttributeTargets.Field)]
	public class RevisionAttribute:Attribute
	{
		private int value;
		public int Value
		{
			get { return value; }
		}

		public RevisionAttribute(int Value)
		{
			this.value = Value;
		}
	}
}
