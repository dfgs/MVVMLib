﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatabaseModelLib
{
	public class Column<ModelType,ValueType>:BaseColumn<ModelType,ValueType?>
		where ValueType:struct
	{
		public override Type DataType
		{
			get { return typeof(ValueType); }
		}

		public Column([CallerMemberName]string Name=null):base(Name)
		{
		
		}

		

		

		
	}
}
