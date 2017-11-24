using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViewModelLib.ViewModelProperties
{
	public abstract class NumericViewModelProperty<ValType>: ViewModelProperty<ValType>
	{
		private ValType minValue;
		public ValType MinValue
		{
			get { return minValue; }
		}

		private ValType maxValue;
		public ValType MaxValue
		{
			get { return maxValue; }
		}

		public static readonly DependencyProperty IncCommandProperty = DependencyProperty.Register("IncCommand", typeof(ViewModelCommand), typeof(NumericViewModelProperty<ValType>));
		public ViewModelCommand IncCommand
		{
			get { return (ViewModelCommand)GetValue(IncCommandProperty); }
			private set { SetValue(IncCommandProperty, value); }
		}
		public static readonly DependencyProperty DecCommandProperty = DependencyProperty.Register("DecCommand", typeof(ViewModelCommand), typeof(NumericViewModelProperty<ValType>));
		public ViewModelCommand DecCommand
		{
			get { return (ViewModelCommand)GetValue(DecCommandProperty); }
			private set { SetValue(DecCommandProperty, value); }
		}

		/*public override bool IsEqualToDefault
		{
			get { return false; }
		}*/

		public NumericViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header, string Category, bool IsMandatory, bool IsReadOnly, bool AutoApply,ValType MinValue,ValType MaxValue)
			:base(ViewModels,pd,Header,Category,IsMandatory,IsReadOnly,AutoApply)
		{
			this.minValue = MinValue;this.maxValue = MaxValue;
			IncCommand = new ViewModelCommand(IncCommandCanExecute, IncCommandExecute);
			DecCommand = new ViewModelCommand(DecCommandCanExecute, DecCommandExecute);
		}


		private bool IncCommandCanExecute(object Parameter)
		{
			if (MaxValue == null) return true;
			return !ValueType.Equals(Value,MaxValue);
		}
		private void IncCommandExecute(object Parameter)
		{
			Value = OnInc(Value) ;
		}
		protected abstract ValType OnInc(ValType Value);

		private bool DecCommandCanExecute(object Parameter)
		{
			if (MinValue == null) return true;
			return !ValueType.Equals(Value, MinValue);
		}
		private void DecCommandExecute(object Parameter)
		{
			Value=OnDec(Value);
		}
		protected abstract ValType OnDec(ValType Value);


		public override bool OnValidateValue(ValType Value)
		{
			if (MinValue!=null)
			{
				if (Comparer<ValType>.Default.Compare(Value, MinValue) < 0) return false;
			}
			if (MaxValue != null)
			{
				if (Comparer<ValType>.Default.Compare(Value, MaxValue) > 0) return false;
			}
			return true;
		}


	}
}
