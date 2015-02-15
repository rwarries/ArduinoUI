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
      private static CancellationTokenSource _cts;
      private static int _interval = 1;  //Fixme lob interval for now
      private static Boolean _isFetchingActive;
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
              if (PropertyChanged != null)
              {
                  NotifyChange(new PropertyChangedEventArgs("SentBuffer"));
              }
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
                new InputOutput() { Pin = 0 , Name = "RX" , Mode = ModeEnum.RESERVED}, 
                new InputOutput() { Pin = 1 , Name = "TX" , Mode = ModeEnum.RESERVED},
                new InputOutput() { Pin = 2 },
                new InputOutput() { Pin = 3 },
                new InputOutput() { Pin = 4 },
                new InputOutput() { Pin = 5 },
                new InputOutput() { Pin = 6 },
                new InputOutput() { Pin = 7 },
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
            /*
            System.Random rg = new Random();
            foreach (InputOutput io in portC)
            {
                io.IsHigh = (rg.NextDouble() > 0.5);
            }
             */
            Byte[] message = {(byte) 'a'};
            byte[] result = _u.Trancieve(message);
            //SentBuffer = SentBuffer + message;
            if (result != null)
            {
                for (int i = 0; i < message.Length; i++)
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
                for (int i=0; i<SendString.Length;i++){
                    _sentBuffer.Add(bytesToSend[i]);
                }
                SendString = "";
                NotifyChange(new PropertyChangedEventArgs("SentBuffer"));             
            }
        }

    }
}


