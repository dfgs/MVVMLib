using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib.ViewModelProperties
{
	public class LongViewModelProperty : NumericViewModelProperty<long?>
	{
		public LongViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header, string Category, bool IsMandatory, bool IsReadOnly, bool AutoApply,long MinValue,long MaxValue) 
			: base(ViewModels,pd, Header,Category, IsMandatory, IsReadOnly,AutoApply,MinValue,MaxValue)
		{
		}

		protected override long? OnInc(long? Value)
		{
			if (Value == null) return 1;
			return Value + 1;
		}
		protected override long? OnDec(long? Value)
		{
			if (Value == null) return -1;
			return Value - 1;
		}

		
	}
}
