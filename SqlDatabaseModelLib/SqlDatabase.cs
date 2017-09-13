
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

				
		

		

		
		

		


		






	}


}
