using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace DatabaseViewModelLib
{
	public abstract class RowViewModel<ModelType>:ViewModel<ModelType>
	{
		private IDatabaseViewModel database;
		public IDatabaseViewModel Database
		{
			get { return database; }
		}


		public RowViewModel(IDatabaseViewModel Database)
		{
			this.database = Database;
		}

	}
}
