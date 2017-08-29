using DatabaseModelLib;
using DatabaseModelLib.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModelLib;

namespace DatabaseViewModelLib
{
	public abstract class DatabaseViewModel<ModelType> :ViewModel<ModelType>,IDatabaseViewModel
		where ModelType:IDatabase
	{
	
		

		public DatabaseViewModel()
		{
		}

		public async Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, Filter<DataType> Filter,IEnumerable<IColumn<DataType>> Orders)
		{
			try
			{
				return await Model.SelectAsync(DataConstructor, Filter,Orders);
			}
			catch(Exception ex)
			{
				Log(ex.Message);
			}
			return null;
		}
		public async Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, string SQL)
		{
			try
			{
				return await Model.SelectAsync(DataConstructor,SQL );
			}
			catch (Exception ex)
			{
				Log(ex.Message);
			}
			return null;
		}

		public async Task<bool> InsertAsync<DataType>(DataType Item)
		{
			try
			{
				await Model.InsertAsync(Item);
				return true;
			}
			catch (Exception ex)
			{
				Log(ex.Message);
				return false;
			}
		}

		public async Task<bool> UpdateAsync<DataType>(DataType Item)
		{
			try
			{
				await Model.UpdateAsync(Item);
				return true;
			}
			catch (Exception ex)
			{
				Log(ex.Message);
				return false;
			}
		}

		public async Task<bool> DeleteAsync<DataType>(DataType Item)
		{
			try
			{
				await Model.DeleteAsync(Item);
				return true;
			}
			catch (Exception ex)
			{
				Log(ex.Message);
				return false;
			}
		}

		

		
	}
}
