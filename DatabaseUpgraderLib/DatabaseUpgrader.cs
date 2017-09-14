using DatabaseModelLib;
using ModelLib;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseUpgraderLib
{
	
	public abstract class DatabaseUpgrader<ConnectionType, CommandType,TransactionType>:IDatabaseUpgrader
		where ConnectionType : DbConnection
		where CommandType : DbCommand, new()
		where TransactionType:DbTransaction
	{
		private Database<ConnectionType,CommandType> database;
		public Database<ConnectionType, CommandType> Database
		{
			get { return database; }
		}

		private Table<UpgradeLog> upgradeLogs;

		/*private string errorMessage;
		public string ErrorMessage
		{
			get { return errorMessage; }
		}*/

		private IEnumerable<IRevision> RevisionItems
		{
			get
			{
				foreach (ITable revision in database.Tables)
				{
					IColumn pk = revision.PrimaryKey;// initialize static constructor of Schema<>
					yield return revision;
				}
				foreach(IRevision revision in database.Tables.SelectMany(item=>item.Columns))
				{
					yield return revision;
				}
				foreach (IRevision revision in database.Relations)
				{
					yield return revision;
				}
			}
		}

		public DatabaseUpgrader(Database<ConnectionType, CommandType> Database)
		{
			this.database = Database;
			upgradeLogs = new Table<UpgradeLog>();
		}



		protected virtual string OnFormatColumnName(IColumn Column)
		{
			return "[" + Column.Name + "]";
		}
		protected virtual string OnFormatTableName(string TableName)
		{
			return "[" + TableName + "]";
		}

		protected abstract CommandType OnCreateTableCreateCommand(ITable Table);
		protected abstract CommandType OnCreateColumnCreateCommand(IColumn Column);
		protected abstract CommandType OnCreateTableDropCommand(ITable Table);
		protected abstract CommandType OnCreateRelationCreateCommand(IRelation Relation);


		private async Task CreateColumnAsync(ConnectionType Connection, TransactionType Transaction, IColumn Column)
		{
			CommandType command;

			command = OnCreateColumnCreateCommand(Column);
			command.Connection = Connection;
			command.Transaction = Transaction;
			await command.ExecuteNonQueryAsync();
		}

		private async Task CreateTableAsync(ConnectionType Connection, TransactionType Transaction, ITable Table)
		{
			CommandType command;

			command = OnCreateTableCreateCommand(Table);
			command.Connection = Connection;
			command.Transaction = Transaction;
			await command.ExecuteNonQueryAsync();
		}

		private async Task DropTableAsync(ConnectionType Connection, TransactionType Transaction, ITable Table)
		{
			CommandType command;

			command = OnCreateTableDropCommand(Table);
			command.Connection = Connection;
			command.Transaction = Transaction;
			await command.ExecuteNonQueryAsync();
		}

		private async Task CreateRelationAsync(ConnectionType Connection, TransactionType Transaction, IRelation Relation)
		{
			CommandType command;

			command = OnCreateRelationCreateCommand(Relation);
			command.Connection = Connection;
			command.Transaction = Transaction;
			await command.ExecuteNonQueryAsync();
		}



		protected abstract Task<bool> OnDatabaseExistsAsync();
		public async Task<bool> DatabaseExistsAsync()
		{
			return await OnDatabaseExistsAsync();
		}

		protected abstract Task<bool> OnSchemaExistsAsync();
		public async Task<bool> SchemaExistsAsync()
		{
			return await OnSchemaExistsAsync();
		}


		protected abstract Task OnDropAsync();
		public async Task DropAsync()
		{
			await OnDropAsync();
		}

		protected abstract Task OnBackupAsync(string Path);
		public async Task BackupAsync(string Path)
		{
			await OnBackupAsync(Path);
		}

		protected abstract ConnectionType OnCreateConnection();

		protected abstract Task OnCreateDatabaseAsync();

		public async Task CreateDatabaseAsync()
		{
			await OnCreateDatabaseAsync();
		}

		public async Task CreateSchemaAsync()
		{
			ConnectionType connection = null;
			TransactionType transaction = null;

			try
			{
				connection = OnCreateConnection();
				await connection.OpenAsync();
				connection.ChangeDatabase(Database.Name);
				transaction = (TransactionType)connection.BeginTransaction();

				await CreateTableAsync(connection, transaction, upgradeLogs);

				foreach (ITable table in Database.Tables.Where(item => item.Revision == 0))
				{
					await CreateTableAsync(connection, transaction, table);
				}

				foreach (IRelation relation in Database.Relations.Where(item => item.Revision == 0))
				{
					await CreateRelationAsync(connection, transaction, relation);
				}

				foreach(CommandType command in OnGetCustomSchemaCommands())
				{
					command.Connection = connection;
					command.Transaction = transaction;
					await command.ExecuteNonQueryAsync();
				}

				transaction.Commit();
				connection.Close();
			}
			catch (Exception ex)
			{
				if (transaction != null) transaction.Rollback();
				if (connection != null) connection.Close();
				throw (ex);
			}
			finally
			{
			}

		}

		protected virtual IEnumerable<CommandType> OnGetCustomSchemaCommands()
		{
			yield break;
		}




		public async Task<int> GetDatabaseRevisionAsync()
		{
			IEnumerable<UpgradeLog> logs;

			logs = await database.SelectAsync<UpgradeLog>();
			return logs.Max(item => item.Revision) ?? 0;
		}

		public int GetTargetRevision()
		{
			return RevisionItems.Max(item => item.Revision);
		}

		public async Task<bool> NeedsUpgradeAsync()
		{
			return GetTargetRevision() != await GetDatabaseRevisionAsync();
		}

	

		public async Task UpgradeAsync()
		{
			int targetRevision;
			int currentRevision;
			UpgradeLog log;
			ConnectionType connection=null;
			TransactionType transaction = null;

			currentRevision = await GetDatabaseRevisionAsync();
			targetRevision = GetTargetRevision();

			try
			{
				connection = OnCreateConnection();
				await connection.OpenAsync();
				connection.ChangeDatabase(Database.Name);

				while (currentRevision != targetRevision)
				{
					transaction = (TransactionType)connection.BeginTransaction();
					currentRevision++;
					//database.SetMaxRevision(currentRevision);
					await OnUpgradeAsync(connection,transaction, currentRevision);
					log = new UpgradeLog() { Date = DateTime.Now, Revision = currentRevision };

					foreach(CommandType command in OnGetCustomUpgradeCommands(currentRevision))
					{
						command.Connection = connection;
						command.Transaction = transaction;
						await command.ExecuteNonQueryAsync();
					}
					await database.InsertAsync(log);
					transaction.Commit();
				}

				connection.Close();
			}
			catch (Exception ex)
			{
				if (transaction != null) transaction.Rollback();
				if (connection != null) connection.Close();
				throw (ex);
			}

		}

		protected virtual IEnumerable<CommandType> OnGetCustomUpgradeCommands(int Revision)
		{
			yield break;
		}

       

        protected virtual async Task OnUpgradeAsync(ConnectionType Connection, TransactionType Transaction, int TargetRevision)
		{
			foreach (ITable table in database.Tables.Where(item=>item.Revision==TargetRevision))
			{
				await CreateTableAsync(Connection, Transaction, table);
			}
			foreach (IColumn column in database.Tables.SelectMany(item => item.Columns).Where(item => item.Revision == TargetRevision))
			{
				await CreateColumnAsync(Connection, Transaction, column);
			}
			foreach (IRelation relation in database.Relations.Where(item => item.Revision == TargetRevision))
			{
				await CreateRelationAsync(Connection, Transaction, relation);
			}

		}
	

	}
}
