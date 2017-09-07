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
	public class TextListViewModelProperty: ListViewModelProperty<string>
	{

		public TextListViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header,bool IsMandatory,bool IsReadOnly, bool AutoApply, string SourcePath, string SelectedValuePath, string DisplayMemberPath) 
			:base(ViewModels,pd,Header,IsMandatory,IsReadOnly,AutoApply,SourcePath,SelectedValuePath,DisplayMemberPath)
		{

		}




	}
}
