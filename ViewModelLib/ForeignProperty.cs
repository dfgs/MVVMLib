using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ViewModelLib
{
	public class ForeignProperty<ComponentType,ViewModelType>
		where ComponentType:ViewModel
		where ViewModelType:ViewModel
	{
		private Func<ComponentType, ViewModelType, bool> matchPredicate;
		private Func<ComponentType, IEnumerable<ViewModelType>> sourcePredicate;

		private string name;
		public string Name
		{
			get { return name; }
		}

		

		private Dictionary<ComponentType,ViewModelType> values;

		private static Regex nameRegex = new Regex(@"^(.*)Property$");


		public ForeignProperty(Func<ComponentType,IEnumerable<ViewModelType>> SourcePredicate,Func<ComponentType,ViewModelType,bool> MatchPredicate, [CallerMemberName]string Name = null)
		{
			Match match;

			this.sourcePredicate = SourcePredicate;
			this.matchPredicate = MatchPredicate;

			values = new Dictionary<ComponentType, ViewModelType>();

			match = nameRegex.Match(Name);
			if (match.Success) name = match.Groups[1].Value;
			else name = Name;
		}

		public ViewModelType GetValue(ComponentType Component)
		{
			ViewModelType result;
			IEnumerable<ViewModelType> source;

			source = sourcePredicate(Component);
				
			if (!values.TryGetValue(Component,out result))
			{
				if (source == null) result = null;
				else result = source.FirstOrDefault( (item) => matchPredicate(Component,item));
				values.Add(Component, result);
			}
			
			return result;
		}


		public void Invalidate(ComponentType Component)
		{
			values.Remove(Component);
			Component.InvalidateProperty(name);
		}

	}


}
