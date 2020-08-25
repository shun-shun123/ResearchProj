int led = 13;
int constant_led = 10;
int cmds[10];


void setup() {
  pinMode(led, OUTPUT);
  Serial.begin(9600);
}

void loop() {
  digitalWrite(constant_led, HIGH);
  // Serialポートに何バイトかデータが到着しているかを返す
  if (Serial.available() <= 0) {
    return;
  }
  // シリアルポートから入力を受け付ける
  String input = Serial.readString();

  // CSV形式のデータからコマンドを読み取る
  int* read_cmds = ReadCommand(input);
  
  // コマンドを実行する
  ExecuteCommand(read_cmds);
  delay(2000);
}

// int[]に変換されたコマンドを実行する
void ExecuteCommand(int* command) {
  // CSV形式の一桁目が操作の種類を示す
  switch (command[0]) {
    case 1:
      SingleTouch(0, 0);
    break;
    case 2:
      DoubleTouch(0, 0, 0);
    break;
    case 3:
      Hold(0, 0, 0);
    break;
    case 4:
      PressAndTouch(0, 0);
    break;
    case 5:
      Scroll(0, 0, 0, 0, 0);
    break;
    case 6:
      PinchIn(0, 0, 0, 0);
    break;
    case 7:
      PinchOut(0, 0, 0, 0);
    break;
    case 8:
      Rotate(0.0, 0.0, false, 0.0);
    break;
    default:
    LEDCheck(command[0]);
    Serial.println("=====INVALID COMMAND=====");
  }
  Serial.print("Read command: ");
  Serial.println(command[0]);
}

// CSV形式からint[]のコマンド実行形式へと変換する
int* ReadCommand(String csv) {
  String splited_csv[10];
  int index = Split(csv, ',', splited_csv);
  for (int i = 0; i < index; i++) {
    cmds[i] = splited_csv[i].toInt();
  }
  return cmds;
}

// 文字列を特定文字で分割するSplit関数
// data: 元の文字列
// delimiter: 区切り文字
// dst: 配列
// return: 
int Split(String data, char delimiter, String *dst) {
  int index = 0;
  int arraySize = (sizeof(data) / sizeof((data)[0]));
  int dataLength = data.length();
  
  // 全文字探索
  for (int i = 0; i < dataLength; i++) {
    char tmp = data.charAt(i);
    // 現在の文字が区切り文字の場合
    if (tmp == delimiter) {
      index++;
      // 算出したarraySizeより大きい場合はエラー
      if (index > (arraySize - 1)) return -1;
    }
    else {
      dst[index] += tmp;
    }
  }
  return (index + 1);
}

void SingleTouch(float pos_x, float pos_y) {
  Serial.println("=====SingleTouch=====");
  LEDCheck(1);
}

void DoubleTouch(float pos_x, float pos_y, float duration) {
  Serial.println("=====DoubleTouch=====");
  LEDCheck(2);
}

void Hold(float pos_x, float pos_y, float duration) {
  Serial.println("=====Hold=====");
  LEDCheck(3);
}

void PressAndTouch(float pos_x, float pos_y) {
  Serial.println("=====PressAndTouch=====");
  LEDCheck(4);
}

void Scroll(float start_x, float start_y, float end_x, float end_y, float duration) {
  Serial.println("=====Scroll=====");
  LEDCheck(5);
}

void PinchIn(float pos_x, float pos_y, float width, float duration) {
  Serial.println("=====PinchIn=====");
  LEDCheck(6);
}

void PinchOut(float pox_x, float pos_y, float width, float duration) {
  Serial.println("=====PinchOut=====");
  LEDCheck(7);
}

void Rotate(float center_x, float center_y, bool isClockWise, float angle) {
  Serial.println("=====Rotate=====");
  LEDCheck(8);
}

// 何かの動作検証をLEDチカチカで行うときの関数
// count: チカチカ回数
// duration: 点滅時間
void LEDCheck(int count) {
  for (int i = 0; i < count; i++) {
    digitalWrite(led, HIGH);
    delay(80);
    digitalWrite(led, LOW);
    delay(80);
  }
}
