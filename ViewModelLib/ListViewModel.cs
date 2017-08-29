using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib
{
	public abstract class ListViewModel<ViewModelType, ModelType> : ViewModelCollection<IList<ModelType>, ViewModelType, ModelType>
		where ViewModelType : ViewModel<ModelType>
	{

		public ListViewModel()
		{
		}

		protected override async Task<bool> OnAddInModelAsync(ViewModelType ViewModel)
		{
			Model.Add(ViewModel.Model);
			return await Task.FromResult<bool>(true);
		}
		protected override async Task<bool> OnRemoveFromModelAsync(ViewModelType ViewModel)
		{
			Model.Remove(ViewModel.Model);
			return await Task.FromResult<bool>(true);
		}
		protected override async Task<bool> OnEditInModelAsync(ViewModelType ViewModel)
		{
			return await Task.FromResult<bool>(true);
		}


		

		


	}
}
