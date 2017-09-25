using DatabaseModelLib;
using DatabaseModelLib.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModelLib;
using System;

namespace DatabaseViewModelLib
{
	public abstract class RowViewModelCollection<ViewModelType,ModelType>:ViewModelCollection<IEnumerable<ModelType>, ViewModelType,ModelType>
		where ViewModelType:RowViewModel<ModelType>
	{
		private IDatabaseViewModel database;
		public IDatabaseViewModel Database
		{
			get { return database; }
		}

		public RowViewModelCollection(IDatabaseViewModel Database)
		{
			this.database = Database;
		}

		protected override bool OnAddCommandCanExecute(object Parameter)
		{
			return IsLoaded;
		}

		protected override async Task<bool> OnAddInModelAsync(ViewModelType ViewModel)
		{
			return await Database.InsertAsync(ViewModel.Model);
		}
		protected override async Task<bool> OnRemoveFromModelAsync(ViewModelType ViewModel)
		{
			return await Database.DeleteAsync(ViewModel.Model);
		}
		protected override async Task<bool> OnEditInModelAsync(ViewModelType ViewModel)
		{
			return await Database.UpdateAsync(ViewModel.Model);
		}

		protected virtual Filter<ModelType> OnCreateFilter()
		{
			return null;
		}
		protected virtual IColumn<ModelType>[] OnCreateOrders()
		{
			return null;
		}

		protected override async Task<IEnumerable<ModelType>> OnLoadModelAsync()
		{
			return await Database.SelectAsync<ModelType>(() => { return OnCreateEmptyModelAsync().Result; },OnCreateFilter(),OnCreateOrders() );
		}


	}
}
