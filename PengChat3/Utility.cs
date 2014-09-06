using System;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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
}