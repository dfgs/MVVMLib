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
	public class IntListViewModelProperty: ListViewModelProperty<int?>
	{

		public IntListViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header, string Category, bool IsMandatory,bool IsReadOnly, bool AutoApply, string SourcePath, string SelectedValuePath, string DisplayMemberPath) 
			:base(ViewModels,pd,Header,Category,IsMandatory,IsReadOnly,AutoApply,SourcePath,SelectedValuePath,DisplayMemberPath)
		{

		}




	}
}
