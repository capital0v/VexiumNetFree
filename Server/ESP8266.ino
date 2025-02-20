#include <ESP8266WiFi.h>

const char* ssid = "WIFI_NAME";
const char* password = "WIFI_PASSWORD";
const char* host = "LOCAL_IP";
const uint16_t port = 8080;

void setup() 
{
  Serial.begin(9600);
  delay(10);
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(500);
    Serial.print(".");
  }
}

void loop() 
{
  WiFiClient client;
  if (!client.connect(host, port)) 
  {
    delay(1000);
    return;
  }

  client.print("GLOW");

  delay(100);
}