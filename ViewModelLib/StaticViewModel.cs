using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib
{
	public class StaticViewModel<ModelType>:ViewModel<ModelType>
	{
		private ModelType _model;
		public StaticViewModel(ModelType Model)
		{
			this._model = Model;
		}

		protected override Task<ModelType> OnLoadModelAsync()
		{
			return Task.FromResult(_model);
		}
	}
}
