using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ExtensionMethods;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , INotifyPropertyChanged
    {
      //private static PropertyMetadata pm = new PropertyMetadata(new PropertyChangedCallback(OnIpChanged));
      public static readonly DependencyProperty IPAddressProperty = DependencyProperty.Register("IPAddress", typeof(string),
                                                                    typeof(MainWindow), new UIPropertyMetadata("192.168.0.177", OnIpChanged));
       
      private static UDPTransciever _u = new UDPTransciever(8888);
      private static CancellationTokenSource _ctsAlive;
      private static CancellationTokenSource _ctsPing;
      private static CancellationTokenSource _cts;

      private static int _interval = 1;
      private static Boolean _isPingActive;
      private static Boolean _isAliveActive;
      private static Boolean _isFetchingActive;
      private static string _pingResult = "not run";

      private static RingBuffer<byte> _receiveBuffer = new RingBuffer<byte>(2000);
      private static RingBuffer<byte> _sentBuffer = new RingBuffer<byte>(2000);
      private static string _sendString = "";

      public RingBuffer<byte> ReceiveBuffer {
          get { return _receiveBuffer; }
          set { 
              _receiveBuffer = value;
              if (PropertyChanged != null){
                 NotifyChange(new PropertyChangedEventArgs("ReceiveBuffer"));
              }
          }
      }

      public RingBuffer<byte> SentBuffer
      {
          get { return _sentBuffer; }
          set
          {
              _sentBuffer = value;
              NotifyChange(new PropertyChangedEventArgs("SentBuffer"));
          }
      }

      public string SendString {
          get { return _sendString; }
          set
          {
              _sendString = value;
              NotifyChange(new PropertyChangedEventArgs("SendString"));
          }
      }

      public string PingResult
      {
          get { return _pingResult; }
          set
          {
              _pingResult = value;
              NotifyChange(new PropertyChangedEventArgs("PingResult"));
          }
      }

      private void NotifyChange(PropertyChangedEventArgs e)
      {
          if (PropertyChanged != null)
              PropertyChanged(this, e);
      }

      public int Port
      {
          get { return _u.RemotePort; }
          set { _u.RemotePort = value; }
      }

      public int Interval
      {
          get { return _interval; }
          set { 
              _interval = value;
          }
      }

        public Boolean IsFetchingIsActive
        {
            get { return _isFetchingActive; }
            set
            {
                _isFetchingActive = value;
                if (_isFetchingActive)
                {
                    _cts = new CancellationTokenSource();
                    Repeat.Interval(TimeSpan.FromSeconds(_interval), () => changeValues(), _cts.Token);
                }
                else
                {
                    if (_cts != null)
                    {
                        _cts.Cancel();
                    }
                }
            }
        }

        public Boolean IsAliveActive
        {
            get { return _isAliveActive; }
            set
            {
                _isAliveActive = value;
                if (_isAliveActive)
                {
                    _ctsAlive = new CancellationTokenSource();
                    Repeat.Interval(TimeSpan.FromSeconds(_interval), () => checkAlive(), _ctsAlive.Token);
                }
                else
                {
                    if (_ctsAlive != null)
                    {
                        _ctsAlive.Cancel();
                    }
                }
            }
        }

        public Boolean IsPingActive
        {
            get { return _isPingActive; }
            set
            {
                _isPingActive = value;
                if (_isPingActive)
                {
                    _ctsPing = new CancellationTokenSource();
                    Repeat.Interval(TimeSpan.FromSeconds(_interval), () => checkPing(), _ctsPing.Token);
                }
                else
                {
                    if (_ctsPing != null)
                    {
                        _ctsPing.Cancel();
                    }
                    //FIXME want to handle this in the view only
                    PingResult = "unknown";
                }
            }
        }

        // Callback for dependency property
        private static void OnIpChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           _u.RemoteIPAddress = (string) e.NewValue;
        }

        public string IPAddress
        {
            get { return (string)GetValue(IPAddressProperty); }
            set { 
                SetValue(IPAddressProperty, value);
                _u.RemoteIPAddress = value;
            }
        }

        ObservableCollection<InputOutput> portC = null;
        public MainWindow()
        {
            InitializeComponent();
            portC = new ObservableCollection<InputOutput>();
            portC.CollectionChanged += portC_CollectionChanged;

            portC.AddRange(new List<InputOutput>{ 
                new InputOutput() { Pin = 0 , Name = "D0/RX"},
                new InputOutput() { Pin = 1 , Name = "D1/TX"},
                new InputOutput() { Pin = 2,  Name = "D2" },
                new InputOutput() { Pin = 3,  Name = "D3" },
                new InputOutput() { Pin = 4,  Name = "D4" },
                new InputOutput() { Pin = 5,  Name = "D5/PWM"},
                new InputOutput() { Pin = 6,  Name = "D6/PWM" },
                new InputOutput() { Pin = 7,  Name = "D7" },
                new InputOutput() { Pin = 8,  Name = "B0" },                                          
                new InputOutput() { Pin = 9,  Name = "B1/PWM"},
                new InputOutput() { Pin = 10, Name = "B2/PWM" },
                new InputOutput() { Pin = 11, Name = "B3/PWM / MOSI (SPI)" },
                new InputOutput() { Pin = 12, Name = "B4/MISO (SPI)" },
                new InputOutput() { Pin = 13, Name = "B5/SCK (SPI)" },    //B6 and B7 not available (crystal)
                new InputOutput() { Pin = 14, Name = "C0/Analog0" }, 
                new InputOutput() { Pin = 15, Name = "C1/Analog1" }, 
                new InputOutput() { Pin = 16, Name = "C2/Analog2" }, 
                new InputOutput() { Pin = 17, Name = "C3/Analog3" }, 
                new InputOutput() { Pin = 18, Name = "C4/Analog4/SDA (I2C)" }, 
                new InputOutput() { Pin = 19, Name = "C5/Analog5/SCL (I2C)" }, 
                new InputOutput() { Pin = 20, Name = "RESET"}, 

            });
            icInputOutputList.ItemsSource = portC;
            _u.RemoteIPAddress = (string) GetValue(IPAddressProperty);
        }

        private void portC_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (InputOutput io in e.NewItems)
                    io.PropertyChanged += InputOutput_PropertyChanged;

            if (e.OldItems != null)
                foreach (InputOutput io in e.OldItems)
                    io.PropertyChanged -= InputOutput_PropertyChanged;
        }

        void InputOutput_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsHigh")
            {
                //MessageBox.Show("IO");
            }
        }

        private void changeValues()
        {
            Byte[] message = {(byte) 's'};
            byte[] result = _u.Trancieve(message);
            handleResult(result);
            foreach (InputOutput io in portC)
            {
                io.IsHigh = GetBit(io.Pin,result);
            }
        }

        private bool GetBit(int pin, byte[] message)
        {
            byte b = message[pin / 8];
            return ((b & (1 << (pin % 8))) != 0);
        }

        private void checkAlive()
        {
            Byte[] message = { (byte)'a' };
            byte[] result = _u.Trancieve(message);
            handleResult(result);
        }

        private void checkPing()
        {
            PingResult = _u.Ping();
        }

        private void handleResult(byte[] result)
        {
            if (result != null)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    ReceiveBuffer.Add(result[i]);
                }
                if (PropertyChanged != null)
                {
                    NotifyChange(new PropertyChangedEventArgs("ReceiveBuffer"));
                }
            }
        }

        // see http://www.daedtech.com/wpf-and-notifying-property-change
        // It is used to make sure that the TextBlocks that display messages get updated
        public event PropertyChangedEventHandler PropertyChanged;

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (SendString.Length > 0)
            {
                byte[] bytesToSend = Encoding.ASCII.GetBytes(SendString);
                handleResult(_u.Trancieve(bytesToSend));
                for (int i=0; i<SendString.Length;i++){
                    _sentBuffer.Add(bytesToSend[i]);
                }
                SendString = "";
                NotifyChange(new PropertyChangedEventArgs("SentBuffer"));             
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            SentBuffer.Clear();
            NotifyChange(new PropertyChangedEventArgs("SentBuffer"));

            ReceiveBuffer.Clear();
            NotifyChange(new PropertyChangedEventArgs("ReceiveBuffer"));
        }

        private void ButtonGetConfig_Click(object sender, RoutedEventArgs e)
        {
            Byte[] message = { (byte)'c' };
            Byte[] result = _u.Trancieve(message);
            // 6 Bytes are received:
            //0:DDRD in DDR. a '0' means input. A '1' indicates it indicates an output
            //1:DDRB
            //2:DDRC
            //3:PIND
            //4:PINB
            //5:PINC
            handleResult(result);
            foreach (InputOutput io in portC)
            {
                Debug.Write("Pin " + io.Pin + " has mode " + GetBit(io.Pin, result));
                io.Mode = GetBit(io.Pin, result);
            }
        }

    }
}