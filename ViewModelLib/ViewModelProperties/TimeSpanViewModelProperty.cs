using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViewModelLib.ViewModelProperties
{
	public class TimeSpanViewModelProperty: ViewModelProperty<TimeSpan?>
	{
		/*public override bool IsEqualToDefault
		{
			get { return (Value==DateTime.MinValue) || (Value==null);	}
		}*/

		public TimeSpanViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header,bool IsMandatory, bool IsReadOnly, bool AutoApply)
			:base(ViewModels,pd,Header,IsMandatory,IsReadOnly,AutoApply)
		{
		}

		
	}
}
