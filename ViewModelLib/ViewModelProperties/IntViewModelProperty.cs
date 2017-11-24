using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib.ViewModelProperties
{
	public class IntViewModelProperty : NumericViewModelProperty<int?>
	{
		public IntViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header, string Category, bool IsMandatory, bool IsReadOnly, bool AutoApply,int MinValue,int MaxValue)
			: base(ViewModels, pd, Header, Category,IsMandatory, IsReadOnly,AutoApply,MinValue,MaxValue)
		{
		}

		protected override int? OnInc(int? Value)
		{
			if (Value == null) return 1;
			return Value + 1;
		}
		protected override int? OnDec(int? Value)
		{
			if (Value == null) return -1;
			return Value - 1;
		}

		
	}
}
