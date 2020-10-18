using Model;
using UnityEngine;
using UnityEngine.UI;

public class TouchDurationRapidTestManager : MonoBehaviour
{
    [SerializeField] private Text resultText;
    [SerializeField] private bool realTimeLog;
    [SerializeField, Tooltip("連続でタッチされる回数")] private int touchCount;
    [SerializeField, Tooltip("タッチ間隔(millis)")] private float touchDuration;

    private int _currentTouchCount;

    private string _resultMsg;

    /// <summary>
    /// タッチ検知用のボタンクリック処理
    /// </summary>
    public void OnClickTouchReceiverButton()
    {
        _currentTouchCount++;
        UpdateResultText();
    }

    /// <summary>
    /// タッチカウントのリセット
    /// </summary>
    public void OnClickCountResetButton()
    {
        var result = new TouchDurationRapidTestResult
        {
            TestCount = $"タッチ回数: {touchCount}",
            TestDuration = $"タッチ間隔: {touchDuration}",
            DetectTouchCount = $"認識タッチ回数: {_currentTouchCount}",
            Accuracy = $"認識精度: {_currentTouchCount / (float)touchCount * 100.0f}",
        };
        // セーブ
        FileUtility.SaveAsJson(result);
        _currentTouchCount = 0;
    }

    private void UpdateResultText()
    {
        // リアルタイムでログ出力をしない（文字列変換などがパフォーマンスネックになる可能性があるので）
        if (realTimeLog == false)
        {
            return;
        }
        _resultMsg = $"testCount: {touchCount}\ntouchDuration: {touchDuration}\ncurrentTouchCount: {_currentTouchCount}\nAccuracy: {_currentTouchCount / (float)touchCount * 100.0f}";
        resultText.text = _resultMsg;
    }
}
