#include <Wire.h>
#include "MMA7660.h"
#include "rgb_lcd.h"

#include <WiFi.h>
#include <WiFiUdp.h>
#include <OSCMessage.h>

// For the communication with the computer
const int oscPort = 7001;
const int outPort = 7002;
const IPAddress outIp(172,20,10,4);

WiFiUDP myUDP;
const char* ssid = "sguptiPhone";
const char* password = "sgupt2435";

// Instantiate accelerometer abd lcd disp
MMA7660 accelemeter;
rgb_lcd lcd;
// Instantiate Base Color Screen
const int colorR = 255;
const int colorG = 255;
const int colorB = 255;

void setup() {
    accelemeter.init();
    Serial.begin(9600);
    // set up the LCD's number of columns and rows:
    lcd.begin(16, 2);
    lcd.setRGB(colorR, colorG, colorB);
    // define LED
    pinMode(2, OUTPUT);
    pinMode(3, INPUT);

    WiFi.begin(ssid, password);

    while (WiFi.status() != WL_CONNECTED){
       delay(100); // do nothing
       Serial.print(".");
    }
    Serial.println("Connected!");
    delay(1000);
    Serial.println(WiFi.localIP());
    myUDP.begin(oscPort);
    lcd.setCursor(0, 0);
        lcd.print("                ");
        lcd.setCursor(0, 1);
        lcd.print("                ");
    }


void loop() {
  OSCMessage OSCMessageIn;
  char toPrint[33] = "";
  String line1;
  String line2;
  int udpPacketSize = myUDP.parsePacket();
  bool playerResponse = false;
  bool playerResponseReceived = false;

    if(udpPacketSize > 0)
    {
      while(udpPacketSize--)
      {
        // Read the packet to the OSCMessage object
        OSCMessageIn.fill(myUDP.read());
      }
      // Handle the incoming data based on its OSC formatting
      if(!OSCMessageIn.hasError())
      {
        OSCMessageIn.getString(0, toPrint, 33);
        toPrint[32] = '\0';
        Serial.println(toPrint);
        line1 = String(toPrint).substring(0, 16);
        line2 = String(toPrint+16).substring(0, 16);
        Serial.println(line1);
        Serial.println(line2);
        lcd.setCursor(0, 0);
        lcd.print("                ");
        lcd.setCursor(0, 1);
        lcd.print("                ");
        lcd.setCursor(0, 0);
        lcd.print(line1);
        lcd.setCursor(0, 1);
        lcd.print(line2);
        // we have now printed out the message form the game. we want to wait for the
        // player to respond
        OSCMessage msg("/booleans");
        while (playerResponseReceived == false){
          // the player can respond 1 of 2 ways
          float ax, ay, az;
          accelemeter.getAcceleration(&ax, &ay, &az);
          if (ay < -0.5){
            playerResponse = true;
            playerResponseReceived = true;
            msg.empty();
            msg.add(playerResponse);
            myUDP.beginPacket(outIp, outPort);
            msg.send(myUDP);
            myUDP.endPacket();
            msg.empty();
            Serial.println("Sending true.");
            delay(500);
          }
          else if (digitalRead(3) == HIGH){
            playerResponse = false;
            playerResponseReceived = true;
            msg.empty();
            msg.add(playerResponse);
            myUDP.beginPacket(outIp, outPort);
            msg.send(myUDP);
            myUDP.endPacket();
            msg.empty();
            Serial.println("Sending false.");
            delay(500);
          }
        }
      }
      else
      {
        OSCErrorCode oscError = OSCMessageIn.getError();
        Serial.print("Error: ");
        Serial.println(oscError);
      }
    }

  delay(500);

}
