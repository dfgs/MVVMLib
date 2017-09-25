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
	public abstract class ViewModelCollection<CollectionModelType,ItemViewModelType,ItemModelType> : ViewModel<CollectionModelType>, IViewModelCollection<ItemViewModelType>
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

		private ItemViewModelType savedSelectedItem;


		public event NotifyCollectionChangedEventHandler CollectionChanged;
		

		public ViewModelCollection()
		{
			items = new List<ItemViewModelType>();
			AddCommand = new ViewModelCommand(OnAddCommandCanExecute, AddCommandExecute);
			RemoveCommand = new ViewModelCommand(OnRemoveCommandCanExecute, RemoveCommandExecute);
			EditCommand = new ViewModelCommand(OnEditCommandCanExecute, EditCommandExecute);
			
		}
	
		

		
		protected override async Task<bool> OnLoadingAsync()
		{
			savedSelectedItem = SelectedItem;
			items.Clear();
			return await base.OnLoadingAsync();
		}
		protected override async Task OnLoadedAsync()
		{
			ItemViewModelType item;

			if (Model != null)
			{
				foreach (ItemModelType model in Model)
				{
					item = await OnCreateViewModelItem(model.GetType());
					await item.LoadAsync(model);
					items.Add(item);
				}
			}

			if (savedSelectedItem == null) SelectedItem = OnGetDefaultSelectedItem() ;
			else
			{
				SelectedItem = this.FirstOrDefault(i => i.IsModelEqualTo(savedSelectedItem.Model ));
			}
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			OnPropertyChanged("Count");
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
			await EditAsync(Parameter as ItemViewModelType);
		}

		public async virtual Task OnEditCommandExecuted(ItemViewModelType ViewModel)
		{
			await Task.Yield();
		}

		public async Task EditAsync(ItemViewModelType Item)
		{
			ItemViewModelType[] items;
			Window window;
			ViewModelSchema schema;
			bool result;

			if (Item == null)
			{
				items = SelectedItems.ToArray();
			}
			else
			{
				items = new ItemViewModelType[] { Item };
			}

			schema = new ViewModelSchema(items, typeof(ItemViewModelType));

			window = OnCreateEditWindow();
			window.Owner = Application.Current.MainWindow;
			schema.Window = window;
			window.DataContext = schema;
			result = window.ShowDialog() ?? false;
			if (result)
			{
				foreach (ItemViewModelType viewModel in items)
				{
					try
					{
						if (await OnEditInModelAsync(viewModel)) await OnEditCommandExecuted(viewModel);
						else schema.Revert(viewModel);
					}
					catch (Exception ex)
					{
						ViewModelLib.ViewModel.Log(ex);
						schema.Revert(viewModel);
					}
					
				}
			}
			else schema.Revert();
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

	}
}
