/*
 * 動作テストなどを行う際のメソッドを定義するタブ
 */


// タッチ精度テスト用のメソッド
// 0~testCount までテスト
void RunTest(int testDuration, int testCount) {
  Serial.println("=====Test Duration(" + String(testDuration) + ")millisec=====");
  for (int i = 0; i <= testCount; i++) {
    int number = i;
    Serial.print(String(number) + ": ");
    // 書き込み開始タッチ
    Touch(1, testDuration);
    // 10bitの数値を計算する
    for (int j = 0; j < 10; j++) {
      // bit「0」
      if (number % 2 == 0) {
        delay(testDuration);
      } else {
        // bitbit「1」
        Touch(1, testDuration / 2);
      }
      Serial.print(number % 2);
      if (number <= 1) {
        number = 0;
      } else {
        number /= 2;
      }
    }
    Serial.println();
    delay(600);
  }
}

// タッチの間隔（mills)とタッチ回数を指定してタッチを生成する
// どれほどの早さまで正確にタッチを検出できるのかのテストに用いる
// duration: タッチ間隔(mills)
// touchCount: タッチ生成回数
void TouchDurationRapidTest(int duration, int touchCount) {
  Serial.println("=====TouchDurationRapidTest=====");
  Serial.println("duration: " + String(duration));
  Serial.println("touchCount: " + String(touchCount));
  Touch(touchCount, duration);
  Serial.println("=====TouchDurationRapidTest Finished=====");
}

// 整数値⇄ビットデータの変換が正しく動いているかのテスト
// testCount: 最大数値testCountまで検証する
// onlyFailed: 失敗時のみログに出力するようにする
void ConvertIntToBitTest(int testCount, bool onlyFailed) {
  Serial.println("=====ConvertIntToBitTest=====");
  Serial.println("testCount: " + String(testCount));
  byte bitArray[10];
  for (int i = 0; i < testCount; i++) {
    CopyIntToBitIntoArray(i, bitArray);
    int data = ConvertBitToInt(bitArray);
    
    // 成功時のログ出力
    if (onlyFailed == false && data == i) {
      Serial.println("TestSuccess: " + String(i) + " == " + String(data));
      PrintBitArray(bitArray);
      Serial.println("\n");
    }
    
    // 失敗時のログ出力
    if (data != i) {
      Serial.println("TestFailed: " + String(i) + " != " + String(data));
      PrintBitArray(bitArray);
      Serial.println("\n");
    }
  }
  Serial.println("=====ConvertIntToBitTest Finished=====");
}

// log_utilに定義しているログ出力メソッドが正しく動作するかのテスト
// 「これは出力されない」という文言がシリアルモニタに表示されると失敗
void LogModeTest() {
  byte currentLogMode = logMode;
  logMode = LOG;
  Log(LOG, "[LOG]これは出力される");
  Log(WARN, "[WARN]これは出力される");
  Log(ERR, "[ERR]これは出力される");
  Logln(LOG, "[LOG]これは出力される");
  Logln(WARN, "[WARN]これは出力される");
  Logln(ERR, "[ERR]これは出力される");
  
  logMode = WARN;
  Log(LOG, "[LOG]これは出力されない");
  Log(WARN, "[WARN]これは出力される");
  Log(ERR, "[ERR]これは出力される");
  Logln(LOG, "[LOG]これは出力されない");
  Logln(WARN, "[WARN]これは出力される");
  Logln(ERR, "[ERR]これは出力される");

  logMode = ERR;
  Log(LOG, "[LOG]これは出力されない");
  Log(WARN, "[WARN]これは出力されない");
  Log(ERR, "[ERR]これは出力される");
  Logln(LOG, "[LOG]これは出力されない");
  Logln(WARN, "[WARN]これは出力されない");
  Logln(ERR, "[ERR]これは出力される");

  logMode = NONE;
  Log(LOG, "[LOG]これは出力されない");
  Log(WARN, "[WARN]これは出力されない");
  Log(ERR, "[ERR]これは出力されない");
  Logln(LOG, "[LOG]これは出力されない");
  Logln(WARN, "[WARN]これは出力されない");
  Logln(ERR, "[ERR]これは出力されない");
}

// 自作ログライブラリlog_utilのパフォーマンス測定
// testCount: ログ出力回数
// calcTimes: 試行回数
void TestLogPerformance(int testCount, int calcTimes) {
  // 現状のログモードを回避(このテストメソッドがメインルーチンから非依存的なものにするため)
  byte currentLogMode = logMode;
  logMode = NONE;
  unsigned long systemLog = 0;
  unsigned long myLogUtil = 0;
  for (int j = 0; j < calcTimes; j++) {
    unsigned long currentTime_m = millis();
    // 通常のSerial.printlnのパフォーマンス測定
    for (int i = 0; i < testCount; i++) {
      Serial.print("log");
    }
    Serial.println("\n=====Performance Serial.println()=====");
    Serial.println("Time: " + String(millis() - currentTime_m));
    systemLog += millis() - currentTime_m;

    // log_utilのパフォーマンス測定
    currentTime_m = millis();
    for (int i = 0; i < testCount; i++) {
      logMode = int(random(0, 4));
      Log(int(random(0, 3)), "log");
    }
    Serial.println("\n=====Performance Logln()=====");
    Serial.println("Time: " + String(millis() - currentTime_m));
    myLogUtil += millis() - currentTime_m;
  }
  systemLog /= float(calcTimes);
  myLogUtil /= float(calcTimes);
  Serial.println("=====TestLogPerformance Result=====");
  Serial.println("testCount: " + String(testCount) + "calcTimes: " + String(calcTimes));
  Serial.println("Serial.print(): " + String(systemLog) + "millis");
  Serial.println("Log(): " + String(myLogUtil) + "millis");
  // ログモード復帰
  logMode = currentLogMode;
}
