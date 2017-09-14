using DatabaseUpgraderLib;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseModelLib;
using SqlCEDatabaseModelLib;
using System.Data.SqlServerCe;
using ModelLib;

namespace SqlCEDatabaseUpgraderLib
{
	public class SqlCEDatabaseUpgrader : DatabaseUpgrader<SqlCeConnection, SqlCeCommand,SqlCeTransaction>
	{

		public SqlCEDatabaseUpgrader(SqlCEDatabase Database) : base(Database)
		{
		}

		protected override SqlCeConnection OnCreateConnection()
		{
			return new SqlCeConnection(@"Data Source=" + ((SqlCEDatabase)Database).FileName + ";Persist Security Info=False;");
		}

		private string GetTypeName(IColumn Column)
		{
			string result;

			switch (Column.DataType.Name)
			{
				case "Text":
					result = "nvarchar(1024)";
					break;
				case "Int32":
					result = "integer";
					break;
				case "Int64":
					result = "bigint";
					break;
				case "Boolean":
					result = "bit";
					break;
				case "DateTime":
					result = "DateTime";
					break;
				case "TimeSpan":
					result = "bigint";
					break;
				default: throw (new NotImplementedException("Cannot convert CLR type " + Column.DataType.Name));

			}
			if (Column.IsIdentity) result += " IDENTITY(1, 1)";
			if (Column.IsPrimaryKey) result += " PRIMARY KEY";
			return result;
		}

		protected override SqlCeCommand OnCreateColumnCreateCommand(IColumn Column)
		{
			string sql;
			SqlCeCommand command;

			sql = "alter table " + OnFormatTableName(Column.TableName) + " ADD COLUMN "
				+ OnFormatColumnName(Column) + " " + GetTypeName(Column) + (Column.IsNullable ? " NULL" : " NOT NULL"); ;
			if (Column.DataType != typeof(Text) && Column.DataType != typeof(DateTime))
			{
				if (Column.DefaultValue != null)
				{
					sql += " DEFAULT(" + Column.DefaultValue.ToString() + ")";
				}
			}
			else
			{
				if (Column.DefaultValue != null) sql += " DEFAULT('" + Column.DefaultValue.ToString() + "')";
			}

			command = new SqlCeCommand(sql);


			return command;
		}

		protected override SqlCeCommand OnCreateRelationCreateCommand(IRelation Relation)
		{
			//ALTER TABLE Orders ADD CONSTRAINT FK_Customer_Order FOREIGN KEY (CustomerId)REFERENCES Customers(CustomerId)
			string sql;
			sql = "ALTER TABLE [" + Relation.ForeignColumn.TableName + "] ADD CONSTRAINT [FK_"
				+ Relation.ForeignColumn.TableName + "_" + Relation.PrimaryColumn.TableName + "] FOREIGN KEY([" + Relation.ForeignColumn.Name + "]) "
				+ "REFERENCES [" + Relation.PrimaryColumn.TableName + "] ([" + Relation.PrimaryColumn.Name + "])";
			if (Relation.DeleteReferentialAction == DeleteReferentialAction.Delete) sql += " ON DELETE CASCADE";
			return new SqlCeCommand(sql);
		}

		protected override SqlCeCommand OnCreateTableCreateCommand(ITable Table)
		{
			string sql;

			//CREATE TABLE MyCustomers(CustID int IDENTITY(100, 1) PRIMARY KEY, CompanyName nvarchar(50))

			sql = "create table " + OnFormatTableName(Table.Name) + " (";
			foreach (IColumn column in Table.Columns.Where(item => (item.Revision ==0) && !item.IsVirtual))
			{
				sql += OnFormatColumnName(column) + " " + GetTypeName(column) + (column.IsNullable ? " NULL," : " NOT NULL,");
			}
			sql = sql.TrimEnd(',') + ")";

			return new SqlCeCommand(sql);
		}

		protected override SqlCeCommand OnCreateTableDropCommand(ITable Table)
		{
			string sql;

			sql = "drop table " + OnFormatTableName(Table.Name);

			return new SqlCeCommand(sql);
		}



		protected override async Task<bool> OnDatabaseExistsAsync()
		{
			return await Task.FromResult(System.IO.File.Exists(((SqlCEDatabase)Database).FileName));
		}

		protected override Task<bool> OnSchemaExistsAsync()
		{
			throw new NotImplementedException();
		}

		protected override async Task OnBackupAsync(string Path)
		{
			await Task.Run(() => System.IO.File.Copy(((SqlCEDatabase)Database).FileName, Path, true));
		}

		protected override async Task OnDropAsync()
		{
			System.IO.File.Delete(((SqlCEDatabase)Database).FileName);
			await Task.Yield();
		}

		protected override async Task OnCreateDatabaseAsync()
		{
			SqlCeEngine engine = new SqlCeEngine(@"Data Source = " + ((SqlCEDatabase)Database).FileName);
			engine.CreateDatabase();
			await Task.Yield();
		}







	}
}
