using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArepaRunner
{
    public class TextOutput : INotifyPropertyChanged
    {
        private string outputString;

        public string OutputString
        {
            get
            {
                if (outputString == null)
                {
                    return string.Empty;
                }
                return outputString;
            }
            set
            {
                outputString = value;
                Changed("OutputString");
            }
        }

        public virtual void Changed(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
