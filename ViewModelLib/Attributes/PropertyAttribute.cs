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
	public abstract class PropertyAttribute:Attribute
	{
		public bool IsMandatory
		{
			get;
			set;
		}
		public bool IsReadOnly
		{
			get;
			set;
		}

		public string Header
		{
			get;
			set;
		}
		
		public string Category
		{
			get;
			set;
		}
		public PropertyAttribute()
		{
			
		}
		public abstract IViewModelProperty CreateViewModelProperty(IEnumerable<IViewModel> ViewModel, PropertyDescriptor pd,bool AutoApply);


	}
}
