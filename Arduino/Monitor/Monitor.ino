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
  
  // For demo reasons setting some port in order to be able to read their function and status in the Windows application.
  // replace with anything you need for your application.
  //Can't use 0 and 1 (used for serial communication)
  pinMode(2, INPUT);           // set pin to input
  digitalWrite(2, LOW);       // turn on pullup resistor
  pinMode(3, INPUT);           // set pin to input
  digitalWrite(3, LOW);       // turn off pullup resistor  
  pinMode(8, OUTPUT);         // set pin to output
  digitalWrite(8, HIGH);       // turn on 
  pinMode(9, OUTPUT);         // set pin to output
  digitalWrite(9, LOW);       // turn on 

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
              Udp.write("Ack:b");
              break;
    case 'c':     //Get Arduino Configuration
              Serial.println("Request Configuration Request");
              Udp.write(DDRD);  //Data direction Register for pins 0..7
              Udp.write(DDRB);  //Data direction Register for pins 8..13
              Udp.write(DDRC);  //Data direction Register for pins 14 (A0)..
              
              Udp.write(PIND);  //Input configuration for pins 0..7
              Udp.write(PINB);  //Input configuration for pins 8..13
              Udp.write(PINC);  //Data direction Register for pins 8..13
              break;      
    case 's':     //Get Status for IO
              Serial.println("Status Request"); // @see http://www.arduino.cc/en/Reference/PortManipulation
              Udp.write(PORTD);  //Write status of digital pins 0..7
              Udp.write(PORTB);  //Write status of digital pins 8..13
              Udp.write(PORTC);  //Write status of digital pins 14 (A0)..
              break;           
    default: 
              Serial.println("Unknown Request");
              Udp.write("Ack:?");
              break;
    } 
    Udp.endPacket();  
}


