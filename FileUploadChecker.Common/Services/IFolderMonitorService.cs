using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderMonitor.Common.Plugin;

namespace FolderMonitor.Common.Services
{
    public interface IFolderMonitorService
    {
        void StartMonitor(IEnumerable<IFileLoader> handlers);

        void StopMonitor();
    }
}
