using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewLib
{
	public interface IValidate
	{

		bool Validate();
		void Commit();
		void Revert();
		

	}
}
