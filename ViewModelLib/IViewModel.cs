using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewLib;
using ViewModelLib.ViewModelProperties;

namespace ViewModelLib
{
	public interface IViewModel
	{
		object Model
		{
			get;
		}

		Task<bool> LoadAsync();
		
	}
	
}
