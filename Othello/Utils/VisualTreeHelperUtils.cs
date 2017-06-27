using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Othello.Utils
{
    public static class VisualTreeHelperUtil
    {
        #region Visual Tree Help

        public static DependencyObject GetParent(DependencyObject o, Func<DependencyObject, bool> matchFunction)
        {
            DependencyObject t = o;
            if (!(t is Visual))
                return null;
            do
            {
                t = VisualTreeHelper.GetParent(t);
            } while (t != null && !matchFunction.Invoke(t));
            return t;
        }

        public static DependencyObject GetParentOrItem(DependencyObject o, Func<DependencyObject, bool> matchFunction)
        {
            DependencyObject t = o;
            if (!(t is Visual) || (t is TextBox) || (t is ComboBox))
                return null;
            while (t != null && !matchFunction.Invoke(t))
            {
                t = VisualTreeHelper.GetParent(t);
                if (!(t is Visual) || (t is TextBox) || (t is ComboBox))
                    return null;
            }
            return t;
        }

        #endregion
    }

}
