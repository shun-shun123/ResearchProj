using Model;
using UnityEngine;

public class SendTouchDataAccuracyManager : MonoBehaviour
{
    [SerializeField] private float touchDurationInSec;
    [SerializeField] private int testMaxCount;
    [SerializeField] private bool isSaveResult;

    private BitTouchReceiveModule _bitTouchReceiveModule;

    private int _currentTestCount;

    private int _missCount;

    private int _correctCount;
    
    private SendTouchDataAccuracyResult _result = new SendTouchDataAccuracyResult();
    
    void Start()
    {
        _bitTouchReceiveModule = new BitTouchReceiveModule(this, touchDurationInSec, OnBitDataReceivedAction);
    }

    public void OnClickReceiveButton()
    {
        _bitTouchReceiveModule.OnClickReceiveButtonAction();
    }

    private void OnBitDataReceivedAction(int data, int[] bits)
    {
        if (data == _currentTestCount)
        {
            _correctCount++;
            Debug.Log($"TRUE: data: {data}, currentTestCount: {_currentTestCount}");
        }
        else
        {
            _missCount++;
            Debug.LogError($"FALSE: data: {data}, currentTestCount: {_currentTestCount}");
        }

        _currentTestCount++;
        if (testMaxCount == _currentTestCount)
        {
            Debug.Log("TEST FINISHED");
            _result.TestMaxCount = $"テスト最大整数値: {testMaxCount}";
            _result.TestDuration = $"テストタッチ間隔(millis): {touchDurationInSec}";
            _result.MissTouchCount = $"誤認識回数: {_missCount}";
            _result.CorrectTouchCount = $"正認識回数: {_correctCount}";
            _result.Accuracy = $"認識率: {_correctCount / (float) (_correctCount + _missCount) * 100.0f}%";
            var json = JsonUtility.ToJson(_result);
            Debug.Log(json);
            _currentTestCount = 0;
            _result = new SendTouchDataAccuracyResult();
        }
    }

    private void OnValidate()
    {
        if (_bitTouchReceiveModule != null)
        {
            _bitTouchReceiveModule.TouchDurationInSec = touchDurationInSec;
        }
    }
}
