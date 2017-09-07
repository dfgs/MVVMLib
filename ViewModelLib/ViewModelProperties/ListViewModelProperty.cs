using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ViewModelLib.ViewModelProperties
{
	public abstract class ListViewModelProperty<ValType>: ViewModelProperty<ValType>
	{
		private IEnumerable items;
		public IEnumerable Items
		{
			get
			{
				return items;
			}
		}
		private string sourcePath;
		public string SourcePath
		{
			get { return sourcePath; }
		}

		private string selectedValuePath;
		public string SelectedValuePath
		{
			get { return selectedValuePath; }
		}
		private string displayMemberPath;
		public string DisplayMemberPath
		{
			get { return displayMemberPath; }
		}

		

		public ListViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header,bool IsMandatory,bool IsReadOnly, bool AutoApply, string SourcePath, string SelectedValuePath, string DisplayMemberPath) 
			:base(ViewModels,pd,Header,IsMandatory,IsReadOnly,AutoApply)
		{
			this.sourcePath = SourcePath; this.displayMemberPath = DisplayMemberPath; this.selectedValuePath = SelectedValuePath;
			items = GetDeepPropertyValue(ViewModels.FirstOrDefault(), sourcePath) as IEnumerable;

		}

		public object GetDeepPropertyValue(object instance, string path)
		{
			string[] parts = path.Split('.');
			Type type;
			PropertyInfo propInfo;

			foreach (string part in parts)
			{
				if (instance == null) return null;
				type = instance.GetType();
				propInfo = type.GetProperty(part);
				if (propInfo != null) instance = propInfo.GetValue(instance, null);
				else throw new ArgumentException("Properties path is not correct ("+path+")");
			}
			return instance;
		}



	}
}
