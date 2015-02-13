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
    public partial class MainWindow : Window
    {
      //private static PropertyMetadata pm = new PropertyMetadata(new PropertyChangedCallback(OnIpChanged));
      public static readonly DependencyProperty IPAddressProperty = DependencyProperty.Register("IPAddress", typeof(string),
                                                                    typeof(MainWindow), new UIPropertyMetadata("192.168.0.177", OnIpChanged));
      

      
      private static UDPTransciever _u = new UDPTransciever(8888);
      private static CancellationTokenSource _cts;
      private static int _interval = 5;

      private static Boolean _isFetchingActive;

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

            //var cancellationTokenSource = new CancellationTokenSource();
            //var task = Repeat.Interval(TimeSpan.FromSeconds(3), () => changeValues(), cancellationTokenSource.Token);
            //cancellationTokenSource.CancelAfter(60000); //Stop changing after a minute
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


        // Simulate changing of values... causing the property to fire an event.
        /*private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            changeValues();
        } */

        private void changeValues()
        {
            System.Random rg = new Random();
            foreach (InputOutput io in portC)
            {
                io.IsHigh = (rg.NextDouble() > 0.5);
            }
        }
    }
}


