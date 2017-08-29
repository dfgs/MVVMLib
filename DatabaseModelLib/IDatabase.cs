using DatabaseModelLib.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseModelLib
{
	public interface IDatabase
	{
		IEnumerable<ITable> Tables
		{
			get;
		}
		IEnumerable<IRelation> Relations
		{
			get;
		}

		Task<IEnumerable<DataType>> SelectAsync<DataType>()
			where DataType : new();
		Task<IEnumerable<DataType>> SelectAsync<DataType>(Filter<DataType> Filter)
			where DataType : new();
		Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, Filter<DataType> Filter);

		Task<IEnumerable<DataType>> SelectAsync<DataType>(Filter<DataType> Filter,IEnumerable<IColumn<DataType>> Orders)
			where DataType : new();
		Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, Filter<DataType> Filter, IEnumerable<IColumn<DataType>> Orders);

		Task CreateTableAsync(ITable Table);
		Task DropTableAsync(ITable Table);
		Task CreateRelationAsync(IRelation Relation);
		Task CreateColumnAsync(IColumn Column);



		Task<IEnumerable<DataType>> SelectAsync<DataType>(string SQL)
			where DataType : new();
		Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, string SQL);

		Task ExecuteAsync(string SQL);
		Task InsertAsync<DataType>(DataType Item);
		Task UpdateAsync<DataType>(DataType Item);
		Task DeleteAsync<DataType>(DataType Item);

		Task<bool> ExistsAsync();
		Task CreateAsync();
		Task BackupAsync(string Path);

		void SetMaxRevision(int Revision);

	}
}
