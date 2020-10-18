/*
 * センサ関連の処理をまとめたタブ
 * 何らかのセンサから値を一つ読み取りグローバル変数であるsensorValueに代入する
 */

// 地球の重力である1Gの加速度(m/s^2)
float ms2 = 9.80665;
// 電源電圧5V時のオフセット電圧(0G=2.5V=2500mv)
float offset_voltage = 2500.0;

// 外部から呼び出す関数
// 何らかのセンサ値を読み取り、sensorValueに代入する
// 内部処理自体は外部からは意識せず任意のセンサ値読み出し関数を変えることで機能する
void readSensorValue() {
  // 現状は3軸加速度センサからの値を読み出すように設定されている
  read3AxisAcceleration();

  // 下記のコメントを外すと読み取るセンサを変えることができる
  // readTemparature();
}

// 3軸加速度センサの値を読み取る
// [型番]946H-1C-3D
// [ピン]A0: x軸, A1: y軸, A2: z軸
void read3AxisAcceleration() {
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
  float xyz[3];
  xyz[0] = xg * ms2;
  xyz[1] = yg * ms2;
  xyz[2] = zg * ms2;
  Serial.println("X* " + String(xyz[0]) + " Y: " + String(xyz[1]) + " Z: " + String(xyz[2]));
  
  // X軸加速度をセンサ値として代入する
  sensorValue = xyz[0];
}

// 温度センサの値を読み取る
void readTemparature() {
  // TODO: 実装予定
  sensorValue = 0;
}
