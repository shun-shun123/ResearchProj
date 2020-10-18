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
