using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace Test.ViewModels
{
	public class PartyViewModel : ViewModel<SqlDatabase, Party>
	{
		public int PartyID
		{
			get { return Model.PartyID; }
		}

		public string Description
		{
			get { return Model?.Description; }
			set { Model.Description = value;OnPropertyChanged(); }
		}

	}
}
