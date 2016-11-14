using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderMonitor.Common.Data;
using System.Windows.Controls;

namespace FolderMonitor.Common.Plugin
{
    public interface IFileLoader
    {
        string Name { get; }
        string Description { get; }
        string Version { get; }
        string ExtensionHandled { get; }
        string DateParseFormat { get; set; }

        UserControl LoadFileData(IEnumerable<string> fileContent);

    }
}
