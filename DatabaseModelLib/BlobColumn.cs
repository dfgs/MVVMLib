using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatabaseModelLib
{
	public class BlobColumn<ModelType> : BaseColumn<ModelType, byte[]>
	{
		public override Type DataType
		{
			get { return typeof(byte[]); }
		}


		public BlobColumn([CallerMemberName]string Name = null):base(Name)
		{
		
		}



		


	}
}
