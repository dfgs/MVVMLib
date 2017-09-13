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
	
	public abstract class DatabaseUpgrader<ConnectionType, CommandType>
		where ConnectionType : DbConnection
		where CommandType : DbCommand, new()
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


		private async Task CreateColumnAsync(ConnectionType Connection, IColumn Column)
		{
			CommandType command;

			command = OnCreateColumnCreateCommand(Column);
			command.Connection = Connection;
			await command.ExecuteNonQueryAsync();
		}

		private async Task CreateTableAsync(ConnectionType Connection, ITable Table)
		{
			CommandType command;

			command = OnCreateTableCreateCommand(Table);
			command.Connection = Connection;
			await command.ExecuteNonQueryAsync();
		}

		private async Task DropTableAsync(ConnectionType Connection, ITable Table)
		{
			CommandType command;

			command = OnCreateTableDropCommand(Table);
			command.Connection = Connection;
			await command.ExecuteNonQueryAsync();
		}

		private async Task CreateRelationAsync(ConnectionType Connection, IRelation Relation)
		{
			CommandType command;

			command = OnCreateRelationCreateCommand(Relation);
			command.Connection = Connection;
			await command.ExecuteNonQueryAsync();
		}



		protected abstract Task<bool> OnExistsAsync();
		public async Task<bool> ExistsAsync()
		{
			return await OnExistsAsync();
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

		protected abstract Task OnCreatingAsync(ConnectionType Connection);

		public async Task CreateAsync()
		{
			ConnectionType connection = null;
			DbTransaction transaction=null;

			try
			{
				connection = OnCreateConnection();
				await connection.OpenAsync();
				transaction=connection.BeginTransaction();

				await OnCreatingAsync(connection);

				await CreateTableAsync(connection,upgradeLogs);

				foreach (ITable table in Database.Tables.Where(item => item.Revision == 0))
				{
					await CreateTableAsync(connection, table);
				}

				foreach (IRelation relation in Database.Relations.Where(item => item.Revision == 0))
				{
					await CreateRelationAsync(connection, relation);
				}

				await OnCreatedAsync();

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

		protected virtual async Task OnCreatedAsync()
		{
			await Task.Yield();
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
			DbTransaction transaction = null;

			currentRevision = await GetDatabaseRevisionAsync();
			targetRevision = GetTargetRevision();

			try
			{
				connection = OnCreateConnection();
				await connection.OpenAsync();
				transaction = connection.BeginTransaction();

				while (currentRevision != targetRevision)
				{
					currentRevision++;
					//database.SetMaxRevision(currentRevision);
					await OnUpgradeAsync(connection,currentRevision);
					log = new UpgradeLog() { Date = DateTime.Now, Revision = currentRevision };

					await OnUpgradedTo(currentRevision);
					await database.InsertAsync(log);
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

		}

		protected virtual async Task OnUpgradedTo(int Revision)
		{
			await Task.Yield();
		}

       

        protected virtual async Task OnUpgradeAsync(ConnectionType Connection,int TargetRevision)
		{
			foreach (ITable table in database.Tables.Where(item=>item.Revision==TargetRevision))
			{
				await CreateTableAsync(Connection, table);
			}
			foreach (IColumn column in database.Tables.SelectMany(item => item.Columns).Where(item => item.Revision == TargetRevision))
			{
				await CreateColumnAsync(Connection, column);
			}
			foreach (IRelation relation in database.Relations.Where(item => item.Revision == TargetRevision))
			{
				await CreateRelationAsync(Connection, relation);
			}

		}
	

	}
}
