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
	public class BytePropertyAttribute:PropertyAttribute
	{
		public byte MinValue
		{
			get;
			set;
		} = byte.MinValue;
		public byte MaxValue
		{
			get;
			set;
		} = byte.MaxValue;

	
		public override IViewModelProperty CreateViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd,bool AutoApply)
		{
			return new ByteViewModelProperty(ViewModels,pd, Header ?? pd.Name, IsMandatory, IsReadOnly,AutoApply,MinValue,MaxValue);
		}

	}
}
