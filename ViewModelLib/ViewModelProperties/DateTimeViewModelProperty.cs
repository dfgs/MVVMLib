using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViewModelLib.ViewModelProperties
{
	public class DateTimeViewModelProperty: ViewModelProperty<DateTime?>
	{
		/*public override bool IsEqualToDefault
		{
			get { return (Value==DateTime.MinValue) || (Value==null);	}
		}*/

		public DateTimeViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header, string Category, bool IsMandatory, bool IsReadOnly, bool AutoApply)
			:base(ViewModels,pd,Header,Category,IsMandatory,IsReadOnly,AutoApply)
		{
		}

		
	}
}
