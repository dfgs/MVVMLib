
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using DatabaseModelLib;
using ModelLib;

namespace SqlCEDatabaseModelLib
{
	public abstract class SqlCEDatabase:Database<SqlCeConnection ,SqlCeCommand>
	{
		private string fileName;
		public string FileName
		{
			get { return fileName; }
		}
		
		

		public SqlCEDatabase(string FileName)
		{
			this.fileName = FileName;
		}

		
		protected override SqlCeConnection OnCreateConnection()
		{
			//MyData.sdf
			return new SqlCeConnection(@"Data Source="+fileName+";Persist Security Info=False;");
		}


		protected override void OnSetParameter<DataType>(SqlCeCommand Command, string Name, object Value)
		{
			Command.Parameters.AddWithValue(Name, Value);
		}

		protected override SqlCeCommand OnCreateIdentityCommand<DataType>()
		{
			return new SqlCeCommand("select @@identity");
		}

		protected override object OnConvertToDbValue<DataType>(IColumn<DataType> Column, DataType Component)
		{
			object value;

			value = Column.GetValue(Component);
			if (value == null) return DBNull.Value;

			if (Column.DataType == typeof(Text)) return value.ToString();
			else if (Column.DataType == typeof(TimeSpan)) return ((TimeSpan)value).Ticks;

			return value;
		}
		protected override object OnConvertFromDbValue<DataType>(IColumn<DataType> Column, object Value)
		{
			if (Value == DBNull.Value) return null;

			if (Column.DataType == typeof(TimeSpan)) return TimeSpan.FromTicks((long)Value) ;
			return Value;
		}

		private string GetTypeName(IColumn Column)
		{
			string result;

			switch (Column.DataType.Name)
			{
				case "Text": result= "nvarchar(1024)";
					break;
				case "Int32": result="integer";
					break;
				case "Int64": result = "bigint";
					break;
				case "Boolean": result = "bit";
					break;
				case "DateTime":
					result = "DateTime";
					break;
				case "TimeSpan":
					result = "bigint";
					break;
				default: throw (new NotImplementedException("Cannot convert CLR type "+ Column.DataType.Name));

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
				if (Column.DefaultValue != null) sql += " DEFAULT('"+Column.DefaultValue.ToString()+"')";
			}
			
			command = new SqlCeCommand(sql);


			return command;
		}

		protected override SqlCeCommand OnCreateTableDropCommand(ITable Table)
		{
			string sql;

			sql = "drop table " + OnFormatTableName(Table.Name);

			return new SqlCeCommand(sql);
		}

		protected override SqlCeCommand OnCreateTableCreateCommand(ITable Table,int Revision)
		{
			string sql;

			//CREATE TABLE MyCustomers(CustID int IDENTITY(100, 1) PRIMARY KEY, CompanyName nvarchar(50))

			sql = "create table "+OnFormatTableName(Table.Name)+" (" ;
			foreach(IColumn column in Table.Columns.Where(item=> item.Revision<=Revision))
			{
				sql += OnFormatColumnName(column)+" " + GetTypeName(column) +  (column.IsNullable?" NULL,":" NOT NULL,");
			}
			sql = sql.TrimEnd(',') + ")";

			return new SqlCeCommand(sql);
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

		protected override async Task OnCreateAsync()
		{
			try
			{
				SqlCeEngine engine = new SqlCeEngine(@"Data Source = " + fileName);
				engine.CreateDatabase();
				await Task.Yield();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
			}
		}

		protected override async Task OnDropAsync()
		{

			try
			{
				System.IO.File.Delete(fileName);
				await Task.Yield();
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
			}
		}


		protected override async Task<bool> OnExistsAsync()
		{
			try
			{
				return await Task.FromResult(System.IO.File.Exists(fileName));
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				
			}
		}

		protected override async Task OnBackupAsync(string Path)
		{
			await Task.Run(() => System.IO.File.Copy(fileName, Path,true));
		}


		



	}


}
