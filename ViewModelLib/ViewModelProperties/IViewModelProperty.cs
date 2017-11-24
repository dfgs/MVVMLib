using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewLib;

namespace ViewModelLib.ViewModelProperties
{
	public interface IViewModelProperty:IValidate
	{
		bool IsMandatory
		{
			get;
		}


		object Value
		{
			get;
			set;
		}

		string Name
		{
			get;
		}
		string Header
		{
			get;
		}

		void Revert(IViewModel ViewModel);
		//bool IsValueInvalid(object Value);

	}
}
