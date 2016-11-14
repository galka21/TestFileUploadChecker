using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FolderMonitor.Common.Data;
using FolderMonitor.Common.Plugin;

namespace XMLFileLoader
{
    public class XMLFileLoader : IFileLoader
    {
        private string _dateFormat = "yyyy-M-d";

        #region Properties
        public string Name
        {
            get { return "XMLFileLoader"; }
        }

        public string Description
        {
            get { return "Loads entries from an XML file"; }
        }

        public string Version
        {
            get { return "0.0.1"; }
        }

        public string ExtensionHandled
        {
            get { return "xml"; }
        }

        public string DateParseFormat
        {
            get
            {
                return _dateFormat;
            }
            set
            {
                _dateFormat = value;
            }
        }
        #endregion

        public System.Windows.Controls.UserControl LoadFileData(IEnumerable<string> fileContent)
        {
            List<FileEntry> loadedEntries = new List<FileEntry>();

             Stream stream = null;
            try
            {
                loadedEntries.AddRange(ProcessFile(fileContent));
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            ucDisplayFileContent wind = new ucDisplayFileContent();
            wind.FileEntries = loadedEntries;

            return wind;
        }

        private List<FileEntry> ProcessFile(IEnumerable<string> fileContent)
        {
            List<FileEntry> loadedEntries = new List<FileEntry>();

            var xmldocument = new XElement("values",
                    fileContent.Select(t => new XElement("value", t)));

            loadedEntries = (from entries in xmldocument.Element("values").Elements("value")
                             select new FileEntry
                                    {
                                        TheDate = DateTime.ParseExact(entries.Attribute("date").Value, _dateFormat, 
                                                                            System.Globalization.CultureInfo.InvariantCulture,
                                                                            DateTimeStyles.None),
                                        Open = double.Parse(entries.Attribute("open").Value),
                                        High = double.Parse(entries.Attribute("high").Value),
                                        Low = double.Parse(entries.Attribute("low").Value),
                                        Close = double.Parse(entries.Attribute("close").Value),
                                        Volume = entries.Attribute("volume").Value
                                    }).ToList();

            return loadedEntries;
        }

        private FileEntry CreateNewEntry(string line)
        {
            FileEntry entry = null;

            string[] values = line.Split(',');

            if (values.Length < 6)
                throw new Exception(line + ", line has incorrect format : " + line);

            DateTime theDate = DateTime.MinValue;
            if (!DateTime.TryParseExact(values[0], _dateFormat, System.Globalization.CultureInfo.InvariantCulture,
                                    DateTimeStyles.None, out theDate))
                throw new Exception(line + ", line has incorrect format : " + line);

            double open;
            if (!Double.TryParse(values[1], out open))
                throw new Exception(line + ", line has incorrect format : " + line + " - " + values[1]);
            double high;
            if (!Double.TryParse(values[2], out high))
                throw new Exception(line + ", line has incorrect format : " + line + " - " + values[2]);
            double low;
            if (!Double.TryParse(values[3], out low))
                throw new Exception(line + ", line has incorrect format : " + line + " - " + values[3]);
            double close;
            if (!Double.TryParse(values[4], out close))
                throw new Exception(line + ", line has incorrect format : " + line + " - " + values[4]);

            entry = new FileEntry(theDate, open, high, low, close, values[5]);

            return entry;
        }
    }
}
