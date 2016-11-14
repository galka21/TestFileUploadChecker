using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FolderMonitor.Common.Plugin;
using FolderMonitor.Common.Services;


namespace FolderMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
 
    public partial class MainWindow : Window
    {
        public const string MONITORED_FOLDER_CONFIGURATION_NAME = "MonitoredFolder";
        public const string CHECK_INTERVAL_CONFIGURATION_NAME = "IntervalSeconds";
        public const string ENABLED_PLUGIN_NAMES = "EnabledPlugins";
        public const string PLUGIN_DIRECTORY = "PluginDirectory";

        private IFolderMonitorService _folderMonitorservice;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string folderToMonitorPath = GetConfiguredMonitoredFolder();
            int configuredInterval = GetConfiguredInterval();
            tbInfo.Text = "Monitored folder: " + folderToMonitorPath + " Status: Stopped";

            _folderMonitorservice = new FolderMonitorService(folderToMonitorPath, configuredInterval);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            tbInfo.Text = "Monitored folder: " + GetConfiguredMonitoredFolder() + " Status: Started";
            try
            {
                _folderMonitorservice.StartMonitor(PluginManager.LoadEnabledFileLoaders(
                    GetConfigurationValue(PLUGIN_DIRECTORY), GetConfigurationValue(ENABLED_PLUGIN_NAMES)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);// TODO: write exceptions to log
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            tbInfo.Text = "Monitored folder: " + GetConfiguredMonitoredFolder() + " Status: Stopped";
            _folderMonitorservice.StopMonitor();
        }

        private string GetConfiguredMonitoredFolder()
        {
            string folder = GetConfigurationValue(MONITORED_FOLDER_CONFIGURATION_NAME);
            if (!Directory.Exists(folder))
                throw new ConfigurationErrorsException("Bad configure " + MONITORED_FOLDER_CONFIGURATION_NAME);
            return folder;
        }

        private int GetConfiguredInterval()
        {
            int seconds = 0;

            string interval = GetConfigurationValue(CHECK_INTERVAL_CONFIGURATION_NAME);
            int.TryParse(interval, out seconds);

            return seconds;
        }

        private string GetConfigurationValue(string configurationName)
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(configurationName))
                throw new ConfigurationErrorsException("Bad configure " + configurationName);

            return ConfigurationManager.AppSettings[configurationName];
        }

    }
}
