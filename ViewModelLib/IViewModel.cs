using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewLib;
using ViewModelLib.ViewModelProperties;

namespace ViewModelLib
{
	public interface IViewModel:INotifyPropertyChanged
	{
		object Model
		{
			get;
		}

		Task<bool> LoadAsync();
		
	}
	
}
