using DatabaseModelLib;
using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseUpgraderLib
{
	public class UpgradeLog
	{

		public static readonly Column<UpgradeLog, int> UpgradeLogIDColumn = new Column<UpgradeLog, int>() { IsPrimaryKey = true, IsIdentity = true };
		public int? UpgradeLogID
		{
			get { return UpgradeLogIDColumn.GetValue(this); }
			set { UpgradeLogIDColumn.SetValue(this, value); }
		}


		public static readonly Column<UpgradeLog, DateTime> DateColumn = new Column<UpgradeLog, DateTime>();
		public DateTime? Date
		{
			get { return DateColumn.GetValue(this); }
			set { DateColumn.SetValue(this, value); }
		}


		public static readonly Column<UpgradeLog, int> RevisionColumn = new Column<UpgradeLog, int>();
		public int? Revision
		{
			get { return RevisionColumn.GetValue(this); }
			set { RevisionColumn.SetValue(this, value); }
		}


	}
}
