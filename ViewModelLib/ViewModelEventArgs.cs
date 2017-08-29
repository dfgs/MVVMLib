using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib
{
	public delegate void ViewModelEventHandler<ViewModelType>(object sender, ViewModelEventArgs<ViewModelType> e)
		where ViewModelType : IViewModel;

	public class ViewModelEventArgs<ViewModelType>
		where ViewModelType:IViewModel
	{
		private ViewModelType viewModel;
		public ViewModelType ViewModel
		{
			get { return viewModel; }
		}

		public ViewModelEventArgs(ViewModelType ViewModel)
		{
			this.viewModel = ViewModel;
		}

	}


}
