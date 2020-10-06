// 地球の重力である1Gの加速度(m/s^2)
float ms2 = 9.80665;
// 電源電圧5V時のオフセット電圧(0G=2.5V=2500mv)
float offset_voltage = 2500.0;

void setup() {
  Serial.begin(9600);
}

void loop() {
  long x, y, z;
  x = (analogRead(A0) / 1024.0) * 5.0 * 1000;
  y = (analogRead(A1) / 1024.0) * 5.0 * 1000;
  z = (analogRead(A2) / 1024.0) * 5.0 * 1000;
  x = x - offset_voltage;
  y = y - offset_voltage;
  z = z - offset_voltage;
  float xg = x / 1000.0;
  float yg = y / 1000.0;
  float zg = z / 1000.0;
  Serial.print("X: ");
  Serial.print(xg * ms2);
  Serial.print(" Y: ");
  Serial.print(yg * ms2);
  Serial.print(" Z: ");
  Serial.println(zg * ms2);
  delay(100);
}
