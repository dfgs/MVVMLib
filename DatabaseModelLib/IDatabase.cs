using DatabaseModelLib.Filters;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

		Task CreateTableAsync(ITable Table);
		Task DropTableAsync(ITable Table);
		Task CreateRelationAsync(IRelation Relation);
		Task CreateColumnAsync(IColumn Column);


		Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor);
		Task<IEnumerable<DataType>> SelectAsync<DataType>()
			where DataType : new();
		Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, Filter<DataType> Filter, params IColumn<DataType>[] Orders);
		Task<IEnumerable<DataType>> SelectAsync<DataType>(Filter<DataType> Filter, params IColumn<DataType>[] Orders)
			where DataType : new();

		/*Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, Filter<DataType> Filter);
		Task<IEnumerable<DataType>> SelectAsync<DataType>(Filter<DataType> Filter)
			where DataType : new();*/


		Task InsertAsync<DataType>(DataType Item);
		Task UpdateAsync<DataType>(DataType Item);
		Task DeleteAsync<DataType>(DataType Item);
		Task DeleteAsync<DataType,PrimaryKeyType>(PrimaryKeyType ItemID);

		Task<bool> ExistsAsync();
		Task CreateAsync();
		Task BackupAsync(string Path);

		void SetMaxRevision(int Revision);

	}

	public interface IDatabase<ConnectionType, CommandType> : IDatabase
		where ConnectionType : DbConnection
		where CommandType : DbCommand, new()
	{
		Task<ValType?> ExecuteAsync<ValType>(CommandType Command)
			where ValType : struct;
		Task ExecuteAsync(CommandType Command);
		Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, CommandType Command);
		Task<IEnumerable<DataType>> SelectAsync<DataType>(CommandType Command)
			where DataType : new();

	}

}
