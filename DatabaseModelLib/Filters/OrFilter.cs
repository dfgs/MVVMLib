using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseModelLib.Filters
{
	public class OrFilter<DataType>:Filter<DataType>
	{
		private Filter<DataType>[] filters;
		public Filter<DataType>[] Filters
		{
			get { return filters; }
		}

		


		public OrFilter(params Filter<DataType>[] Filters)
		{
			this.filters = Filters;
		}

		public override string ToString()
		{
			string result;

			if ((filters == null) || (filters.Length == 0)) throw (new ArgumentNullException("Filters")); ;

			result = "("+filters[0].ToString()+")";
			for(int t=1;t<filters.Length;t++)
			{
				result += " OR (" + filters[t].ToString()+")";
			}

			return result;
		}


	}
}
