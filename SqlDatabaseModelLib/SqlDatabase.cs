
using DatabaseModelLib;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDatabaseModelLib
{
	public abstract class SqlDatabase:Database<SqlConnection,SqlCommand>
	{
		private string dataSource;
		public string DataSource
		{
			get { return dataSource; }
		}
		
		private string additionalParameters;
		public string AdditionalParameters
		{
			get { return additionalParameters; }
		}

		public SqlDatabase(string DataSource,string AdditionalParameters=null)
		{
			this.dataSource = DataSource;this.additionalParameters = AdditionalParameters;
		}

		
		protected override SqlConnection OnCreateConnection()
		{
			return new SqlConnection(@"Data Source="+dataSource+";Initial Catalog="+Name+";Integrated Security=True"+ ( additionalParameters==null?"":";"+additionalParameters ) );
		}


		protected override void OnSetParameter<DataType>(SqlCommand Command,string Name, object Value)
		{
			Command.Parameters.AddWithValue(Name, Value);
		}

		protected override SqlCommand OnCreateIdentityCommand<DataType>()
		{
			return new SqlCommand("select @@identity");
		}

		private string GetTypeName(IColumn Column)
		{
			string result;

			switch (Column.DataType.Name)
			{
				case "Text": result= "nvarchar(MAX)";
					break;
				case "Byte":
					result = "tinyint";
					break;
				case "Int32":
					result = "int";
					break;
				case "Int64": result = "bigint";
					break;
				case "Boolean": result = "bit";
					break;
				case "DateTime":result = "DateTime";
					break;
				case "TimeSpan":result = "Time(7)";
					break;
				default: throw (new NotImplementedException("Cannot convert CLR type "+ Column.DataType.Name));

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
		protected override SqlCommand OnCreateTableDropCommand(ITable Table)
		{
			string sql;

			sql = "drop table [" + Table.Name + "];";

			return new SqlCommand(sql);
		}
		protected override SqlCommand OnCreateTableCreateCommand(ITable Table,int Revision)
		{
			string sql;
			

			sql = "create table ["+Table.Name+"] (" ;
			foreach(IColumn column in Table.Columns.Where(item=>(item.Revision<=Revision) && !item.IsVirtual))
			{
				sql += OnFormatColumnName(column)+ " " + GetTypeName(column) +  (column.IsNullable?" NULL,":" NOT NULL,");
			}
			sql += "CONSTRAINT [PK_" + Table.Name + "]" + " PRIMARY KEY CLUSTERED ([" + Table.PrimaryKey.Name + "] ASC))";
			
			return new SqlCommand(sql);
		}

		protected override SqlCommand OnCreateRelationCreateCommand(IRelation Relation)
		{
			string sql;

			sql = "ALTER TABLE [" + Relation.ForeignColumn.TableName + "] WITH CHECK ADD CONSTRAINT [FK_"
				+ Relation.ForeignColumn.TableName + "_" + Relation.PrimaryColumn.TableName + "] FOREIGN KEY([" + Relation.ForeignColumn.Name + "]) "
				+ "REFERENCES [" + Relation.PrimaryColumn.TableName + "] ([" + Relation.PrimaryColumn.Name + "])";
			if (Relation.DeleteReferentialAction==DeleteReferentialAction.Delete) sql += " ON DELETE CASCADE";
			return new SqlCommand(sql);
		}

		protected override async Task OnCreateAsync()
		{
			SqlCommand command;
			SqlConnection connection = null;

			try
			{
				connection = new SqlConnection(@"Data Source=" + dataSource + ";Integrated Security=True" + (additionalParameters == null ? "" : ";" + additionalParameters));
				await connection.OpenAsync();

				command = new SqlCommand("create database [" + this.Name + "]");
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

		protected override async Task OnDropAsync()
		{
			SqlCommand command;
			SqlConnection connection = null;

			try
			{

				connection = new SqlConnection(@"Data Source=" + dataSource + ";Integrated Security=True" + (additionalParameters == null ? "" : ";" + additionalParameters));
				await connection.OpenAsync();

				command = new SqlCommand("drop database [" + this.Name + "]");
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


		protected override async Task<bool> OnExistsAsync()
		{
			SqlCommand command;
			SqlConnection connection = null;
			SqlDataReader reader;

			try
			{

				connection = new SqlConnection(@"Data Source=" + dataSource + ";Integrated Security=True" + (additionalParameters == null ? "" : ";" + additionalParameters));
				await connection.OpenAsync();

				command = new SqlCommand("select name from sys.databases where name = '"+this.Name+"'");
				command.Connection = connection;

				reader=await command.ExecuteReaderAsync();
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
