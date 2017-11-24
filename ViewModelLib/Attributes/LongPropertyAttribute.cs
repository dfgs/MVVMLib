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
	public class LongPropertyAttribute:PropertyAttribute
	{
		public long MinValue
		{
			get;
			set;
		} = long.MinValue;
		public long MaxValue
		{
			get;
			set;
		} = long.MaxValue;

	

		public override IViewModelProperty CreateViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, bool AutoApply)
		{
			return new LongViewModelProperty(ViewModels,pd, Header ?? pd.Name, Category, IsMandatory, IsReadOnly,AutoApply,MinValue,MaxValue);
		}

	}
}
