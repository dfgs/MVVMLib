using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViewModelLib.ViewModelProperties
{
	public class LargeTextViewModelProperty: ViewModelProperty<string>
	{
		

		public LargeTextViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header, string Category, bool IsMandatory, bool IsReadOnly, bool AutoApply) 
			:base(ViewModels,pd,Header,Category,IsMandatory,IsReadOnly,AutoApply)
		{
		}

		public override bool OnValidateValue(string Value)
		{
			if (IsNullable) return true;
			return !String.IsNullOrWhiteSpace(Value);
		}
	}
}
