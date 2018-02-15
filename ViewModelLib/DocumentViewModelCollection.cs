using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViewModelLib
{
	public class DocumentViewModelCollection<ViewModelType,ModelType> : ListViewModel<ViewModelType, ModelType>
		where ViewModelType:DocumentViewModel<ModelType>,new()
		where ModelType:new()
	{

		public static readonly DependencyProperty OpenCommandProperty = DependencyProperty.Register("OpenCommand", typeof(ViewModelCommand), typeof(DocumentViewModelCollection<ViewModelType, ModelType>));
		public ViewModelCommand OpenCommand
		{
			get { return (ViewModelCommand)GetValue(OpenCommandProperty); }
			private set { SetValue(OpenCommandProperty, value); }
		}

		public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.Register("SaveCommand", typeof(ViewModelCommand), typeof(DocumentViewModelCollection<ViewModelType, ModelType>));
		public ViewModelCommand SaveCommand
		{
			get { return (ViewModelCommand)GetValue(SaveCommandProperty); }
			private set { SetValue(SaveCommandProperty, value); }
		}

		public static readonly DependencyProperty SaveAsCommandProperty = DependencyProperty.Register("SaveAsCommand", typeof(ViewModelCommand), typeof(DocumentViewModelCollection<ViewModelType, ModelType>));
		public ViewModelCommand SaveAsCommand
		{
			get { return (ViewModelCommand)GetValue(SaveAsCommandProperty); }
			private set { SetValue(SaveAsCommandProperty, value); }
		}

		public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ViewModelCommand), typeof(DocumentViewModelCollection<ViewModelType, ModelType>));
		public ViewModelCommand CloseCommand
		{
			get { return (ViewModelCommand)GetValue(CloseCommandProperty); }
			private set { SetValue(CloseCommandProperty, value); }
		}



		public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(DocumentViewModelCollection<ViewModelType, ModelType>));
		public bool IsBusy
		{
			get { return (bool)GetValue(IsBusyProperty); }
			protected set { SetValue(IsBusyProperty, value); }
		}


		public DocumentViewModelCollection()
		{
			OpenCommand = new ViewModelCommand(OnOpenCommandCanExecute, OpenCommandExecute);
			SaveCommand = new ViewModelCommand(OnSaveCommandCanExecute, SaveCommandExecute);
			SaveAsCommand = new ViewModelCommand(OnSaveAsCommandCanExecute, SaveAsCommandExecute);
			CloseCommand = new ViewModelCommand(OnCloseCommandCanExecute, CloseCommandExecute);

		}
		protected override Task<IList<ModelType>> OnLoadModelAsync()
		{
			List<ModelType> result;

			result = new List<ModelType>();
			return Task.FromResult((IList<ModelType>)result);
		}

	
		protected override Task<ModelType> OnCreateEmptyModelAsync()
		{
			return Task.FromResult(new ModelType());
		}
		protected override Task<ViewModelType> OnCreateViewModelItem(Type ModelType)
		{
			return Task.FromResult(new ViewModelType());
		}

		protected override async Task OnItemAddedAsync(ViewModelType Item, int Index)
		{
			if (SelectedItem != null) SelectedItem.IsSelected = false;
			Item.IsSelected = true;
			await base.OnItemAddedAsync(Item, Index);
		}

		protected virtual bool OnOpenCommandCanExecute(object Parameter)
		{
			return IsLoaded;
		}
		private async void OpenCommandExecute(object Parameter)
		{
			OpenFileDialog dialog;
			ViewModelType viewModel;

			dialog = new OpenFileDialog();
			dialog.Title = "Open document";
			if (dialog.ShowDialog(Application.Current.MainWindow)??false)
			{
				IsBusy = true;
				try
				{
					viewModel = new ViewModelType();
					await viewModel.LoadFromFileAsync(dialog.FileName);
					await this.AddAsync(viewModel);
				}
				catch(Exception ex)
				{
					Log(ex.Message);
					return;
				}
				finally
				{
					IsBusy = false;
				}
			}
		}

		protected virtual bool OnSaveCommandCanExecute(object Parameter)
		{
			return (SelectedItem!=null) && (SelectedItem.FileName!=null);
		}
		private async void SaveCommandExecute(object Parameter)
		{
			try
			{
				await SelectedItem.SaveToFileAsync();
			}
			catch (Exception ex)
			{
				Log(ex.Message);
				return;
			}
			
		}

		protected virtual bool OnSaveAsCommandCanExecute(object Parameter)
		{
			return (SelectedItem != null) ;
		}
		private async void SaveAsCommandExecute(object Parameter)
		{
			SaveFileDialog dialog;

			dialog = new SaveFileDialog();
			dialog.Title = "Save document";
			if (dialog.ShowDialog(Application.Current.MainWindow) ?? false)
			{
				IsBusy = true;
				try
				{
					await SelectedItem.SaveToFileAsync(dialog.FileName);
				}
				catch (Exception ex)
				{
					Log(ex.Message);
					return;
				}
				finally
				{
					IsBusy = false;
				}
			}
		}


		protected virtual bool OnCloseCommandCanExecute(object Parameter)
		{
			return SelectedItem!=null;
		}
		private async void CloseCommandExecute(object Parameter)
		{
			await RemoveAsync(SelectedItem);
		}

		public override bool Equals(ModelType x, ModelType y)
		{
			return ReferenceEquals(x , y);
		}


	}
}
