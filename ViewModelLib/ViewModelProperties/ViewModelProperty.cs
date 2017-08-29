using ModelLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViewModelLib.ViewModelProperties
{
	public abstract class ViewModelProperty<ValType>:DependencyObject,IViewModelProperty,INotifyPropertyChanged
	{

		public static readonly DependencyProperty ClearCommandProperty = DependencyProperty.Register("ClearCommand", typeof(ViewModelCommand), typeof(ViewModelProperty<ValType>));
		public ViewModelCommand ClearCommand
		{
			get { return (ViewModelCommand)GetValue(ClearCommandProperty); }
			private set { SetValue(ClearCommandProperty, value); }
		}
		public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register("EditCommand", typeof(ViewModelCommand), typeof(ViewModelProperty<ValType>));
		public ViewModelCommand EditCommand
		{
			get { return (ViewModelCommand)GetValue(EditCommandProperty); }
			private set { SetValue(EditCommandProperty, value); }
		}




		public static readonly DependencyProperty IsLockedProperty = DependencyProperty.Register("IsLocked", typeof(bool), typeof(ViewModelProperty<ValType>));
		public bool IsLocked
		{
			get { return (bool)GetValue(IsLockedProperty); }
			private set { SetValue(IsLockedProperty, value); }
		}


		private bool isMultiValue;
		public bool IsMultiValue
		{
			get { return isMultiValue; }
		}



		private string header;
		public string Header
		{
			get { return header; }
		}

		private bool isMandatory;
		public bool IsMandatory
		{
			get { return isMandatory; }
		}


		private bool isReadOnly;
		public bool IsReadOnly
		{
			get { return isReadOnly; }
		}

		private bool autoApply;
		public bool AutoApply
		{
			get { return autoApply; }
		}

		public bool IsNullable
		{
			get { return (!isReadOnly) && (!isMandatory); }
		}

		


		private IEnumerable<IViewModel>  viewModels;
		public IEnumerable<IViewModel> ViewModels
		{
			get { return viewModels; }
		}

		private PropertyDescriptor pd;
		public PropertyDescriptor PropertyDescriptor
		{
			get { return pd; }
		}

		public string Name
		{
			get { return pd.Name; }
		}


		private object initialValue;
		object IViewModelProperty.Value
		{
			get { return Value; }
		}

		private TypeConverter converter;


		public event PropertyChangedEventHandler PropertyChanged;

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(ValType), typeof(ViewModelProperty<ValType>),new PropertyMetadata(default(ValType),ValuePropertyChanged,ValuePropertyCoerce));
		public ValType Value
		{
			get { return (ValType)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public ViewModelProperty(IEnumerable<IViewModel> ViewModels, PropertyDescriptor pd, string Header,bool IsMandatory,bool IsReadOnly,bool AutoApply)
		{
			ClearCommand = new ViewModelCommand(OnClearCommandCanExecute, OnClearCommandExecute);
			EditCommand = new ViewModelCommand(OnEditCommandCanExecute, OnEditCommandExecute);

			this.viewModels = ViewModels;
			this.pd = pd;
			this.isMandatory = IsMandatory;
			this.isReadOnly = IsReadOnly;
			this.header = Header;
			this.autoApply = AutoApply;
			converter = TypeDescriptor.GetConverter(pd.PropertyType);

			LoadViewModelValue();
		}

		private static object ValuePropertyCoerce(DependencyObject d, object baseValue)
		{
			ViewModelProperty<ValType> vp;
			vp = d as ViewModelProperty<ValType>;
			return vp.ValueCoerce(baseValue);
		}


		private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ViewModelProperty<ValType> vp;
			vp = d as ViewModelProperty<ValType>;
			vp.OnValueChanged();

		}

		



		protected virtual void OnValueChanged()
		{
			if ((AutoApply) && (Validate())) Commit();
		}

		private object ValueCoerce(object baseValue)
		{
			ValType value;

			if ((baseValue == null) && (IsNullable)) return baseValue;
			if (baseValue is ValType)
			{
				value = (ValType)baseValue;
				if (OnValidateValue(value)) return value;
				return DependencyProperty.UnsetValue;
			}
			else return DependencyProperty.UnsetValue;
			
		}


		protected virtual bool OnClearCommandCanExecute(object Parameter)
		{
			if (IsLocked) return false;
			return IsNullable;
		}

		
		public virtual bool OnValidateValue(ValType Value)
		{
			return true;
		}

		protected virtual void OnClearCommandExecute(object Parameter)
		{
			this.Value = default(ValType);
		}

		protected virtual bool OnEditCommandCanExecute(object Parameter)
		{
			return (IsLocked);
		}
		protected virtual void OnEditCommandExecute(object Parameter)
		{
			IsLocked = false;
		}




		private void LoadViewModelValue()
		{
			System.Type valType,underlyingType ;
			object value;
			
			valType = typeof(ValType);
			underlyingType = System.Nullable.GetUnderlyingType(valType);

			isMultiValue = false;
			this.initialValue = pd.GetValue(viewModels.FirstOrDefault());
			foreach(IViewModel vm in this.ViewModels)
			{
				value = pd.GetValue(vm);
				if (ValueType.Equals(value, initialValue)) continue;
				isMultiValue = true;
			}
			IsLocked = IsMultiValue;

			if (underlyingType == null)
			{
				this.Value = (ValType)System.Convert.ChangeType(initialValue, valType);
			}
			else
			{
				this.Value = (ValType)initialValue;
			}
			
		}

		public bool Validate()
		{
			if (IsLocked) return true;
			if (Value == null) return !IsMandatory;
			return OnValidateValue(Value);
		}

		public void Commit()
		{
			object convertedValue;

			if (IsLocked) return;
			

			if ((Value != null) && (Value.GetType() == pd.PropertyType))
			{
				convertedValue = Value;
			}
			else
			{
				convertedValue = converter.ConvertFrom(Value);
			}
			foreach (IViewModel viewModel in viewModels)
			{
				pd.SetValue(viewModel, convertedValue);
			}
		}
		public void Revert()
		{
			if (IsLocked) return;

			foreach (IViewModel viewModel in viewModels)
			{
				pd.SetValue(viewModel, initialValue);
			}

		}
		public void Revert(IViewModel ViewModel)
		{
			if (IsLocked) return;

			pd.SetValue(ViewModel, initialValue);
		}

		protected virtual void OnPropertyChanged(string PropertyName)
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
		}

		
	}
}
