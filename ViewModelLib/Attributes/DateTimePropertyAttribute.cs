using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib.ViewModelProperties;

namespace ViewModelLib.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DateTimePropertyAttribute:PropertyAttribute
	{


		public override IViewModelProperty CreateViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, bool AutoApply)
		{
			return new DateTimeViewModelProperty(ViewModels,pd, Header ?? pd.Name, IsMandatory, IsReadOnly,AutoApply);
		}

	}
}
