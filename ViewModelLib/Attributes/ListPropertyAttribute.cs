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
	public class ListPropertyAttribute:PropertyAttribute
	{
		public string SourcePath
		{
			get;
			set;
		}

		public string SelectedValuePath
		{
			get;
			set;
		}

		public string DisplayMemberPath
		{
			get;
			set;
		}

		public override IViewModelProperty CreateViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, bool AutoApply)
		{
			return new ListViewModelProperty(ViewModels, pd, Header ?? pd.Name, IsMandatory, IsReadOnly,AutoApply, SourcePath, SelectedValuePath, DisplayMemberPath);
		}

	}
}
