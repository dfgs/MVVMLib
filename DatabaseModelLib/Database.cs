
using DatabaseModelLib.Filters;
using ModelLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DatabaseModelLib
{
	public abstract class Database<ConnectionType,CommandType>:IDatabase<ConnectionType,CommandType>
		where ConnectionType:DbConnection
		where CommandType:DbCommand,new()
	{

		private int maxRevision;

		private List<ITable> tables;
		public IEnumerable<ITable> Tables
		{
			get { return tables; }
		}

		private List<IRelation> relations;
		public IEnumerable<IRelation> Relations
		{
			get { return relations; }
		}

		private string name;
		public string Name
		{
			get { return name; }
		}

		public Database()
		{
			FieldInfo[] fis;
			DatabaseAttribute databaseAttribute;
			ITable table;
			IRelation relation;
			Type databaseType;
			int revision;
			RevisionAttribute revisionAttribute;

			maxRevision = int.MaxValue;

			databaseType = GetType();

			tables = new List<ITable>();
			relations = new List<IRelation>();

			databaseAttribute = Utils.GetCustomAttribute<DatabaseAttribute>(databaseType);
			name = databaseAttribute?.Name ?? databaseType.Name;

			fis = databaseType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo fi in fis)
			{
				revisionAttribute = Utils.GetCustomAttribute<RevisionAttribute>(fi);
				revision = revisionAttribute?.Value??0;

				table = fi.GetValue(null) as ITable;
				if (table != null)
				{
					table.Revision = revision;
					tables.Add(table);
				}

				relation = fi.GetValue(null) as IRelation;
				if (relation != null)
				{
					relation.Revision = revision;
					relations.Add(relation);
				}
			}

		}

		public void SetMaxRevision(int Revision)
		{
			this.maxRevision=Revision;
		}

		protected abstract ConnectionType OnCreateConnection();

		protected abstract void OnSetParameter<DataType>(CommandType Command, string Name, object Value);


		protected virtual string OnCreateEqualFilter<DataType>(EqualFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) return OnFormatColumnName(Filter.Column) + " is null";
			else return OnFormatColumnName(Filter.Column) + " = " + parameter.Item1 ;
		}
		protected virtual string OnCreateGreaterFilter<DataType>(GreaterFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " > " + parameter.Item1;
		}
		protected virtual string OnCreateLowerFilter<DataType>(LowerFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " < " + parameter.Item1;
		}
		protected virtual string OnCreateGreaterOrEqualFilter<DataType>(GreaterOrEqualFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " >= " + parameter.Item1;
		}
		protected virtual string OnCreateLowerOrEqualFilter<DataType>(LowerOrEqualFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " <= " + parameter.Item1;
		}
		protected virtual string OnCreateAndFilter<DataType>(AndFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			string result;

			if ((Filter.Filters == null) || (Filter.Filters.Length == 0)) throw (new ArgumentNullException("Filters")); ;

			result = "(" + OnCreateFilter<DataType>(Filter.Filters[0],Parameters) + ")";
			for (int t = 1; t < Filter.Filters.Length; t++)
			{
				result += " AND (" + OnCreateFilter<DataType>(Filter.Filters[t], Parameters) + ")";
			}

			return result;
		}
		protected virtual string OnCreateOrFilter<DataType>(OrFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			string result;

			if ((Filter.Filters == null) || (Filter.Filters.Length == 0)) throw (new ArgumentNullException("Filters")); ;

			result = "(" + OnCreateFilter<DataType>(Filter.Filters[0], Parameters) + ")";
			for (int t = 1; t < Filter.Filters.Length; t++)
			{
				result += " OR (" + OnCreateFilter<DataType>(Filter.Filters[t], Parameters) + ")";
			}

			return result;
		}

		private string OnCreateFilter<DataType>(Filter<DataType> Filter,List<Tuple<string,object>> Parameters)
		{

			if (Filter is EqualFilter<DataType>) return OnCreateEqualFilter((EqualFilter<DataType>)Filter,Parameters);
			else if (Filter is GreaterFilter<DataType>) return OnCreateGreaterFilter((GreaterFilter<DataType>)Filter, Parameters);
			else if (Filter is LowerFilter<DataType>) return OnCreateLowerFilter((LowerFilter<DataType>)Filter, Parameters);
			else if (Filter is GreaterOrEqualFilter<DataType>) return OnCreateGreaterOrEqualFilter((GreaterOrEqualFilter<DataType>)Filter, Parameters);
			else if (Filter is LowerOrEqualFilter<DataType>) return OnCreateLowerOrEqualFilter((LowerOrEqualFilter<DataType>)Filter, Parameters);
			else if (Filter is AndFilter<DataType>) return OnCreateAndFilter((AndFilter<DataType>)Filter, Parameters);
			else if (Filter is OrFilter<DataType>) return OnCreateOrFilter((OrFilter<DataType>)Filter, Parameters);

			else throw (new InvalidOperationException("Not supported filter"));
		}


		protected virtual string OnFormatColumnName(IColumn Column)
		{
			return "[" + Column.Name + "]";
		}
		protected virtual string OnFormatTableName(string TableName)
		{
			return "[" + TableName + "]";
		}

		
		protected virtual string OnCreateParameterName<DataType>(IColumn<DataType> Column,int Index)
		{
			return "@_" + Column.Name+Index.ToString();
		}
		
		protected virtual object OnConvertToDbValue<DataType>(IColumn<DataType> Column,DataType Component)
		{
			object value;

			value = Column.GetValue(Component);
			if (value == null) return DBNull.Value;

			if (Column.DataType == typeof(Text)) return value.ToString();

			return value;
		}
		protected virtual object OnConvertFromDbValue<DataType>(IColumn<DataType> Column, object Value)
		{
			if (Value == DBNull.Value) return null;
			return Value;

		}

		protected abstract CommandType OnCreateIdentityCommand<DataType>();
		protected abstract CommandType OnCreateTableCreateCommand(ITable Table,int Revision);
		protected abstract CommandType OnCreateColumnCreateCommand(IColumn Column);
		protected abstract CommandType OnCreateTableDropCommand(ITable Table);
		protected abstract CommandType OnCreateRelationCreateCommand(IRelation Relation);

		protected abstract Task<bool> OnExistsAsync();
		protected abstract Task OnCreateAsync();
		protected abstract Task OnDropAsync();
		protected abstract Task OnBackupAsync(string Path);


		private CommandType CreateSelectCommand<DataType>(Filter<DataType> Filter, IEnumerable<IColumn<DataType>> Orders,int Revision)
		{
			string sql;
			CommandType command;
			IColumn<DataType>[] orders;
			List<Tuple<string, object>> parameters;;

			parameters = new List<Tuple<string, object>>();

			if (Orders == null) orders = new IColumn<DataType>[] { };
			else orders = Orders.ToArray();

			sql = "select " + Utils.Join(Schema<DataType>.Columns.Where(item=>(item.Revision<=Revision) && (!item.IsVirtual)), ", ", item => OnFormatColumnName(item)) + " from "+ OnFormatTableName(Schema<DataType>.TableName);

			if (Filter!=null) sql += " where " + OnCreateFilter(Filter,parameters);
			

			if (orders.Length > 0)
			{
				sql += " order by " + OnFormatColumnName(orders[0]);
				for (int t = 1; t < orders.Length; t++)
				{
					sql += ", " + OnFormatColumnName(orders[t]);
				}
			}

			command = new CommandType();
			command.CommandText = sql;

			foreach (Tuple<string, object> parameter in parameters)
			{
				if (parameter.Item2 == null) continue;
				OnSetParameter<DataType>(command, parameter.Item1,parameter.Item2);
			}

			return command;
		}
		private CommandType CreateUpdateCommand<DataType>(DataType Item, int Revision)
		{
			string sql;
			CommandType command;
			IEnumerable<IColumn<DataType>> columns;

			columns = Schema<DataType>.Columns.Where(item => (!item.IsPrimaryKey) && (!item.IsIdentity) && (item.Revision <= maxRevision) && (!item.IsVirtual));

			sql = "update " + OnFormatTableName(Schema<DataType>.TableName) + " set " +
				Utils.Join(columns, ",",
				item => OnFormatColumnName(item) + "=" + OnCreateParameterName(item,0));
			sql += " where " + OnFormatColumnName(Schema<DataType>.PrimaryKey) + "=" + OnCreateParameterName(Schema<DataType>.PrimaryKey, 1); 

			command = new CommandType();
			command.CommandText = sql;

			foreach (IColumn<DataType> column in columns)
			{
				OnSetParameter<DataType>(command, OnCreateParameterName(column,0), OnConvertToDbValue(column,Item) );
			}
			OnSetParameter<DataType>(command, OnCreateParameterName(Schema<DataType>.PrimaryKey, 1), OnConvertToDbValue( Schema<DataType>.PrimaryKey,Item));

			return command;
		}
		private CommandType CreateInsertCommand<DataType>(DataType Item,int Revision)
		{
			string sql;
			CommandType command;
			IEnumerable<IColumn<DataType>> columns;

			columns = Schema<DataType>.Columns.Where(item => ((!item.IsIdentity) && (item.Revision<=Revision) && (!item.IsVirtual)));

			sql = "insert into " + OnFormatTableName(Schema<DataType>.TableName) + " (" + Utils.Join(columns, ", ", item => OnFormatColumnName(item)) + ") values (" + Utils.Join(columns, ",", item => OnCreateParameterName(item,0) ) + ")";

			command = new CommandType();
			command.CommandText = sql;
			foreach (IColumn<DataType> column in columns)
			{
				OnSetParameter<DataType>(command, OnCreateParameterName(column, 0), OnConvertToDbValue( column,Item) );
			}

			return command;
		}
		private CommandType CreateIdentityCommand<DataType>()
		{
			return OnCreateIdentityCommand<DataType>();
		}
		private CommandType CreateDeleteCommand<DataType>(object Key)
		{
			string sql;
			CommandType command;

			sql = "delete from " + OnFormatTableName(Schema<DataType>.TableName);
			sql += " where " + OnFormatColumnName(Schema<DataType>.PrimaryKey) + "=" + OnCreateParameterName(Schema<DataType>.PrimaryKey,0);

			command = new CommandType();
			command.CommandText = sql;
			OnSetParameter<DataType>(command, OnCreateParameterName(Schema<DataType>.PrimaryKey, 0), Key);
			
			return command;
		}
		private CommandType CreateDeleteCommand<DataType>(DataType Item)
		{
			return CreateDeleteCommand<DataType>(OnConvertToDbValue(Schema<DataType>.PrimaryKey, Item));
		}


		public async Task CreateColumnAsync(IColumn Column)
		{
			ConnectionType connection;
			CommandType command;

			connection = null;
			try
			{

				connection = OnCreateConnection();
				await connection.OpenAsync();

				command = OnCreateColumnCreateCommand(Column);
				command.Connection = connection;

				await command.ExecuteNonQueryAsync();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (connection != null) connection.Close();
			}
		}

		public async Task CreateTableAsync(ITable Table)
		{
			ConnectionType connection;
			CommandType command;

			connection = null;
			try
			{

				connection = OnCreateConnection();
				await connection.OpenAsync();

				command = OnCreateTableCreateCommand(Table,maxRevision);
				command.Connection = connection;

				await command.ExecuteNonQueryAsync();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (connection != null) connection.Close();
			}
		}

		public async Task DropTableAsync(ITable Table)
		{
			ConnectionType connection;
			CommandType command;

			connection = null;
			try
			{

				connection = OnCreateConnection();
				await connection.OpenAsync();

				command = OnCreateTableDropCommand(Table);
				command.Connection = connection;

				await command.ExecuteNonQueryAsync();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (connection != null) connection.Close();
			}
		}

		public async Task CreateRelationAsync(IRelation Relation)
		{
			ConnectionType connection;
			CommandType command;

			connection = null;
			try
			{

				connection = OnCreateConnection();
				await connection.OpenAsync();

				command = OnCreateRelationCreateCommand(Relation);
				command.Connection = connection;

				await command.ExecuteNonQueryAsync();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (connection != null) connection.Close();
			}
		}

		public async Task<bool> ExistsAsync()
		{
			return await OnExistsAsync();
		}

		public async Task DropAsync()
		{

			try
			{
				await OnDropAsync();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
		}

		public async Task CreateAsync()
		{
			try
			{
				
				await OnCreateAsync();


				foreach(ITable table in tables.Where(item=>item.Revision<=maxRevision))
				{
					await CreateTableAsync(table);
				}

				foreach (IRelation relation in relations.Where(item => item.Revision <= maxRevision))
				{
					await CreateRelationAsync(relation);
				}

				await OnCreatedAsync();

			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
			}

		}

		protected virtual async Task OnCreatedAsync()
		{
			await Task.Yield();
		} 

		public async Task BackupAsync(string Path)
		{
			try
			{

				await OnBackupAsync(Path);

			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
			}
		}








		public async Task InsertAsync<DataType>(DataType Item)
		{
			CommandType command;
			CommandType identityCommand;
			int identity;
			IColumn<DataType> identityColumn;
			ConnectionType connection=null;
			DbTransaction transaction=null;

			try
			{
				connection = OnCreateConnection();
				await connection.OpenAsync();
				transaction = connection.BeginTransaction();

				command = CreateInsertCommand<DataType>(Item,maxRevision);
				command.Connection = connection;
				command.Transaction = transaction;
					
				await command.ExecuteNonQueryAsync();

				identityColumn = Schema<DataType>.IdentityColumn;
				if (identityColumn != null)
				{
					identityCommand = CreateIdentityCommand<DataType>();
					identityCommand.Connection = connection;
					identityCommand.Transaction = transaction;

					identity = Convert.ToInt32(await identityCommand.ExecuteScalarAsync());
					identityColumn.SetValue(Item, identity);
				}
				
				transaction.Commit();
			}
			catch (Exception ex)
			{
				if (transaction!=null) transaction.Rollback();
				throw (ex);
			}
			finally
			{
				if (connection != null) connection.Close();
			}


		}

		public async Task UpdateAsync<DataType>(DataType Item)
		{
			DbCommand command;
			ConnectionType connection = null;

			try
			{
				connection = OnCreateConnection();
				await connection.OpenAsync();


				command = CreateUpdateCommand<DataType>(Item, maxRevision);
				command.Connection = connection;

				await command.ExecuteNonQueryAsync();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (connection != null) connection.Close();
			}
		}

		public async Task DeleteAsync<DataType>(DataType Data)
		{
			DbCommand command;
			ConnectionType connection = null;


			try
			{
				connection = OnCreateConnection();
				await connection.OpenAsync();

				command = CreateDeleteCommand<DataType>(Data);
				command.Connection = connection;
				await command.ExecuteNonQueryAsync();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (connection != null) connection.Close();
			}


		}

		public async Task DeleteAsync<DataType, KeyType>(KeyType Key)
		{
			DbCommand command;
			ConnectionType connection = null;

			try
			{
				connection = OnCreateConnection();
				await connection.OpenAsync();

				command = CreateDeleteCommand<DataType>(Key);
				command.Connection = connection;
				await command.ExecuteNonQueryAsync();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (connection != null) connection.Close();
			}


		}

		public async Task ExecuteAsync(CommandType Command)
		{
			ConnectionType connection = null;

			try
			{
				connection = OnCreateConnection();
				await connection.OpenAsync();

				Command.Connection = connection;

				await Command.ExecuteNonQueryAsync();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (connection != null) connection.Close();
			}


		}

		public async Task<ValType?> ExecuteAsync<ValType>(CommandType Command)
			where ValType:struct
		{
			ConnectionType connection = null;
			ValType? result;

			try
			{
				connection = OnCreateConnection();
				await connection.OpenAsync();

				Command.Connection = connection;

				result = (ValType?)await Command.ExecuteScalarAsync();
				return result;
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (connection != null) connection.Close();
			}


		}

		public async Task<IEnumerable<DataType>> SelectAsync<DataType>()
			where DataType : new()
		{
			return await SelectAsync<DataType>(() => { return new DataType(); },null,null);
		}
		public async Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor)
		{
			return await SelectAsync<DataType>(DataConstructor, null, null);
		}
		public async Task<IEnumerable<DataType>> SelectAsync<DataType>(Filter<DataType> Filter, params IColumn<DataType>[] Orders)
			where DataType : new()
		{
			return await SelectAsync<DataType>(() => { return new DataType(); }, Filter,Orders);
		}
		public async Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, Filter<DataType> Filter, params IColumn<DataType>[] Orders)
		{
			CommandType command;

			command = CreateSelectCommand<DataType>(Filter, Orders, maxRevision);
			return await SelectAsync<DataType>(DataConstructor, command);
		}

		public async Task<IEnumerable<DataType>> SelectAsync<DataType>(CommandType Command)
			where DataType:new()
		{
			return await SelectAsync<DataType>(() => { return new DataType(); }, Command);
		}
		public async Task<IEnumerable<DataType>> SelectAsync<DataType>(Func<DataType> DataConstructor, CommandType Command)
		{
			DataType data;
			List<DataType> results;
			DbDataReader reader;
			ConnectionType connection = null;
			int index;

			results = new List<DataType>();

			try
			{
				connection = OnCreateConnection();
				await connection.OpenAsync();

				Command.Connection = connection;

				reader = await Command.ExecuteReaderAsync();
				while (reader.Read())
				{
					data = DataConstructor();
					foreach (IColumn<DataType> column in Schema<DataType>.Columns.Where(item => (item.Revision <= maxRevision)  ))
					{
						try
						{
							index = reader.GetOrdinal(column.Name);
						}
						catch
						{
							continue;
						}
						column.SetValue(data, OnConvertFromDbValue(column, reader[index]));
					}
					results.Add(data);
				}
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (connection != null) connection.Close();
			}

			return results;// Task.FromResult<IEnumerable<DataType>>(results);
		}













	}
}



