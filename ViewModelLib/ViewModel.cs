using ModelLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewLib;
using ViewModelLib.Attributes;
using ViewModelLib.ViewModelProperties;

namespace ViewModelLib
{
	public abstract class ViewModel:DependencyObject, INotifyPropertyChanged
	{
		
		public event PropertyChangedEventHandler PropertyChanged;

		private static readonly ObservableCollection<string> errors;
		public static ObservableCollection<string> Errors
		{
			get { return errors; }
		}


		static ViewModel()
		{
			errors = new ObservableCollection<string>();
		}

		public static void Log(Exception ex)
		{
			errors.Add("Unexpected exception occured: " + ex.Message);
		}
		public static void Log(string Message)
		{
			errors.Add(Message);
		}

		protected virtual void OnPropertyChanged([CallerMemberName]string PropertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
		}
	}

	public abstract class ViewModel<ModelType>: ViewModel, IViewModel
	{

		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ViewModel<ModelType>));
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		

		public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(ViewModel<ModelType>));
		public bool IsLoading
		{
			get { return (bool)GetValue(IsLoadingProperty); }
			private set { SetValue(IsLoadingProperty, value); }
		}

		public static readonly DependencyProperty IsLoadedProperty = DependencyProperty.Register("IsLoaded", typeof(bool), typeof(ViewModel<ModelType>));
		public bool IsLoaded
		{
			get { return (bool)GetValue(IsLoadedProperty); }
			private set { SetValue(IsLoadedProperty, value); }
		}

		private List<IViewModel> children;
		public List<IViewModel> Children
		{
			get { return children; }
		}


		private ModelType model;
		public ModelType Model
		{
			get {return model;}

		}
		object IViewModel.Model
		{
			get { return model; }
		}


		public ViewModel()
		{
			children = new List<IViewModel>();
		}

		public async Task<bool> LoadAsync()
		{
			bool result;

			if (IsLoading) return false;

			IsLoading = true;
			result = await OnLoadingAsync();
			//IsLoaded = false; // is loaded update must occur after OnLoadingAsync in order to save selected item

			if (result)
			{ 
				try
				{
					model = await OnLoadModelAsync();
					if (model == null)
					{
						Log("Error while loading " + typeof(ModelType) + ": Model cannot be null");
						result=false;
					}
				}
				catch (Exception ex)
				{
					Log(ex);
					result = false;
				}
			}		
			foreach (IViewModel child in children)
			{
				await child.LoadAsync();
			}
			IsLoaded = result;
			await OnLoadedAsync();
			
			IsLoading = false;

			return result;
		}

		public async Task<bool> LoadAsync(ModelType Model)
		{
			bool result;

			if (IsLoading) return false;

			//IsLoaded = false;
			IsLoading = true;
			result = await OnLoadingAsync();
			if (result)
			{
				model = Model;
			}
			foreach (IViewModel child in children)
			{
				await child.LoadAsync();
			}
			IsLoaded = result;
			await OnLoadedAsync();
			
			IsLoading = false;

			return result;
		}
		protected async virtual Task<bool> OnLoadingAsync()
		{
			return await Task.FromResult(true);
		}

		protected async virtual Task OnLoadedAsync()
		{
			await Task.Yield();
		}

		protected abstract Task<ModelType> OnLoadModelAsync();

		public virtual bool IsModelEqualTo(ModelType Other)
		{
			return ValueType.Equals(Other,Model);
		}
		

		

		



	}


}
