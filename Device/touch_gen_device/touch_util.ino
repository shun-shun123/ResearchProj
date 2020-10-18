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
    digitalWrite(led, HIGH);
    delay(touchDuration);
    digitalWrite(led, LOW);
    delay(touchDuration);
  }
}

// bitデータをタッチに変換する
// bitArray: ビットデータ配列[10]
// touchDuration: タッチ間隔(mills)
void GenerateTouchDataFromBits(byte* bitArray, int touchDuration) {
  for (int i = 0; i < 10; i++) {
    // ログに出力するのは「リトルエンディアン」の方がわかりやすいため、変換(リトルエンディアン: 最右ビットが最小値）
    Serial.print(bitArray[9 - i]);
    
    // 「0」ビットはタッチしない
    if (bitArray[i] == 0) {
      delay(touchDuration);
      continue;
    }
    
    // 「1」ビットならタッチを生成
    Touch(1, touchDuration / 2);
  }
}
