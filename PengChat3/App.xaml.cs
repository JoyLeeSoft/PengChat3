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
                Utility.Error("Could not load language pack \"" + LanguagePackName + "\"\n" + ex.Message, true);
            }

            ChatTabItem.InitImages();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (RegistryManager.IsOpened())
                RegistryManager.CloseRegistry();
        }
    }
}
