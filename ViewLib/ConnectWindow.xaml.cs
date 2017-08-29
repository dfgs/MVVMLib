using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ViewLib
{
	/// <summary>
	/// Interaction logic for ConnectWindow.xaml
	/// </summary>
	public partial class ConnectWindow : Window
	{
		private ConnectionInfo connectionInfo;
		public ConnectionInfo ConnectionInfo
		{
			get { return connectionInfo; }
		}

		public ConnectWindow():this(new ConnectionInfo())
		{
			
		}
		public ConnectWindow(ConnectionInfo ConnectionInfo)
		{
			this.connectionInfo = ConnectionInfo;
			InitializeComponent();
			DataContext = ConnectionInfo;
		}

		private void OKCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !(string.IsNullOrWhiteSpace(connectionInfo.Login) || string.IsNullOrWhiteSpace(connectionInfo.Server) );
			e.Handled = true;
		}
		private void OKCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			connectionInfo.Password = passwordBox.Password;
			DialogResult = true;
		}
		private void CancelCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true; e.Handled = true;
		}
		private void CancelCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			passwordBox.Focus();
		}
	}
}
