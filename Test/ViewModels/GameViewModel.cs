using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace Test.ViewModels
{
	public class GameViewModel : ViewModel<SqlDatabase, Game>
	{
		public int GameID
		{
			get { return Model.GameID; }
		}

		public string Name
		{
			get { return Model?.Name; }
			set { Model.Name = value;OnPropertyChanged(); }
		}

	}
}
