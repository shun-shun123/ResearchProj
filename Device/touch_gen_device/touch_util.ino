/*
 * タッチ関連のメソッドを定義するタブ
 */

// 関数終了までには2 x touchDuration (mills)かかる
// NOTE: 0回タッチ（タッチせずに1回分待機）にも対応
// touchDuration: LEDがHIGH→LOWに切り替わる時間
void Touch(int touchCount, int touchDuration) {
  // タッチせずに1回分待機する場合の処理
  if (touchCount == 0) {
    CustomDelayInMs(touchDuration * 2);
    return;
  }
  
  // タッチ生成する場合の処理
  for (int i = 0; i < touchCount; i++) {
    digitalWrite(TOUCH_PIN, HIGH);
    CustomDelayInMs(touchDuration);
    digitalWrite(TOUCH_PIN, LOW);
    CustomDelayInMs(touchDuration);
  }
}

// 0,1のビットデータをホールドデータに変換する
// bitData: 0 or 1
// holdDuration: ホールドし続ける時間　（ms）
void Hold(int bitData, int holdDuration) {
  if (bitData == 0) {
    digitalWrite(TOUCH_PIN, LOW);
    CustomDelayInMs(holdDuration + 25);
  } else if (bitData == 1) {
    digitalWrite(TOUCH_PIN, HIGH);
    CustomDelayInMs(holdDuration);
  }
}


// bitデータをタッチに変換する
// bitArray: ビットデータ配列[10]
// touchDuration: タッチ間隔(mills)
void GenerateTouchDataFromBits(byte* bitArray, int touchDuration) {
  LogBitArray(LOG, bitArray);
  for (int i = 0; i < 10; i++) {
    // 「0」ビットはタッチしない
    if (bitArray[i] == 0) {
      Touch(0, touchDuration);
      continue;
    }
    
    // 「1」ビットならタッチを生成
    Touch(1, touchDuration);
  }
}

// bitデータをホールドデータに変換する
// bitArray: ビットデータ配列[10]
// holdDuration: ホールド時間(millis)
void GenerateHoldDataFromBits(byte* bitArray, int holdDuration) {
  for (int i = 0; i < 10; i++) {
    unsigned long startTime = millis();
    Hold(bitArray[i], holdDuration);
    Logln(LOG, "Hold in " + String(millis() - startTime) + " [" + String(bitArray[i]) + "]");
  }
  digitalWrite(TOUCH_PIN, LOW);
}

// 受信側にタッチデータを送信するメソッド
// bitArray: ビットデータ配列[10]
// touchDuration: タッチ間隔
void SendTouchDataFromBits(byte* bitArray, int touchDuration) {
  // 送信開始タッチ
  Touch(1, touchDuration);
  // タッチデータ生成
  GenerateTouchDataFromBits(bitArray, touchDuration);
}

// 受信側にホールドデータを送信するメソッド
// bitArray: ビットデータ配列[10]
// holdDuration: ホールド間隔(milis)
void SendHoldDataFromBits(byte* bitArray, int holdDuration) {
  // 送信開始タッチ
  Touch(1, holdDuration);
  // ホールドデータ送信
  GenerateHoldDataFromBits(bitArray, holdDuration);
}

void CustomDelayInMs(int ms) {
  unsigned long time_start = millis();
  while (millis() - time_start < ms) {}
}
