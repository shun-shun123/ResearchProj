// ログ出力
// mode: ログモード
// msg: ログメッセージ
void Log(byte mode, String msg) {
  if (logMode <= mode) {
    Serial.print(msg);
  }
}

// 改行付きログ出力
// mode: ログモード
// msg: ログメッセージ
void Logln(byte mode, String msg) {
  if (logMode <= mode) {
    Serial.println(msg);
  }
}

// リトルエンディアンで格納されたビットデータをログに出力する
// bitArray: bit配列[10]
void LogBitArray(byte mode, byte* bitArray) {
  if (logMode <= mode) {
    for (int i = 9; i >= 0; i--) {
      Serial.print(bitArray[i]);
    }
    Serial.println();
  }
}
