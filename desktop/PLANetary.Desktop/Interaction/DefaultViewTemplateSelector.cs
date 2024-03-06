using PLANetary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PLANetary.Interaction
{
    class DefaultViewTemplateSelector : DataTemplateSelector
    {

        const string viewNameSpace = "PLANetary.Views";

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ViewModelBase)
            {
                string viewModelName = item.GetType().Name;
                if (!viewModelName.EndsWith("ViewModel", StringComparison.InvariantCultureIgnoreCase))
                {                    
                    return null;
                }

                string viewName = viewNameSpace + "." + viewModelName.Substring(0, viewModelName.Length - 5);

                Type viewType = Type.GetType(viewName);
                var dt = new DataTemplate();
                FrameworkElementFactory spFactory = new FrameworkElementFactory(viewType);
                dt.VisualTree = spFactory;

                return dt;
            }

            return null;
        }

    }
}
