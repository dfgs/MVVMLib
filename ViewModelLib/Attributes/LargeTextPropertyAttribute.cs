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
	public class LargeTextPropertyAttribute : PropertyAttribute
	{
		public override IViewModelProperty CreateViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, bool AutoApply)
		{
			return new LargeTextViewModelProperty(ViewModels,pd, Header ?? pd.Name, Category, IsMandatory, IsReadOnly,AutoApply);
		}
	}
}
