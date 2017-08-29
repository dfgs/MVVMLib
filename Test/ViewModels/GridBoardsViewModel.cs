using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace Test.ViewModels
{
	public class GridBoardsViewModel : ChildViewModelCollection<SqlDatabase,PartyViewModel,Party, GridBoardViewModel, GridBoard>
	{

		
		protected override async Task<IEnumerable<GridBoard>> OnRefreshAsync()
		{
			if (Parent?.SelectedItem == null) return await Task.FromResult<IEnumerable<GridBoard>>(null);

			return await Task.FromResult(Client.Select<GridBoard>(new Filter<GridBoard>(Schema<GridBoard>.Instance.Table["PartyID"], Parent.SelectedItem.PartyID) )  );
		}

	}
}
