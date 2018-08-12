#include <MPU6050_tockn.h>
#include <Wire.h>

MPU6050 mpu6050(Wire);

// Flex sensors to track finger bends
const int flexSensorPin = A0; // The flex sensor for pin 0
const int flexSensorPin2 = A1; // The flex sensor for pin 1
const int flexSensorPin3 = A2; // The flex sensor for pin 2
const int flexSensorPin4 = A3; // The flex sensor for pin 3
const int flexSensorPin5 = A4; // The flex sensor for pin 4

// Joystick + button to move the virtual hand - Could easily be replaced with Vive Tracker
const int xPin = A5;
const int yPin = A6;
const int buttonPin = 3; // joystick button press - up on z axis
const int buttonPin2 = 4; // button press - down on z axis

// Vibration motors to simulate haptics
const int indexMotorPin = 12;
const int middleMotorPin = 11;
const int ringMotorPin = 10;
const int pinkyMotorPin = 9;
const int palmMotorPin = 8;
const int palmMotorPin2 = 7;
const int palmMotorPin3 = 6;

void setup() {
  Serial.begin(9600);
  Wire.begin();
  mpu6050.begin();
  mpu6050.calcGyroOffsets(true); // Calibrate the gyro offsets
  
  pinMode(flexSensorPin, INPUT);
  pinMode(flexSensorPin2, INPUT);
  pinMode(flexSensorPin3, INPUT);
  pinMode(flexSensorPin4, INPUT);
  pinMode(flexSensorPin5, INPUT);

  pinMode(xPin, INPUT);
  pinMode(yPin, INPUT);
  pinMode(buttonPin, INPUT_PULLUP);
  pinMode(buttonPin2, INPUT_PULLUP);
  
  pinMode(indexMotorPin, OUTPUT);
  digitalWrite(indexMotorPin, LOW);
  pinMode(middleMotorPin, OUTPUT);
  digitalWrite(middleMotorPin, LOW);
  pinMode(ringMotorPin, OUTPUT);
  digitalWrite(ringMotorPin, LOW);
  pinMode(pinkyMotorPin, OUTPUT);
  digitalWrite(pinkyMotorPin, LOW);
  pinMode(palmMotorPin, OUTPUT);
  digitalWrite(palmMotorPin, LOW);
  pinMode(palmMotorPin2, OUTPUT);
  digitalWrite(palmMotorPin2, LOW);
  pinMode(palmMotorPin3, OUTPUT);
  digitalWrite(palmMotorPin3, LOW);
}

void loop() { 
  mpu6050.update();

  // Output physical inputs to Unity
  Serial.print(analogRead(flexSensorPin)); // A0  
  Serial.print(","); //delimiter
  Serial.print(analogRead(flexSensorPin2)); // A1
  Serial.print(","); //delimiter
  Serial.print(analogRead(flexSensorPin3)); // A2
  Serial.print(","); //delimiter
  Serial.print(analogRead(flexSensorPin4)); // A3
  Serial.print(","); //delimiter
  Serial.print(analogRead(flexSensorPin5)); // A4
  Serial.print(",");
  Serial.print(analogRead(xPin) / 1024.0);
  Serial.print(",");
  Serial.print(analogRead(yPin) / 1024.0);
  Serial.print(",");
  Serial.print(digitalRead(buttonPin) ? (digitalRead(buttonPin2) ? 0.5 : 0) : (digitalRead(buttonPin2) ? 1 : 0.5));
  Serial.print(",");
  Serial.print(mpu6050.getAngleX());
  Serial.print(",");
  Serial.print(mpu6050.getAngleY());
  Serial.print(",");
  Serial.println(mpu6050.getAngleZ());

  // Read vibration types for each finger from Unity
  String message = Serial.readStringUntil('~');
  ChooseVibrationType(indexMotorPin, message[0]);
  ChooseVibrationType(middleMotorPin, message[1]);
  ChooseVibrationType(ringMotorPin, message[2]);
  ChooseVibrationType(pinkyMotorPin, message[3]);
  ChooseVibrationType(palmMotorPin, message[4]);
  ChooseVibrationType(palmMotorPin2, message[4]);
  ChooseVibrationType(palmMotorPin3, message[4]);
}

void ChooseVibrationType(int pinNum, char vibeType) {
  switch(vibeType) {
  case '0': // Off
    digitalWrite(pinNum, LOW);
    break;
  case '1': // On
    digitalWrite(pinNum, HIGH);
    break;
  case '2': // Waterfall
    if(random(0,5) > 3) {
      digitalWrite(pinNum, LOW);
    } else {
      digitalWrite(pinNum, HIGH);
    }
    break;
  }
}
