using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViewModelLib
{
	public abstract class DocumentViewModel<ModelType> : ViewModel<ModelType>
	{

		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(DocumentViewModel<ModelType>));


		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}


		public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(DocumentViewModel<ModelType>));
		public string FileName
		{
			get { return (string)GetValue(FileNameProperty); }
			set { SetValue(FileNameProperty, value); }
		}
		

		
		public DocumentViewModel()
		{

			Header = "New document";
			FileName = null;
		}



		public async Task SaveToFileAsync()
		{
			if (FileName!=null)	await SaveToFileAsync(FileName);
		}
		public async Task SaveToFileAsync(string FileName)
		{
			await OnSaveModelAsync(FileName);
			this.FileName = FileName;
			this.Header = System.IO.Path.GetFileNameWithoutExtension(FileName);
		}
		protected abstract Task OnSaveModelAsync(string FileName);
		

		public async Task LoadFromFileAsync(string FileName)
		{
			ModelType model;

			model = await OnLoadModelAsync(FileName);
			await LoadAsync(model);

			this.FileName = FileName;
			this.Header = System.IO.Path.GetFileNameWithoutExtension(FileName);
		}

		protected abstract Task<ModelType> OnLoadModelAsync(string FileName);

		


	}

}
