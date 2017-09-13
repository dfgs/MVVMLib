
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
		

	
		

		
		

		

		

		

	


		

	}


}
