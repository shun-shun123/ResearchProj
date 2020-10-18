/*
 * Analog⇄Digitalのデータ変換を行うためのメソッドを定義する
 */


// int->bit
// 整数値をbitデータに変換する
// 最大bit数は10bit
// [9] ~ [0] の順番
void CopyIntToBitIntoArray(int data, byte* byteArray) {
  // 書き込み開始通知タッチ
  int current = data;
  for (int i = 0; i < 10; i++) {
    byteArray[i] = current % 2;
    
    // 1以下の値の場合は特殊処理
    if (current <= 1) {
      current = 0;
    } else {
      current /= 2;
    }
  }
  return byteArray;
}

// bit->int
// ビットデータを表す[10]の配列を整数値に変換するメソッド
// bitArray: ビットデータを格納した長さ10の配列(リトルエンディアン: 最右のビットが最小値)
int ConvertBitToInt(byte* bitArray) {
  int mult = 1;
  int data = 0;
  for (int i = 0; i < 10; i++) {
    data += bitArray[i] * mult;
    mult *= 2;
  }
  return data;
}
