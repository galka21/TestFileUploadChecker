using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderMonitor.Common.Data
{
    public class FileEntry
    {    
        private DateTime _theDate;
        private double _open;
        private double _high;
        private double _low;
        private double _close;
        private string _volume;

        public string Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        public double Close
        {
          get { return _close; }
          set { _close = value; }
        }

        public double Low
        {
          get { return _low; }
          set { _low = value; }
        }

        public double High
        {
          get { return _high; }
          set { _high = value; }
        }

        public double Open
        {
          get { return _open; }
          set { _open = value; }
        }

        public DateTime TheDate
        {
          get { return _theDate; }
          set { _theDate = value; }
        }

        public FileEntry(DateTime theDate, double open, double high, double low, double close, string volume)
        {
            _theDate = theDate;
            _open = open;
            _high = high;
            _low = low;
            _close = close;
            _volume = volume;
        }
        public FileEntry(){}
    }
}
