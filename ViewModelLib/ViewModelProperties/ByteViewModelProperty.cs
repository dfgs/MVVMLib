using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib.ViewModelProperties
{
	public class ByteViewModelProperty : NumericViewModelProperty<byte?>
	{
		


		public ByteViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header, string Category, bool IsMandatory, bool IsReadOnly, bool AutoApply,byte? MinValue,byte? MaxValue)
			: base(ViewModels, pd, Header,Category, IsMandatory, IsReadOnly,AutoApply,MinValue,MaxValue)
		{
		}

		protected override byte? OnInc(byte? Value)
		{
			if (Value == null) return 1;
			return (byte)(Value + 1);
		}
		protected override byte? OnDec(byte? Value)
		{
			if (Value == null) return 0;
			return (byte)(Value - 1);
		}

		
	}
}
