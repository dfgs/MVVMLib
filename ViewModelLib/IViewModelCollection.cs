using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib
{
	public interface IViewModelCollection<ViewModelType>: IEnumerable<ViewModelType>, INotifyCollectionChanged
	{
		ViewModelType this[int Index]
		{
			get;
		}
		int IndexOf(ViewModelType Item);
		int Count
		{
			get;
		}
	}
}
