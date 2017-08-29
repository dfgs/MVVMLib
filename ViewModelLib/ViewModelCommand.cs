using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModelLib
{
	public class ViewModelCommand :ICommand
	{

		public event EventHandler CanExecuteChanged;

		public event EventHandler Executed;

		private Func<object, bool> canExecute;
		private Action<object> execute;

		public ViewModelCommand( Func<object,bool> CanExecute,Action<object> Execute )
		{
			this.execute = Execute;this.canExecute = CanExecute;
			CommandManager.RequerySuggested += CommandManager_RequerySuggested;
		}

		private void CommandManager_RequerySuggested(object sender, EventArgs e)
		{
			OnCanExecuteChanged();
		}


		private void Command_Executed(object sender, EventArgs e)
		{
			OnCanExecuteChanged();
		}//*/
		

		protected virtual void OnExecuted()
		{
			Executed?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		public bool CanExecute(object parameter)
		{
			if (canExecute != null) return canExecute(parameter);
			else return true;
		}

		public void Execute(object parameter)
		{
			execute?.Invoke(parameter);
			OnExecuted();
			OnCanExecuteChanged();
		}

	}
}
