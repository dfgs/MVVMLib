using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace Test.ViewModels
{
	public class PartiesViewModel:ChildViewModelCollection<SqlDatabase,GameViewModel,Game, PartyViewModel, Party>
	{

		
		protected override async Task<IEnumerable<Party>> OnRefreshAsync()
		{
			if (Parent?.SelectedItem == null) return await Task.FromResult<IEnumerable<Party>>(null);

			return await Task.FromResult( Client.Select<Party>( new Filter<Party>( Schema<Party>.Instance.Table["GameID"], Parent.SelectedItem.GameID) )  );
		}

	}
}
