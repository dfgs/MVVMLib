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
	public class TextPropertyAttribute : PropertyAttribute
	{
		public override IViewModelProperty CreateViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, bool AutoApply)
		{
			return new TextViewModelProperty(ViewModels,pd, Header ?? pd.Name, Category, IsMandatory, IsReadOnly,AutoApply);
		}
	}
}
