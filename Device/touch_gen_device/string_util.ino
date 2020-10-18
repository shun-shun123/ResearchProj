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
