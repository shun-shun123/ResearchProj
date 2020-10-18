/*
 * メイン処理の記述タブ
 */

int led = 13;
int cmds[10];
int duration = 100;
int testCount = 30;

// ログのフィルタリング
const byte LOG = 0;
const byte WARN = 1;
const byte ERR = 2;
const byte NONE = 3;
byte logMode= LOG;

// タッチの間隔を徐々に早め、どれだけの精度で読み取り続けられるかのテストコマンド
const int TEST_TOUCH_DURATION = 100;
// 整数値をビットデータに変換し、正しく変換できているかのテストコマンド
const int TEST_INT_TO_BIT_DATA = 101;
// log_utilに定義したログ出力用メソッドと、そのフィルタリングが正しく動作するかのテストコマンド
const int TEST_LOG_UTIL = 102;
// 自作ログライブラリlog_utilのパフォーマンス測定
const int TEST_LOG_UTIL_PERFORMANCE = 103;
// 整数値からビット変換したものをタッチデータにして生成する
const int GENERATE_TOUCH_FROM_INT = 104;
// 整数値をビット変換、さらにタッチデータに変換し送信した時の精度をテストする（Unity必須）
const int TEST_SEND_TOUCH_DATA = 105;

float sensorValue;

void setup() {
  pinMode(led, OUTPUT);
  Serial.begin(9600);
}

void loop() {
  // Serialポートに何バイトかデータが到着しているかを返すテスト回数
  if (Serial.available() <= 0) {
    return;
  }
  // センサ読み取り
  readSensorValue();
  // シリアルポートから入力を受け付ける
  String input = Serial.readString();

  // CSV形式のデータからコマンドを読み取る
  int* read_cmds = ReadCommand(input);
  
  // コマンドを実行する
  ExecuteCommand(read_cmds);
}

// int[]に変換されたコマンドを実行する
// command: コマンド整数値が入った配列へのポインタ
void ExecuteCommand(int* command) {
  Logln(LOG, "Read command: " + String(command[0]));
  // CSV形式の一桁目が操作の種類を示す
  switch (command[0]) {
    // 100: タッチの間隔を徐々に早め、どれだけの精度で読み取り続けられるかのテストコマンド
    case TEST_TOUCH_DURATION:
      // command[0]: コマンド
      // command[1]: タッチ間隔(mills)
      // command[2]: タッチ回数
      TouchDurationRapidTest(command[1], command[2]);
      break;
    // 101: 整数値をビットデータに変換し、正しく変換できているかのテストコマンド
    case TEST_INT_TO_BIT_DATA:
      // command[0]: コマンド
      // command[1]: 検証する最大の整数値
      // command[2]: ログ出力モード(0: 失敗時のみ, それ以外: 全て出力)
      ConvertIntToBitTest(command[1], command[2] == 0);
      break;
    // 102: log_utilに定義したログ出力用メソッドと、そのフィルタリングが正しく動作するかのテストコマンド
    case TEST_LOG_UTIL:
      // command[0]: コマンド
      LogModeTest();
      break;
    // 103: 自作ログライブラリlog_utilのパフォーマンス測定
    case TEST_LOG_UTIL_PERFORMANCE:
      // command[0]: コマンド
      // command[1]: ログ出力回数
      // command[2]: 試行回数
      TestLogPerformance(command[1], command[2]);
      break;
    // 104: 整数値からビット変換したものをタッチデータにして生成する
    case GENERATE_TOUCH_FROM_INT:
      // command[0]: コマンド
      // command[1]: 整数値
      // command[2]: タッチ間隔(millis)
      byte bits[10];
      CopyIntToBitIntoArray(command[1], bits);
      SendTouchDataFromBits(bits, command[2]);
      break;
    // 105: 整数値をビット変換、さらにタッチデータに変換し送信した時の精度をテストする（Unity必須）
    case TEST_SEND_TOUCH_DATA:
      // command[0]: コマンド
      // command[1]: テスト最大整数値
      // command[2]: タッチ間隔(millis)
      // command[3]: 一回送信ごとの待機時間(millis)
      SendTouchDataAccuracyTest(command[1], command[2], command[3]);
      break;
    case 1024:
      for (int i = 100; i >= 10; i -= 10) {
        RunTest(i, 30);
      }
    default:
      Logln(ERR, "=====INVALID COMMAND=====");
  }
}
