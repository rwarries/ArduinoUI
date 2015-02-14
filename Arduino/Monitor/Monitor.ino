/*
 This sketch answers to UDP request for Arduino status and responds with the desired info.
 It can be used as an alternative to the regular Serial class and has the built-in ability to monitor IO
 For testing connectivity it can repond to an alive request.
*/
 

#include <SPI.h>
#include <Ethernet.h>

// Enter a MAC address and IP address for your controller below.
// The IP address will be dependent on your local network:
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
IPAddress ip(192, 168, 0, 177); // <== enter your Arduino ip (With DHCP is possible but then you still need to know the adress for the winsows client

unsigned int localPort = 8888;      // local port to listen on

// buffers for receiving and sending data
char packetBuffer[UDP_TX_PACKET_MAX_SIZE]; //buffer to hold incoming packet,

// An EthernetUDP instance to let us send and receive packets over UDP
EthernetUDP Udp;

void setup() {
  // start the Ethernet and UDP:
  Ethernet.begin(mac,ip);
  Udp.begin(localPort);
  Serial.begin(9600);
}

void loop() {
  // if there's data available, read a packet
  int size = Udp.parsePacket();
  Serial.print(".");
  if(size>0)
  {
    IPAddress remote = Udp.remoteIP();
 
    for (int octetNr = 0; octetNr <= 2; octetNr++)
    {
      Serial.print(remote[octetNr], DEC);
      Serial.print(".");
    }
    Serial.print(remote[3], DEC); //Last octet outside loop (I don't want the extra '.')
    Serial.print(":");
    Serial.println(Udp.remotePort());

    // Read the payload itself...
    Udp.read(packetBuffer,UDP_TX_PACKET_MAX_SIZE);
    handleDatagram(); //FIXME not ver elegant to be fiddling with globals
  }
  delay(1000);  //All other work that needs to be done goes instead of this delay...
}

void handleDatagram(){
  // Parse datagram...
  // First byte is the request
  // Rest is dependent on the type of request
  
  Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());  //Start teh response
  
  switch (packetBuffer[0]) {
    case 'a':   // alive request
              Serial.println("Alive Request");
              Udp.write("Ack:a"); // send a reply, to the IP address and port that sent us the packet we received 
              break;
    case 'b':
              Serial.println("Type B Request");
              Udp.write("Ack:a");
              break;
    case 'c':
              Serial.println("Request Configuration Request");
              Udp.write("Ack:c... Configuration is Blah");
              break;      
    case 's':
              Serial.println("Status Request");
              Udp.write("Ack:s... Status is...");
              break;           
    default: 
              Serial.println("Unknown Request");
              Udp.write("Ack:?");
              break;
    } 
    Udp.endPacket();  
}


