using DatabaseModelLib;
using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseUpgraderLib
{
	
	public class DatabaseUpgrader
	{
		private IDatabase database;
		public IDatabase Database
		{
			get { return database; }
		}
		private Table<UpgradeLog> upgradeLogs;

		private string errorMessage;
		public string ErrorMessage
		{
			get { return errorMessage; }
		}

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

		public DatabaseUpgrader(IDatabase Database)
		{
			this.database = Database;
			upgradeLogs = new Table<UpgradeLog>();
		}
		public async Task<int> GetDatabaseRevisionAsync()
		{
			IEnumerable<UpgradeLog> logs;

			errorMessage = null;
			try
			{
				logs = await database.SelectAsync<UpgradeLog>();
				return logs.Max(item => item.Revision)??0;
			}
			catch(Exception ex)
			{
				errorMessage = ex.Message;
				return -1;
			}
		}

		public int GetTargetRevision()
		{
			return RevisionItems.Max(item => item.Revision);
		}

		public async Task<bool> NeedsUpgradeAsync()
		{
			return GetTargetRevision() != await GetDatabaseRevisionAsync();
		}

		public async Task<bool> UpgradeAsync()
		{
			int targetRevision;
			int currentRevision;
			bool success;
			UpgradeLog log;

			currentRevision = await GetDatabaseRevisionAsync();
			targetRevision = GetTargetRevision();

			errorMessage = null;
			if (currentRevision == -1) // no under version control
            {
                success = await OnCreateVersionControlAsync(targetRevision);
                if (success)
                {
                    log = new UpgradeLog() { Date = DateTime.Now, Revision = 0 };
                    try
                    {
                        await database.InsertAsync(log);
						currentRevision = 0;
                    }
                    catch(Exception ex)
                    {
						errorMessage = ex.Message;
						return false;
                    }
                }
            }
            
            success = true;
            while (currentRevision != targetRevision)
            {
                currentRevision++;
				database.SetMaxRevision(currentRevision);
				success = await OnUpgradeAsync(currentRevision);
                if (!success) break;

                log = new UpgradeLog() { Date = DateTime.Now, Revision = currentRevision };
                try
                {
					await OnUpgradedTo(currentRevision);
                    await database.InsertAsync(log);
                }
                catch (Exception ex)
                {
					errorMessage = ex.Message;
					success = false;
                    break;
                }
            }

			return success;
		}
		protected virtual async Task OnUpgradedTo(int Revision)
		{
			await Task.Yield();
		}

        protected virtual async Task<bool> OnCreateVersionControlAsync(int TargetRevision)
        {
			errorMessage = null;
			try
            {
                await database.CreateTableAsync(upgradeLogs);
            }
            catch (Exception ex)
            {
				errorMessage = ex.Message;
				return false;
            }
            return true;
        }

        protected virtual async Task<bool> OnUpgradeAsync(int TargetRevision)
		{

			errorMessage = null;
			foreach (ITable table in database.Tables.Where(item=>item.Revision==TargetRevision))
			{
				try
				{
					await database.CreateTableAsync(table);
				}
				catch (Exception ex)
                {
					errorMessage = ex.Message;
					return false;
				}
			}
			foreach (IColumn column in database.Tables.SelectMany(item => item.Columns).Where(item => item.Revision == TargetRevision))
			{
				try
				{
					await database.CreateColumnAsync(column);
				}
				catch (Exception ex)
                {
					errorMessage = ex.Message;
					return false;
				}
			}
			foreach (IRelation relation in database.Relations.Where(item => item.Revision == TargetRevision))
			{
				try
				{
					await database.CreateRelationAsync(relation);
				}
				catch (Exception ex)
                {
					errorMessage = ex.Message;
					return false;
				}
			}

			return true;
		}
		

	}
}
