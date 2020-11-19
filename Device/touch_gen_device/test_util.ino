/*
 * 動作テストなどを行う際のメソッドを定義するタブ
 */

// タッチの間隔（mills)とタッチ回数を指定してタッチを生成する
// どれほどの早さまで正確にタッチを検出できるのかのテストに用いる
// duration: タッチ間隔(mills)
// touchCount: タッチ生成回数
void TouchDurationRapidTest(int duration, int touchCount) {
  Logln(LOG, "=====TouchDurationRapidTest=====");
  Logln(LOG, "duration: " + String(duration));
  Logln(LOG, "touchCount: " + String(touchCount));
  Touch(touchCount, duration);
  Logln(LOG, "=====TouchDurationRapidTest Finished=====");
}

// 整数値⇄ビットデータの変換が正しく動いているかのテスト
// testCount: 最大数値testCountまで検証する
// onlyFailed: 失敗時のみログに出力するようにする
void ConvertIntToBitTest(int testCount, bool onlyFailed) {
  Logln(LOG, "=====ConvertIntToBitTest=====");
  Logln(LOG, "testCount: " + String(testCount));
  byte bitArray[10];
  for (int i = 0; i < testCount; i++) {
    CopyIntToBitIntoArray(i, bitArray);
    int data = ConvertBitToInt(bitArray);
    
    // 成功時のログ出力
    if (onlyFailed == false && data == i) {
      Logln(LOG, "TestSuccess: " + String(i) + " == " + String(data));
      LogBitArray(LOG, bitArray);
      Logln(LOG, "\n");
    }
    
    // 失敗時のログ出力
    if (data != i) {
      Logln(ERR, "TestFailed: " + String(i) + " != " + String(data));
      LogBitArray(LOG, bitArray);
      Logln(ERR, "\n");
    }
  }
  Logln(LOG, "=====ConvertIntToBitTest Finished=====");
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

// 整数値をビット変換、さらにタッチデータとして送信する精度をテストするメソッド（Unity必須）
// testMaxCount: 0~テストする最大値に該当
// testDuration: タッチ間隔(millis)
// testWait: 一度データを送信するごとに待機する時間
void SendTouchDataAccuracyTest(int testMaxCount, int testDuration, int testWait) {
  byte bits[10];
  for (int i = 0; i < testMaxCount; i++) {
    Logln(LOG, "SendData: " + String(i));
    // 整数値→ビットデータ変換
    CopyIntToBitIntoArray(i, bits);
    LogBitArray(LOG, bits);
    // ビットデータ→タッチデータ変換・送信
    SendTouchDataFromBits(bits, testDuration);
    delay(testWait);
  }
}

// 整数値をビット変換、さらにホールドデータとして送信する制度をテストするメソッド（Unity必須）
// testMaxCoun: 0~テストする最大値に該当
// testDuration: ホールド時間(millis)
// testWait: 一度データを送信するごとに待機する時間
void SendHoldDataAccuracyTest(int testMaxCount, int testDuration, int testWait) {
  byte bits[10];
  for (int i = 0; i < testMaxCount; i++) {
    Logln(LOG, "SendData: " + String(i));
    // 整数値→ビットデータ変換
    CopyIntToBitIntoArray(i, bits);
    LogBitArray(LOG, bits);
    // ビットデータ→ホールドデータ変換・送信
    SendHoldDataFromBits(bits, testDuration);
    delay(testWait);
    Touch(1, testDuration);
  }
}

// ホールド時間間隔がどれだけの精度で出ているかテストするメソッド（Unity必須）
void TestHoldDurationAcc(int textMaxCount, int testDuration) {
  for (int i = 0; i < textMaxCount; i++) {
    unsigned long startTime = millis();
    Hold(1, testDuration);
    Logln(LOG, String(millis() - startTime));
    Hold(0, 100);  // 確実にタッチ離れを検知する時間間隔を設定しておく
  }
}
