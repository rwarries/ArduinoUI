using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class UDPTransciever
    // Trancieving = Tansmitting and Receiving
    {

        public string RemoteIPAddress
        {
            get { return _remoteIP.ToString(); }
            set { _remoteIP = IPAddress.Parse(value); }  // I can trust the front-end to have validated it....(I hope)
        }

        public int RemotePort
        {
            get { return _remotePort; }
            set { _remotePort = value; }  // I can trust the front-end to have validated it....(I hope)
        }


        private UdpClient _udpClient;
        private IPAddress _remoteIP;
        private int _remotePort;

        Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there?");


        public UDPTransciever()
        {

            this._remotePort = 8888;
        }

        public UDPTransciever(int remotePort)
        {
            _udpClient = new UdpClient(0);
            this._remotePort = remotePort;
        }

        public string Trancieve(string message) // ascii in and out
        {
            // Sends a message to the host to which you have connected.
            Byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            return Encoding.ASCII.GetString(this.Trancieve(sendBytes));
        }

        public byte[] Trancieve(byte[] message)
        {
            byte[] returnValue;

            try
            {
                _udpClient = new UdpClient(0); // Zero, let OS stack choose an available local port
                _udpClient.Connect(_remoteIP, _remotePort);

                _udpClient.Send(message, message.Length);

                //IPEndPoint object will allow us to read datagrams sent from any source.
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = _udpClient.Receive(ref RemoteIpEndPoint);


                // Uses the IPEndPoint object to determine which host responded.
                //Console.WriteLine("This is the message you received:\"" +
                //                             returnData.ToString() + "\"");
                //Console.WriteLine("This message was sent from " +
                //                            RemoteIpEndPoint.Address.ToString() +
                //                            " on their port number :" +
                //                            RemoteIpEndPoint.Port.ToString());


                returnValue = receiveBytes;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                returnValue = null;
            }
            finally
            {
                _udpClient.Close();
            }
            return returnValue;
        }

        public string Ping()
        {
            Ping pingSender = new Ping();
            IPAddress address = this._remoteIP;
            PingReply reply = pingSender.Send(address);

            if (reply.Status == IPStatus.Success)
            {
                return reply.RoundtripTime.ToString();
            }
            else
            {
                return reply.Status.ToString();
            }
        }
    }
}
