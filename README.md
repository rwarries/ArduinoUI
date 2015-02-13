# ArduinoUI
A C# WPF UI that will visualise Arduino Uno state (assuming you have an ethernetshield) using UDP communication

It is in its early stage of development. I had many parts working separately and I'm now slowly bringing them together.

The idea is to end with a light-weight framework for monitoring an arduino over ethernet using the UDP protocol.

It will require:
1) An Arduino (will test for Uno only) + Ethernet shield (CODE NOT YET IN REPO)
2) A windows computer running the WPF based Front-end

Roadmap:

0) Lifesign send/receive
1) Monitor (polling only) digital in/out
2) ditto for Analog
3) Customisation + persistence of UI settings
4) Event based (iso polling) data acquisition
5) IO graph (make it a low-end digital analyser)

....(anything else that pops up along the route)
