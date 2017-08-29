using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ViewLib
{
	public class PropertyDataTemplateSelector:DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			FrameworkElement element;
			

			element = container as FrameworkElement;
			if ((item == null) || (element==null)) return null;

			return element.TryFindResource(item.GetType().Name) as DataTemplate;
		}
	}
}
