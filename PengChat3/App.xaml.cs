using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace PengChat3
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        internal static int Port = 13333;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // PengChat3 registry
            if (RegistryManager.OpenRegistry() == false)
                Utility.Error("Could not open registry " + RegistryManager.DefaultPath, true);

            // Load language pack
            string LanguagePackDll;
            object temp = RegistryManager.ReadValue(RegistryManager.LanguagePackPath);

            // If the value is exists
            if (temp != null)
                LanguagePackDll = temp.ToString();
            else
            {
                RegistryManager.WriteValue(RegistryManager.LanguagePackPath, "PC3LP_ko.dll");
                LanguagePackDll = RegistryManager.ReadValue(RegistryManager.LanguagePackPath).ToString();
            }

            try
            {
                ResourceManager.LoadResource(LanguagePackDll, "PC3LP_ko.Properties.Resources");
            }
            catch (Exception ex)
            {
                Utility.Error("Could not load language pack \"" + LanguagePackDll + "\"\n" + ex.Message, true);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (RegistryManager.IsOpened())
                RegistryManager.CloseRegistry();
        }
    }
}
