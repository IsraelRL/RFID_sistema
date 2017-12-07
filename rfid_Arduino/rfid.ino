//librerias
#include <SPI.h>
#include <MFRC522.h>

#define RST_PIN  9    //Pin 9 para el reset del RC522
#define SS_PIN  10   //Pin 10 para el SS (SDA) del RC522
MFRC522 mfrc522(SS_PIN, RST_PIN); ///Creamos el objeto para el RC522


//pines de los actuadores
const int ledPin =  4;
const int ledPin1 =  3;
const int puerta = 5;


void setup() {
  Serial.begin(250000); //Iniciamos La comunicacion serial
  SPI.begin();        //Iniciamos el Bus SPI
  mfrc522.PCD_Init(); // Iniciamos el MFRC522

  //declaramos el tipo de pin E/S
  pinMode(ledPin, OUTPUT);
  pinMode(ledPin1, OUTPUT);
  pinMode(puerta, OUTPUT);
}


String ActualUID=""; //almacenará el código del Tag leído

//String Usuario1 = "4232191017387129"; código del usuario 1
//String Usuario2 = "439731017387129" ; código del usuario 2

void loop() {
  // Revisamos si hay nuevas tarjetas  presentes
  if ( mfrc522.PICC_IsNewCardPresent()) 
        {  
      //Seleccionamos una tarjeta
            if ( mfrc522.PICC_ReadCardSerial()) 
            {
                  // Enviamos serialemente su UID
                  for (byte i = 0; i < mfrc522.uid.size; i++) {
                          ActualUID += mfrc522.uid.uidByte[i];
                  } 

                  //Mandamos por puerto serial, al programa en c# el numero de la tarjeta
                  Serial.println(ActualUID);
                  
                  //leemos el resultado del programa en c#
                  //si es "1" prende y apaga un led, al terminar de parpadear, abre la puerta
                  if(Serial.readString() == "1"){
                    //Serial.println("Acceso concedido...");
                      digitalWrite(ledPin, HIGH);
                      delay(200);
                      digitalWrite(ledPin, LOW);
                      delay(200);
                      digitalWrite(ledPin, HIGH);
                      delay(200);
                      digitalWrite(ledPin, LOW);
                      delay(200);
                      digitalWrite(ledPin, HIGH);
                      digitalWrite(puerta, HIGH);
                      delay(4000);
                      digitalWrite(puerta, LOW);
                      digitalWrite(ledPin, LOW);
                      }
                      //si es diferente a "1" prende y apaga otro led
                      else
                      {
                       
                        digitalWrite(ledPin1, HIGH);
                        delay(200);
                        digitalWrite(ledPin1, LOW);
                        delay(200);
                        digitalWrite(ledPin1, HIGH);
                        delay(200);
                        digitalWrite(ledPin1, LOW);
                        
                      }
                  // Terminamos la lectura de la tarjeta tarjeta  actual
                  mfrc522.PICC_HaltA();
          
            }
      
  }
  //reiniciamos la variable
  ActualUID="";
}
