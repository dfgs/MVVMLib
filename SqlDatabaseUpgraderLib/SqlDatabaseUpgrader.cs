using DatabaseUpgraderLib;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseModelLib;
using SqlDatabaseModelLib;

namespace SqlDatabaseUpgraderLib
{
	public class SqlDatabaseUpgrader : DatabaseUpgrader<SqlConnection, SqlCommand,SqlTransaction>
	{
		
		public SqlDatabaseUpgrader(SqlDatabase Database) : base(Database)
		{
		}

		protected override SqlConnection OnCreateConnection()
		{
			SqlConnection connection;
			connection = new SqlConnection(@"Data Source=" + ((SqlDatabase)Database).DataSource + ";Integrated Security=True;");
			if (((SqlDatabase)Database).AdditionalParameters != null) connection.ConnectionString += ((SqlDatabase)Database).AdditionalParameters;
			return connection;
		}

		private string GetTypeName(IColumn Column)
		{
			string result;

			if (Column.DataType.IsEnum) return "int";

			switch (Column.DataType.Name)
			{
				case "Text":
					result = "nvarchar(MAX)";
					break;
				case "Byte":
					result = "tinyint";
					break;
				case "Int32":
					result = "int";
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
					result = "Time(7)";
					break;
				case "Byte[]":
					result = "varbinary(max)";
					break;
				default:
					throw (new NotImplementedException("Cannot convert CLR type " + Column.DataType.Name));

			}
			if (Column.IsIdentity) result += " IDENTITY(1, 1)";
			return result;
		}


		protected override SqlCommand OnCreateColumnCreateCommand(IColumn Column)
		{
			string sql;
			SqlCommand command;

			sql = "alter table " + OnFormatTableName(Column.TableName) + " ADD  "
				+ OnFormatColumnName(Column) + " " + GetTypeName(Column) + (Column.IsNullable ? " NULL" : " NOT NULL"); ;
			if (Column.DefaultValue != null) sql += " DEFAULT('" + Column.DefaultValue.ToString() + "')";

			command = new SqlCommand(sql);

			return command;
		}

		protected override SqlCommand OnCreateRelationCreateCommand(IRelation Relation)
		{
			string sql;

			sql = "ALTER TABLE [" + Relation.ForeignColumn.TableName + "] WITH CHECK ADD CONSTRAINT [FK_"
				+ Relation.ForeignColumn.TableName + "_" + Relation.PrimaryColumn.TableName + "] FOREIGN KEY([" + Relation.ForeignColumn.Name + "]) "
				+ "REFERENCES [" + Relation.PrimaryColumn.TableName + "] ([" + Relation.PrimaryColumn.Name + "])";
			if (Relation.DeleteReferentialAction == DeleteReferentialAction.Delete) sql += " ON DELETE CASCADE";
			return new SqlCommand(sql);
		}

		protected override SqlCommand OnCreateTableCreateCommand(ITable Table)
		{
			string sql;


			sql = "create table [" + Table.Name + "] (";
			foreach (IColumn column in Table.Columns.Where(item => (item.Revision == 0 ) && !item.IsVirtual))
			{
				sql += OnFormatColumnName(column) + " " + GetTypeName(column) + (column.IsNullable ? " NULL," : " NOT NULL,");
			}
			sql += "CONSTRAINT [PK_" + Table.Name + "]" + " PRIMARY KEY CLUSTERED ([" + Table.PrimaryKey.Name + "] ASC))";

			return new SqlCommand(sql);
		}

		protected override SqlCommand OnCreateTableDropCommand(ITable Table)
		{
			string sql;

			sql = "drop table [" + Table.Name + "];";

			return new SqlCommand(sql);
		}



		protected override async Task<bool> OnDatabaseExistsAsync()
		{
			SqlCommand command;
			SqlConnection connection = null;
			SqlDataReader reader;

			using (connection = OnCreateConnection())
			{
				await connection.OpenAsync();

				command = new SqlCommand("select name from sys.databases where name = '" + Database.Name + "'");
				command.Connection = connection;

				reader = await command.ExecuteReaderAsync();
				return reader.HasRows;
			}
		}

		protected override async Task<bool> OnSchemaExistsAsync()
		{
			SqlCommand command;
			SqlConnection connection = null;
			SqlDataReader reader;

			using (connection = OnCreateConnection())
			{
				await connection.OpenAsync();
				connection.ChangeDatabase(Database.Name);

				command = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UpgradeLog'");
				command.Connection = connection;

				reader = await command.ExecuteReaderAsync();
				return reader.HasRows;
			}
			
		}

		protected override Task OnBackupAsync(string Path)
		{
			throw new NotImplementedException();
		}

		protected override async Task OnDropAsync()
		{
			SqlCommand command;
			SqlConnection connection = null;

			using (connection = OnCreateConnection())
			{
				await connection.OpenAsync();

				command = new SqlCommand("drop database [" + Database.Name + "]");
				command.Connection = connection;

				await command.ExecuteNonQueryAsync();
			}
		}

		protected override async Task OnCreateDatabaseAsync()
		{
			SqlCommand command;
			SqlConnection connection;

			using (connection = OnCreateConnection())
			{
				await connection.OpenAsync();

				command = new SqlCommand("create database [" + Database.Name + "]");
				command.Connection = connection;

				await command.ExecuteNonQueryAsync();
			}
		}







	}
}
