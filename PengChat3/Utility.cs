using System;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Collections.Generic;

namespace PengChat3
{
    internal static class Utility
    {
        public static void Error(string msg, bool useWin8Style = true, bool shutdown = false)
        {
            if (useWin8Style)
            {
                App.Instance.ShowMessageAsync("PengChat3 - error", msg, MessageDialogStyle.Affirmative);
            }
            else
                MessageBox.Show(msg, "PengChat3 - error", MessageBoxButton.OK, MessageBoxImage.Error);

            if (shutdown)
                Application.Current.Shutdown(-1);
        }

        public static T Find<T>(this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var item = coll.Where(condition);

            if (item != null)
                return item.ToArray()[0];
            else
                return default(T);
        }

        public static int Remove<T>(this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }

            return itemsToRemove.Count;
        }
    }

    public static class VisualTreeHelperExtensions
    {
        public static T FindVisualParent<T>(DependencyObject depObj) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(depObj);

            if (parent == null || parent is T)
                return (T)parent;

            return FindVisualParent<T>(parent);
        }

        public static T FindVisualChild<T>(DependencyObject depObj) where T : Visual
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        public static T FindVisualChild<T>(DependencyObject depObj, string name) where T : FrameworkElement
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T && (child as T).Name.Equals(name))
                    {
                        return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        if (childOfChild.Name.Equals(name))
                            return childOfChild;
                    }
                }
            }
            return null;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                if (child != null && child is T)
                {
                    yield return (T)child;
                }

                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }
}