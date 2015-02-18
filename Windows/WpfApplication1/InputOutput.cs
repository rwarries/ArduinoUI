using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class InputOutput : INotifyPropertyChanged
    {
        private string _name;
        private int _pinNumber;
        private Boolean? _mode;
        private Boolean _isHigh;

        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;

        public InputOutput()
        {
            _name = "";
            _pinNumber = -1;
            _mode = null;
            _isHigh = false;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Pin
        {
            get { return _pinNumber; }
            set { _pinNumber = value; }
        }
        
        
        public Boolean? Mode
        {
            get { return _mode; }
            set { 
                _mode = value;
                OnPropertyChanged("Mode");
            }
        }

        public Boolean IsHigh
        { 
            get { return _isHigh; }
            set { 
                _isHigh = value;
                OnPropertyChanged("IsHigh");  
            }

        }

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
