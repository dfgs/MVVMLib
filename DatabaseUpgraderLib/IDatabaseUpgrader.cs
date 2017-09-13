using DatabaseModelLib;
using DatabaseModelLib.Filters;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace DatabaseUpgraderLib
{
	public interface IDatabaseUpgrader
	{

		/*Task CreateTableAsync(ITable Table);
		Task DropTableAsync(ITable Table);
		Task CreateRelationAsync(IRelation Relation);
		Task CreateColumnAsync(IColumn Column);*/


		Task<bool> ExistsAsync();
		Task CreateAsync();
		Task BackupAsync(string Path);

		void SetMaxRevision(int Revision);
	}

	
}
