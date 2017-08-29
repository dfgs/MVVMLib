using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewLib;
using ViewModelLib.Attributes;
using ViewModelLib.ViewModelProperties;

namespace ViewModelLib
{
	
	public class ViewModelSchema:DependencyObject,IValidate
	{
		
        public static readonly DependencyProperty ApplyCommandProperty = DependencyProperty.Register("ApplyCommand", typeof(ViewModelCommand), typeof(ViewModelSchema));
        public ViewModelCommand ApplyCommand
        {
            get { return (ViewModelCommand)GetValue(ApplyCommandProperty); }
            private set { SetValue(ApplyCommandProperty, value); }
        }
        public static readonly DependencyProperty OKCommandProperty = DependencyProperty.Register("OKCommand", typeof(ViewModelCommand), typeof(ViewModelSchema));
        public ViewModelCommand OKCommand
        {
            get { return (ViewModelCommand)GetValue(OKCommandProperty); }
            private set { SetValue(OKCommandProperty, value); }
        }
        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register("CancelCommand", typeof(ViewModelCommand), typeof(ViewModelSchema));
        public ViewModelCommand CancelCommand
        {
            get { return (ViewModelCommand)GetValue(CancelCommandProperty); }
            private set { SetValue(CancelCommandProperty, value); }
        }


        public static readonly DependencyProperty WindowProperty = DependencyProperty.Register("Window", typeof(Window), typeof(ViewModelSchema));
        public Window Window
        {
            get { return (Window)GetValue(WindowProperty); }
            set { SetValue(WindowProperty, value); }
        }


        private Dictionary<string, IViewModelProperty> properties;
		public IEnumerable<IViewModelProperty> Properties
		{
			get { return properties.Values; }
		}

		private IEnumerable<IViewModel> viewModels;
        public IEnumerable<IViewModel> ViewModels
        {
            get { return viewModels; }
        }

        public IViewModelProperty this[string Name]
        {
            get { return GetProperty(Name); }
        }

		public ViewModelSchema(IEnumerable<IViewModel> ViewModels,Type ViewModelType,bool AutoApply=false)
		{
			PropertyDescriptorCollection pds;
			PropertyAttribute propertyAttribute;
			IViewModelProperty property;

            ApplyCommand = new ViewModelCommand(OnApplyCommandCanExecute, OnApplyCommandExecute);
            OKCommand = new ViewModelCommand(OnOKCommandCanExecute, OnOKCommandExecute);
            CancelCommand = new ViewModelCommand(OnCancelCommandCanExecute, OnCancelCommandExecute);


            this.viewModels = ViewModels;
			properties = new Dictionary<string, IViewModelProperty>();

			pds = TypeDescriptor.GetProperties(ViewModelType);
			foreach (PropertyDescriptor pd in pds)
			{
				propertyAttribute = pd.Attributes.OfType<PropertyAttribute>().FirstOrDefault();
				if (propertyAttribute == null) continue;
				property = propertyAttribute.CreateViewModelProperty(ViewModels, pd,AutoApply);
				properties.Add(pd.Name, property);
			}

		}

		public IViewModelProperty GetProperty(string PropertyName)
		{
			IViewModelProperty property;
			properties.TryGetValue(PropertyName, out property);
			return property;
			
		}


		//
		public bool Validate()
		{
			if (Properties.FirstOrDefault() == null) return true;

			foreach (IViewModelProperty property in Properties)
			{
				if (!property.Validate()) return false;
			}

			return true;
		}

		public void Commit()
		{
			foreach (IViewModelProperty property in Properties)
			{
				property.Commit();
			}
		}

		public void Revert()
		{
			foreach (IViewModelProperty property in Properties)
			{
				property.Revert();
			}
		}
		public void Revert(IViewModel ViewModel)
		{
			foreach (IViewModelProperty property in Properties)
			{
				property.Revert(ViewModel);
			}
		}




		private bool OnApplyCommandCanExecute(object Parameter)
        {
            return Validate();
        }
        private void OnApplyCommandExecute(object Parameter)
        {
            this.Commit();
        }

        private bool OnOKCommandCanExecute(object Parameter)
        {
            return Validate();
        }
        private void OnOKCommandExecute(object Parameter)
        {
            this.Commit();
            if (Window != null) Window.DialogResult = true;
        }

        private bool OnCancelCommandCanExecute(object Parameter)
        {
            return true;
        }
        private void OnCancelCommandExecute(object Parameter)
        {
            this.Revert();
            if (Window != null) Window.DialogResult = false;
        }



    }
}
