using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewLib;

namespace ViewModelLib
{
	public abstract class ViewModelCollection<CollectionModelType,ItemViewModelType,ItemModelType> : ViewModel<CollectionModelType>, IViewModelCollection<ItemViewModelType>,IEqualityComparer<ItemModelType>
		where CollectionModelType:IEnumerable<ItemModelType>
		where ItemViewModelType : ViewModel<ItemModelType>

	{
		private List<ItemViewModelType> items;

		public ItemViewModelType this[int Index]
		{
			get
			{
				return items?[Index]; 
			}
		}

		public virtual bool UseDiff
		{
			get { return true; }
		}
		public int Count
		{
			get
			{
				return items?.Count??0;
			}
		}

        protected virtual bool EditItemOnAdd
        {
            get { return true; }
        }

		protected virtual bool SelectItemOnAdd
		{
			get { return true; }
		}

		public static readonly DependencyProperty AddCommandProperty = DependencyProperty.Register("AddCommand", typeof(ViewModelCommand), typeof(ViewModelCollection<CollectionModelType,ItemViewModelType,ItemModelType>));
		public ViewModelCommand AddCommand
		{
			get { return (ViewModelCommand)GetValue(AddCommandProperty); }
			private set { SetValue(AddCommandProperty, value); }
		}

		public static readonly DependencyProperty RemoveCommandProperty = DependencyProperty.Register("RemoveCommand", typeof(ViewModelCommand), typeof(ViewModelCollection<CollectionModelType, ItemViewModelType, ItemModelType>));
		public ViewModelCommand RemoveCommand
		{
			get { return (ViewModelCommand)GetValue(RemoveCommandProperty); }
			private set { SetValue(RemoveCommandProperty, value); }
		}

		public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register("EditCommand", typeof(ViewModelCommand), typeof(ViewModelCollection<CollectionModelType, ItemViewModelType, ItemModelType>));
		public ViewModelCommand EditCommand
		{
			get { return (ViewModelCommand)GetValue(EditCommandProperty); }
			private set { SetValue(EditCommandProperty, value); }
		}

		public IEnumerable<ItemViewModelType> SelectedItems
		{
			get { return this.Where(item => item.IsSelected); }
		}

		public ItemViewModelType SelectedItem
		{
			get { return SelectedItems.FirstOrDefault(); }
			set
			{
				foreach (ItemViewModelType viewModel in this) viewModel.IsSelected = false;
				if (value != null) value.IsSelected = true;
				OnPropertyChanged();
			}
		}

		//private ItemViewModelType savedSelectedItem;


		public event NotifyCollectionChangedEventHandler CollectionChanged;
		

		public ViewModelCollection()
		{
			items = new List<ItemViewModelType>();
			AddCommand = new ViewModelCommand(OnAddCommandCanExecute, AddCommandExecute);
			RemoveCommand = new ViewModelCommand(OnRemoveCommandCanExecute, RemoveCommandExecute);
			EditCommand = new ViewModelCommand(OnEditCommandCanExecute, EditCommandExecute);
			
		}
	
		

		
		/*protected override async Task<bool> OnLoadingAsync()
		{
			savedSelectedItem = SelectedItem;
			items.Clear();
			return await base.OnLoadingAsync();
		}*/

		protected override async Task OnLoadedAsync()
		{
			ItemViewModelType item;
			IEnumerable<NetDiff.DiffResult<ItemModelType>> diffs;
			int index;

			if (Model == null)
			{
				items.Clear();
				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				OnPropertyChanged("Count");
				SelectedItem = null;
				return;
			}


			if (UseDiff)
			{
				index = 0;
				/*if (GetType().Name.Contains("EmployeeViewViewModel"))
				{
					int t = 0;
				}//*/
				var option = new NetDiff.DiffOption<ItemModelType>();
				option.EqualityComparer = this;

				diffs = NetDiff.DiffUtil.Diff(items.Select(i => i.Model), Model,option);

				foreach (NetDiff.DiffResult<ItemModelType> diff in diffs)
				{
					switch(diff.Status)
					{
						case NetDiff.DiffStatus.Equal:
							item = items[index];
							await item.LoadAsync(diff.Obj1);
							index++;
							break;
						case NetDiff.DiffStatus.Deleted:
							item = items[index];
							items.RemoveAt(index);
							CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
							
							break;
						case NetDiff.DiffStatus.Inserted:
							item = await OnCreateViewModelItem(diff.Obj2.GetType());
							await item.LoadAsync(diff.Obj2);
							items.Insert(index, item);
							CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,item,index));
							index++;
							break;
						default:
							throw (new NotImplementedException());
					}
				}
				OnPropertyChanged("Count");
			}
			else
			{
				items.Clear();
				foreach (ItemModelType model in Model)
				{
					item = await OnCreateViewModelItem(model.GetType());
					await item.LoadAsync(model);
					items.Add(item);
				}
				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				OnPropertyChanged("Count");
			}

			
			SelectedItem = items.FirstOrDefault(i => i.IsSelected);
			if (SelectedItem == null) SelectedItem = OnGetDefaultSelectedItem();

