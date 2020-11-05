/*
 * タッチ関連のメソッドを定義するタブ
 */

// 関数終了までには2 x touchDuration (mills)かかる
// NOTE: 0回タッチ（タッチせずに1回分待機）にも対応
// touchDuration: LEDがHIGH→LOWに切り替わる時間
void Touch(int touchCount, int touchDuration) {
  // タッチせずに1回分待機する場合の処理
  if (touchCount == 0) {
    delay(touchDuration * 2);
    return;
  }
  
  // タッチ生成する場合の処理
  for (int i = 0; i < touchCount; i++) {
    digitalWrite(TOUCH_PIN, HIGH);
    delay(touchDuration);
    digitalWrite(TOUCH_PIN, LOW);
    delay(touchDuration);
  }
}

// 0,1のビットデータをホールドデータに変換する
// bitData: 0 or 1
// holdDuration: ホールドし続ける時間　（ms）
void Hold(int bitData, int holdDuration) {
  if (bitData == 0) {
    digitalWrite(TOUCH_PIN, LOW);
    delay(holdDuration);
  } else if (bitData == 1) {
    digitalWrite(TOUCH_PIN, HIGH);
    delay(holdDuration);
  } else {
    Logln(LOG, "bitDataが0,1以外で来ています");
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
  LogBitArray(LOG, bitArray);
  for (int i = 0; i < 10; i++) {
    Hold(bitArray[i], holdDuration);
  }
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
