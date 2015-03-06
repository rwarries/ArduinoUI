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
        private int _changed; //Indicates how long ago this was changed in # polls ago
        private RingBuffer<Boolean> _history;

        private static Random _rnd = new Random();

        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;

        public InputOutput()
        {
            _name = "";
            _pinNumber = -1;
            _mode = null;
            _isHigh = false;
            _changed = 0;
            _history = new RingBuffer<Boolean>(1000); // FIXME Maybe a deent value an be calculated?
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
                if (value != _isHigh)
                {
                    _isHigh = value;
                    _changed = 0; // start counting again
                    OnPropertyChanged("IsHigh");
                }
            }
        }

        public int Changed
        {
            get { return _changed; }
            set { _changed = value;
                    OnPropertyChanged("Changed");
            }
        }

        public RingBuffer<Boolean> History
        {
            get { return _history; }
            set {
                _history = value;
                OnPropertyChanged("History");
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