			await base.OnLoadedAsync();
		}

		protected virtual ItemViewModelType OnGetDefaultSelectedItem()
		{
			return this.FirstOrDefault();
		}

		protected abstract Task<ItemModelType> OnCreateEmptyModelAsync();
		protected abstract Task<ItemViewModelType> OnCreateViewModelItem(Type ModelType); // Need parameter ModelType in case of abstract view model an heterogeneous items

		protected abstract Task<bool> OnAddInModelAsync(ItemViewModelType ViewModel);
		protected abstract Task<bool> OnRemoveFromModelAsync(ItemViewModelType ViewModel);
		protected abstract Task<bool> OnEditInModelAsync(ItemViewModelType ViewModel);

		public int IndexOf(ItemViewModelType Item)
		{
			return items.IndexOf(Item);
		}

		


		protected virtual Window OnCreateEditWindow()
        {
            return new EditWindow();
        }

		protected virtual bool OnAddCommandCanExecute(object Parameter)
		{
			return IsLoaded;
		}

		private async void AddCommandExecute(object Parameter)
		{
			ItemViewModelType viewModel;
			ItemModelType model;

			model = await OnCreateEmptyModelAsync();
			viewModel = await OnCreateViewModelItem(model.GetType());
			if (!await viewModel.LoadAsync(model)) return;
			
			if (await AddAsync(viewModel, EditItemOnAdd)) await OnAddCommandExecuted(viewModel);
			           
        }
		public async virtual Task OnAddCommandExecuted(ItemViewModelType ViewModel)
		{
			await Task.Yield();
		}

		protected virtual bool OnRemoveCommandCanExecute(object Parameter)
		{
			ItemViewModelType item;

			item = Parameter as ItemViewModelType;
			if (item == null) item = SelectedItem;

			return (IsLoaded) && (item != null);
		}
		private async void RemoveCommandExecute(object Parameter)
		{
			ItemViewModelType item;
			ItemViewModelType[] items;

			item = Parameter as ItemViewModelType;
			if (item == null)
			{
				items = SelectedItems.ToArray();
			}
			else
			{
				items = new ItemViewModelType[] { item };
			}
			foreach (ItemViewModelType viewModel in items)
			{
				if (await RemoveAsync(viewModel)) await OnRemoveCommandExecuted(viewModel);
			}
			
		}
		public async virtual Task OnRemoveCommandExecuted(ItemViewModelType ViewModel)
		{
			await Task.Yield();
		}

		protected virtual bool OnEditCommandCanExecute(object Parameter)
		{
			ItemViewModelType item;

			item = Parameter as ItemViewModelType;
			if (item == null) item = SelectedItem;

			return (IsLoaded) && (item != null);
		}
		private async void EditCommandExecute(object Parameter)
		{
			ItemViewModelType item;
			item = Parameter as ItemViewModelType;
			if (item == null) await EditAsync();
			else await EditAsync(item);
		}

		public async virtual Task OnEditCommandExecuted(ItemViewModelType ViewModel)
		{
			await Task.Yield();
		}

		public async Task<bool> EditAsync(bool ShowEditWindow = true)
		{
			return await EditAsync(SelectedItems.ToArray(), ShowEditWindow);
		}
		public async Task<bool> EditAsync(ItemViewModelType Item, bool ShowEditWindow = true)
		{
			return await EditAsync(new ItemViewModelType[] { Item }, ShowEditWindow);
		}

		public async Task<bool> EditAsync(ItemViewModelType[] Items, bool ShowEditWindow = true)
		{
			ViewModelSchema schema;

			schema = new ViewModelSchema(Items, typeof(ItemViewModelType));

			return await EditAsync(schema, ShowEditWindow);
		}
		public async Task<bool> EditAsync(ViewModelSchema Schema, bool ShowEditWindow = true)
		{
			Window window;
			bool result;



			if (ShowEditWindow)
			{
				window = OnCreateEditWindow();
				window.Owner = Application.Current.MainWindow;
				Schema.Window = window;
				window.DataContext = Schema;
				if (!window.ShowDialog() ?? false)
				{
					Schema.Revert();
					return false;
				}
			}

			



			result = true;
			foreach (ItemViewModelType viewModel in Schema.ViewModels)
			{
				try
				{
					if (await OnEditInModelAsync(viewModel)) await OnEditCommandExecuted(viewModel);
					else
					{
						result = false;
						Schema.Revert(viewModel);
					}
				}
				catch (Exception ex)
				{
					result = false;
					ViewModelLib.ViewModel.Log(ex);
					Schema.Revert(viewModel);
				}
			}

			return result;
		}

		public async Task<ItemViewModelType> AddAsync(ItemModelType Model, bool ShowEditWindow = false)
		{
			return await AddAsync(this.items.Count, Model,ShowEditWindow);
		}
		public async Task<ItemViewModelType> AddAsync(int Index, ItemModelType Model, bool ShowEditWindow = false)
		{
			ItemViewModelType vm;
			vm = await OnCreateViewModelItem(Model.GetType());
			if (!await vm.LoadAsync(Model)) return null;
			if (!await AddAsync(Index,vm,ShowEditWindow)) return null;
			return vm;
		}

		public async Task<bool> AddAsync(ItemViewModelType ViewModel, bool ShowEditWindow = false)
		{
			return await AddAsync(items.Count, ViewModel,ShowEditWindow);
		}
		public async Task<bool> AddAsync(int Index,ItemViewModelType ViewModel,bool ShowEditWindow=false)
		{
			Window window;
			ViewModelSchema schema;


			if (!ViewModel.IsLoaded)
			{
				if (!await ViewModel.LoadAsync()) return false;
			}


			if (ShowEditWindow)
			{
				window = OnCreateEditWindow();
				window.Owner = Application.Current.MainWindow;

				schema = new ViewModelSchema(new ItemViewModelType[] { ViewModel }, typeof(ItemViewModelType));
				schema.Window = window;

				window.DataContext = schema;
				if (!window.ShowDialog() ?? false) return false;
			}

			//if (ApplyOnModel)
			//{
				try
				{
					if (!await OnAddInModelAsync(ViewModel)) return false;
				}
				catch (Exception ex)
				{
					ViewModelLib.ViewModel.Log(ex);
					return false;
				}
			//}

			items.Insert(Index,ViewModel);
			if (SelectItemOnAdd) SelectedItem = ViewModel;
			await OnItemAddedAsync(ViewModel, Index);
			
			return true;
		}

		public async Task<bool> RemoveAsync(ItemModelType Model)
		{
			ItemViewModelType vm;


			vm = this.FirstOrDefault(item => ReferenceEquals(item.Model, Model));
			if (vm == null) return false;

			return await RemoveAsync(vm);
		}
		public async Task<bool> RemoveAsync(ItemViewModelType ViewModel)
		{
			int index;

			index = items.IndexOf(ViewModel);
			//if (ApplyOnModel)
			//{
				try
				{
					if (!await OnRemoveFromModelAsync(ViewModel)) return false;
				}
				catch (Exception ex)
				{
					ViewModelLib.ViewModel.Log(ex);
					return false;
				}
			//}

			items.Remove(ViewModel);
			await OnItemRemovedAsync(ViewModel, index);

			return true;
		}


		protected virtual async Task OnItemAddedAsync(ItemViewModelType Item, int Index)
		{
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Item, Index));
			OnPropertyChanged("Count");
			await Task.Yield();
		}
		protected virtual async Task OnItemRemovedAsync(ItemViewModelType Item, int Index)
		{
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Item, Index));
			OnPropertyChanged("Count");
			await Task.Yield();
		}
		protected virtual async Task OnItemsResetedAsync()
		{
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			OnPropertyChanged("Count");
			await Task.Yield();
		}

		public IEnumerator<ItemViewModelType> GetEnumerator()
		{
			if (!IsLoaded) return Enumerable.Empty<ItemViewModelType>().GetEnumerator();
			return items.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public abstract bool Equals(ItemModelType x, ItemModelType y);

		public int GetHashCode(ItemModelType obj)
		{
			return obj.GetHashCode();
		}



	}
}
