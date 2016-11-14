using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using FolderMonitor.Common.Plugin;
using FolderMonitor.Common.Services;

namespace FolderMonitor
{
    public class FolderMonitorService : IFolderMonitorService
    {

        private string _monitoredPath = String.Empty;
        private int _checkInterval = 5;
        IEnumerable<IFileLoader> _handlers;

        List<string> _oldFiles = new List<string>();
        Timer _checkIntervalTimer = new Timer();

        public FolderMonitorService(string monitoredPath, int intervalInSec)
        {
            _monitoredPath = monitoredPath;
            _checkInterval = intervalInSec;
        }

        public void StartMonitor(IEnumerable<IFileLoader> handlers)
        {
            if (String.IsNullOrEmpty(_monitoredPath) || !Directory.Exists(_monitoredPath))
                return;

            _handlers = handlers;

            _oldFiles.AddRange(Directory.GetFiles(_monitoredPath));

            _checkIntervalTimer.Interval = _checkInterval * 1000;
            _checkIntervalTimer.Elapsed += _checkIntervalTimer_Elapsed;
            _checkIntervalTimer.Start();

        }

        void _checkIntervalTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CheckFolder();
        }
        
        private void CheckFolder()
        {
            List<string> newFiles = new List<string>();
            newFiles.AddRange(HasNewFiles(_monitoredPath, _oldFiles));

            var exceptions = new ConcurrentQueue<Exception>();

            foreach (string currentFile in newFiles)
            {
             Task<UserControl>.Factory.StartNew(
                    () =>
                    {
                        try
                        {
                            var lines = new BlockingCollection<string>();
                            using (var reader = new StreamReader(currentFile))
                            {
                                string line;

                                while ((line = reader.ReadLine()) != null)
                                {
                                    lines.Add(line);
                                }
                            }

                            return Process(currentFile, lines.GetConsumingEnumerable());
                        }
                        catch (Exception e) { exceptions.Enqueue(e); }
                        return null;
                    }).ContinueWith((t) =>
                       {
                           _oldFiles.Add(currentFile);
                           wResult res = new wResult();
                           var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
                           stackPanel.Children.Add(t.Result);
                           stackPanel.Children.Add(new Button { Content = "Button" });
                           res.Content = stackPanel;
                           res.Show();
                       }
                    );
            }


            if (exceptions.Count > 0) throw new AggregateException(exceptions);


        }

        private UserControl Process(string fileName, IEnumerable<string> fileContent)
        {
            string fileExtension = fileName.Substring(fileName.LastIndexOf(".") + 1);

            foreach (IFileLoader loader in _handlers)
            {
                if (loader.ExtensionHandled == fileExtension)
                    return loader.LoadFileData(fileContent);
            }

            return null;
        }

        private IEnumerable<string> HasNewFiles(string path, List<string> lastKnownFiles)
        {
            return from f in Directory.GetFiles(path)
                   where !lastKnownFiles.Contains(f)
                   select f;
        }

        public void StopMonitor()
        {
            _checkIntervalTimer.Stop();
        }
    }
}
