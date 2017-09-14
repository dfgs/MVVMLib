using DatabaseUpgraderLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseModelLib;
using MySql.Data.MySqlClient;
using MySqlDatabaseModelLib;
using System.Data.Common;

namespace MySqlDatabaseUpgraderLib
{
	public class MySqlDatabaseUpgrader : DatabaseUpgrader<MySqlConnection, MySqlCommand,MySqlTransaction>
	{
	


		public MySqlDatabaseUpgrader(MySqlDatabase Database, string DataSource, string AdditionalParameters = null) : base(Database)
		{
		}

		protected override MySqlConnection OnCreateConnection()
		{
			MySqlConnection connection;
			connection = new MySqlConnection(@"SERVER=" + ((MySqlDatabase)Database).Server + ";" + "UID=" + ((MySqlDatabase)Database).Login + ";" + "PASSWORD=" + ((MySqlDatabase)Database).Password);
			return connection;
		}

		private string GetTypeName(IColumn Column)
		{
			string result;

			switch (Column.DataType.Name)
			{
				case "Text":
					result = "longtext";
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
					result = "bigint";
					break;
				default: throw (new NotImplementedException("Cannot convert CLR type " + Column.DataType.Name));

			}
			return result;
		}

		protected override MySqlCommand OnCreateColumnCreateCommand(IColumn Column)
		{
			throw new NotImplementedException();
		}

		protected override MySqlCommand OnCreateRelationCreateCommand(IRelation Relation)
		{
			return new MySqlCommand("ALTER TABLE " + Relation.ForeignColumn.TableName + " ADD FOREIGN KEY (" + Relation.ForeignColumn.Name + ") REFERENCES " + Relation.PrimaryColumn.TableName + "(" + Relation.PrimaryColumn.Name + ")");
		}

		protected override MySqlCommand OnCreateTableCreateCommand(ITable Table)
		{
			string sql;

			sql = "CREATE TABLE " + Table.Name + " (";
			foreach (IColumn column in Table.Columns.Where(item => (item.Revision == 0) && !item.IsVirtual))
			{
				sql += column.Name + " " + GetTypeName(column) + (column.IsNullable ? " NULL" : " NOT NULL");
				if (column.IsIdentity) sql += " AUTO_INCREMENT,"; else sql += ",";
			}
			sql += "PRIMARY KEY(" + Table.PrimaryKey.Name + "))";

			return new MySqlCommand(sql);
		}

		protected override MySqlCommand OnCreateTableDropCommand(ITable Table)
		{
			string sql;

			sql = "drop table " + Table.Name + ";";

			return new MySqlCommand(sql);
		}



		protected override async Task<bool> OnDatabaseExistsAsync()
		{
			MySqlCommand command;
			MySqlConnection connection = null;
			DbDataReader reader;

			using (connection = OnCreateConnection())
			{
				await connection.OpenAsync();

				command = new MySqlCommand("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '" + Database.Name + "'");
				command.Connection = connection;

				reader = await command.ExecuteReaderAsync();
				return reader.HasRows;
			}
		}

		protected override Task<bool> OnSchemaExistsAsync()
		{
			throw new NotImplementedException();
		}

		protected override Task OnBackupAsync(string Path)
		{
			throw new NotImplementedException();
		}

		protected override async Task OnDropAsync()
		{
			MySqlCommand command;
			MySqlConnection connection = null;

			using (connection = OnCreateConnection())
			{
				await connection.OpenAsync();

				command = new MySqlCommand("drop database " + Database.Name);
				command.Connection = connection;

				await command.ExecuteNonQueryAsync();
			}
		}

		protected override async Task OnCreateDatabaseAsync()
		{
			MySqlCommand command;
			MySqlConnection connection = null;

			using (connection = OnCreateConnection())
			{
				command = new MySqlCommand("create database " + Database.Name);
				command.Connection = connection;
				await command.ExecuteNonQueryAsync();

				command = new MySqlCommand("GRANT ALL PRIVILEGES ON " + Database.Name + ".* TO '" + ((MySqlDatabase)Database).Login + "'@'%' WITH GRANT OPTION;");
				command.Connection = connection;
				await command.ExecuteNonQueryAsync();
			}
		}







	}
}
