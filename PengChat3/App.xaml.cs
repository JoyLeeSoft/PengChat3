﻿//#define TEST

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Text;

using Octokit;

namespace PengChat3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        internal static int Port = 13333;
        internal static MainWindow Instance = null;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if !TEST
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
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

            ChatTab.InitImages();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (RegistryManager.IsOpened())
                RegistryManager.CloseRegistry();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            //Utility.Error("Fatal error : PengChat3 unhandled exception!" + '\n' +
                //ex.Message + '\n' + "HRESULT : " + ex.HResult, false, true);

            CrashReport(ex);
        }

        private void CrashReport(Exception ex)
        {
            StringBuilder body = new StringBuilder("This issue was generated by PengChat3 Crash Repoter.\n\n\n");

            body.AppendLine("OS : " + Environment.OSVersion.ToString());
            body.AppendLine("At " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            body.AppendLine("PengChat3 version : " + PengChat3.Properties.Settings.Default.Version);
            body.AppendLine("API version : " + PC3API_dn.Properties.Settings.Default.Version);
            body.AppendLine("Assembly : " + ex.Source);
            body.AppendLine("Exception : " + ex.GetType().FullName);
            body.AppendLine("Message : " + ex.Message);
            body.AppendLine("HRESULT : " + ex.HResult + '\n');
            body.AppendLine("Stack Trace : " + ex.StackTrace);

            var client = new GitHubClient(new ProductHeaderValue("PengChat3"));
            client.Credentials = new Credentials("PengChat3CrashRepoter", "pengchat3crashrepoter1");
            var i = new NewIssue("PengChat3 Crash Report : " + ex.GetType().FullName)
            {
                Body = body.ToString()
            };
            i.Labels.Add("bug");
            var task = client.Issue.Create("JoyLeeSoft", 
#if TEST
                "test", 
#else
                "PengChat3",
#endif 
                i);
            var issue = task.Result;
            
            if (MessageBox.Show("PengChat3 unhandled exception : " + ex.Message + "\n" +
                "This information automatically send to the GitHub issue.\n" +
                "Do you want to check error report?", "PengChat3 - fatal error!", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start(issue.HtmlUrl.ToString());
            }
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
