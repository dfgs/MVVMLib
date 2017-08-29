using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib.ViewModelProperties
{
	public class ULongViewModelProperty : NumericViewModelProperty<ulong?>
	{
		public ULongViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header, bool IsMandatory, bool IsReadOnly, bool AutoApply,ulong? MinValue,ulong? MaxValue) 
			: base(ViewModels,pd, Header, IsMandatory, IsReadOnly,AutoApply,MinValue,MaxValue)
		{
		}

		
		protected override ulong? OnInc(ulong? Value)
		{
			if (Value == null) return 1;
			return Value + 1;
		}
		protected override ulong? OnDec(ulong? Value)
		{
			if ((Value == 0) || (Value == null)) return 0;
			return Value - 1;
		}

		
	}
}
