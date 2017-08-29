using DatabaseModelLib;
using DatabaseModelLib.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModelLib;

namespace DatabaseViewModelLib
{
	public interface IDatabaseViewModel:IViewModel
	{
		/*bool IsConnected
		{
			get;
		}*/

		Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, Filter<DataType> Filter, IEnumerable<IColumn<DataType>> Orders);
		Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, string SQL);
		Task<bool> InsertAsync<DataType>(DataType Item);
		Task<bool> UpdateAsync<DataType>(DataType Item);
		Task<bool> DeleteAsync<DataType>(DataType Item);


	}
}
