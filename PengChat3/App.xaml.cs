using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PengChat3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static int Port = 13333;
        internal static MainWindow Instance = null;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // PengChat3 registry
            if (RegistryManager.OpenRegistry() == false)
                Utility.Error("Could not open registry " + RegistryManager.DefaultPath, false, true);

            // Load language pack
            string LanguagePackName;
            object temp = RegistryManager.ReadValue(RegistryManager.LanguagePackName);

            // If the value is exists
            if (temp != null)
                LanguagePackName = temp.ToString();
            else
            {
                RegistryManager.WriteValue(RegistryManager.LanguagePackName, "PC3LP_ko");
                LanguagePackName = RegistryManager.ReadValue(RegistryManager.LanguagePackName).ToString();
            }

            try
            {
                ResourceManager.LoadResource(LanguagePackName + ".dll", LanguagePackName + ".Properties.Resources");
            }
            catch (Exception ex)
            {
                Utility.Error("Could not load language pack \"" + LanguagePackName + "\"\n" + ex.Message, false, true);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (RegistryManager.IsOpened())
                RegistryManager.CloseRegistry();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            Utility.Error("Fatal error : PengChat3 unhandled exception!" + '\n' +
                ex.Message + '\n' + "HRESULT : " + ex.HResult, false, true);

            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
