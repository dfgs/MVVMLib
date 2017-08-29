using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace Test.ViewModels
{
	public class GamesViewModel:ViewModelCollection<SqlDatabase,GameViewModel, Game>
	{

		protected override async Task<IEnumerable<Game>> OnRefreshAsync()
		{
			return await Task.FromResult(Client.Select<Game>());
		}

	}
}
