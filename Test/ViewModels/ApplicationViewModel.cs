using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewModelLib;

namespace Test.ViewModels
{
	public class ApplicationViewModel:ViewModel<SqlDatabase,object>
	{
		public static readonly DependencyProperty GamesProperty = DependencyProperty.Register("Games", typeof(GamesViewModel), typeof(ApplicationViewModel));
		public GamesViewModel Games
		{
			get { return (GamesViewModel)GetValue(GamesProperty); }
			private set { SetValue(GamesProperty, value); }
		}

		public static readonly DependencyProperty PartiesProperty = DependencyProperty.Register("Parties", typeof(PartiesViewModel), typeof(ApplicationViewModel));
		public PartiesViewModel Parties
		{
			get { return (PartiesViewModel)GetValue(PartiesProperty); }
			private set { SetValue(PartiesProperty, value); }
		}

		public static readonly DependencyProperty GridBoardsProperty = DependencyProperty.Register("GridBoards", typeof(GridBoardsViewModel), typeof(ApplicationViewModel));
		public GridBoardsViewModel GridBoards
		{
			get { return (GridBoardsViewModel)GetValue(GridBoardsProperty); }
			private set { SetValue(GridBoardsProperty, value); }
		}


		public static readonly DependencyProperty ConnectCommandProperty = DependencyProperty.Register("ConnectCommand", typeof(ViewModelCommand), typeof(ApplicationViewModel));
		public ViewModelCommand ConnectCommand
		{
			get { return (ViewModelCommand)GetValue(ConnectCommandProperty); }
			private set { SetValue(ConnectCommandProperty, value); }
		}
		public static readonly DependencyProperty DisconnectCommandProperty = DependencyProperty.Register("DisconnectCommand", typeof(ViewModelCommand), typeof(ApplicationViewModel));
		public ViewModelCommand DisconnectCommand
		{
			get { return (ViewModelCommand)GetValue(DisconnectCommandProperty); }
			private set { SetValue(DisconnectCommandProperty, value); }
		}

		public static readonly DependencyProperty IsConnectedProperty = DependencyProperty.Register("IsConnected", typeof(bool), typeof(ApplicationViewModel));
		public bool IsConnected
		{
			get { return (bool)GetValue(IsConnectedProperty); }
			private set { SetValue(IsConnectedProperty, value); }
		}

		public ApplicationViewModel()
		{
			this.Client = new SqlDatabase("127.0.0.1", "BoardGameEngine");

			ConnectCommand = new ViewModelCommand(ConnectCommandCanExecute, ConnectCommandExecute);
			DisconnectCommand = new ViewModelCommand(DisconnectCommandCanExecute, DisconnectCommandExecute);

			Games = new GamesViewModel() { Client = this.Client };
			Parties = new PartiesViewModel() { Client = this.Client ,Parent=Games };
			GridBoards = new GridBoardsViewModel() { Client = this.Client, Parent = Parties };

		}

		private bool ConnectCommandCanExecute(object Parameter)
		{
			return !IsConnected;
		}
		private async void ConnectCommandExecute(object Parameter)
		{
			IsConnected = true;
			await OnRefreshAsync();
		}
		private bool DisconnectCommandCanExecute(object Parameter)
		{
			return IsConnected;
		}
		private async void DisconnectCommandExecute(object Parameter)
		{
			IsConnected = false;
			await OnClearAsync();
		}


		protected override async Task<object> OnRefreshAsync()
		{
			await Games.RefreshAsync();
			await Parties.RefreshAsync();
			await GridBoards.RefreshAsync();
			return null;
		}

		protected override async Task OnClearAsync()
		{
			await Games.ClearAsync();
			await Parties.ClearAsync();
			await GridBoards.ClearAsync();
		}

	}
}
