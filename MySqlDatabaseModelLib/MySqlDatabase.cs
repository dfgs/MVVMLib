
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using DatabaseModelLib;

namespace MySqlDatabaseModelLib
{
	public abstract class MySqlDatabase:Database<MySqlConnection,MySqlCommand>
	{
		
		private string server;
		public string Server
		{
			get { return server; }
		}
		
		private string login;
		public string Login
		{
			get { return login; }
		}
		private string password;
		public string Password
		{
			get { return password; }
		}

		public MySqlDatabase(string Server,string Login,string Password)
		{
			this.server = Server;this.login = Login;this.password = Password;
		}

		
		protected override string OnFormatColumnName(IColumn Column)
		{
			return "`" + Column.Name + "`";
		}
		protected override string OnFormatTableName(string TableName)
		{
			return "`" + TableName + "`";
		}
		protected override MySqlConnection OnCreateConnection()
		{
			MySqlConnection connection;
			connection= new MySqlConnection(@"SERVER=" + server + ";" + "DATABASE=" + Name + ";" + "UID=" + Login + ";" + "PASSWORD=" + Password + ";Convert Zero Datetime = True;Allow Zero Datetime=True;");

			return connection;
		}

		protected override void OnSetParameter<DataType>(MySqlCommand Command, string Name, object Value)
		{
			Command.Parameters.AddWithValue(Name, Value);
		}

		protected override MySqlCommand OnCreateIdentityCommand<DataType>()
		{
			return new MySqlCommand("select @@identity");
		}
		

		protected override MySqlCommand OnCreateRelationCreateCommand(IRelation Relation)
		{
			return new MySqlCommand("ALTER TABLE " + Relation.ForeignColumn.TableName + " ADD FOREIGN KEY (" + Relation.ForeignColumn.Name + ") REFERENCES " + Relation.PrimaryColumn.TableName + "(" + Relation.PrimaryColumn.Name + ")");
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

		protected override MySqlCommand OnCreateTableDropCommand(ITable Table)
		{
			string sql;

			sql = "drop table " + Table.Name + ";";

			return new MySqlCommand(sql);
		}

		protected override MySqlCommand OnCreateTableCreateCommand(ITable Table,int Revision)
		{
			string sql;
	
			sql = "CREATE TABLE " + Table.Name + " (";
			foreach(IColumn column in Table.Columns.Where(item=>(item.Revision<=Revision) && !item.IsVirtual))
			{
				sql += column.Name +" "+ GetTypeName(column) + (column.IsNullable ? " NULL" : " NOT NULL");
				if (column.IsIdentity) sql += " AUTO_INCREMENT,"; else sql += ",";
			}
			sql += "PRIMARY KEY(" + Table.PrimaryKey.Name + "))";

			return new MySqlCommand(sql);

		}

		protected override async Task OnCreateAsync()
		{
			MySqlCommand command;
			MySqlConnection connection = null;

			try
			{
				connection = new MySqlConnection(@"SERVER=" + server + ";" + "UID=" + Login + ";" + "PASSWORD=" + Password);
				await connection.OpenAsync();

				command = new MySqlCommand("create database " + this.Name );
				command.Connection = connection;
				await command.ExecuteNonQueryAsync();

				command = new MySqlCommand("GRANT ALL PRIVILEGES ON "+Name+".* TO '"+Login+"'@'%' WITH GRANT OPTION;");
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

		protected override async Task OnDropAsync()
		{
			MySqlCommand command;
			MySqlConnection connection = null;

			try
			{

				connection = new MySqlConnection(@"SERVER=" + server + ";" + "UID=" + Login + ";" + "PASSWORD=" + Password);
				await connection.OpenAsync();

				command = new MySqlCommand("drop database " + this.Name );
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

		protected override Task OnBackupAsync(string Path)
		{
			throw new NotImplementedException();
		}


		protected override async Task<bool> OnExistsAsync()
		{
			MySqlCommand command;
			MySqlConnection connection = null;
			DbDataReader reader;

			try
			{

				connection = new MySqlConnection(@"SERVER=" + server + ";"  + "UID=" + Login + ";" + "PASSWORD=" + Password);
				await connection.OpenAsync();

				command = new MySqlCommand("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '"+ this.Name +"'");
				command.Connection = connection;

				reader = await command.ExecuteReaderAsync();
				return reader.HasRows;

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


		

	}


}
