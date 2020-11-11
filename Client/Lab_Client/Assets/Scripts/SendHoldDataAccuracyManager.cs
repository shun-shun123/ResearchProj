using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class SendHoldDataAccuracyManager : MonoBehaviour
{
    [Header("Test Static Parameters")]
    [Tooltip("送信するビットデータの長さ")]
    [SerializeField] private int bitDataLength;
    [Tooltip("VSyncCount: 0の場合ディスプレイの垂直同期とは独立した更新になる")]
    [SerializeField] private int vSyncCount;
    [Tooltip("TargetFrameRate: 十分に高い値を設定すると良い")]
    [SerializeField] private int targetFrameRate;

    [Header("Test Dynamic Parameters")] 
    [SerializeField] private float holdDurationInSec;
    [SerializeField] private float testMaxCount;
    [SerializeField] private bool needsLog;
    [Tooltip("閾値検知のための幅")]
    [SerializeField] private float threshold;
    [Tooltip("デバイス遅延")]
    [SerializeField] private float deviceDelay;
    [SerializeField] private bool isTestMode;

    [SerializeField]
    private HoldEventReceiver holdEventReceiver;
    
    private int[] _bitData;

    private bool _isPressing;

    /// <summary>
    /// データの受信中フラグ
    /// </summary>
    private bool _isDataReceiving;

    private float fixedTimer;

    private bool fixedLock;

    private int index;

    private int testCount;

    private int successCount;

    private void Start()
    {
        _bitData = new int[bitDataLength];
        QualitySettings.vSyncCount = vSyncCount;
        Application.targetFrameRate = targetFrameRate;

        holdEventReceiver.OnPointerDownAction = OnPointerDown;
        holdEventReceiver.OnPointerUpAction = OnPointerUp;
    }

    public void OnClickReceivingButton()
    {
        if (_isDataReceiving)
        {
            return;
        }
        StartCoroutine(DataReceivingCoroutine());
    }

    private void FixedUpdate()
    {
        if (fixedLock)
        {
            return;
        }
        fixedTimer += Time.fixedDeltaTime;
        // 入力開始想定時間 + (入力受付時間 / 4) ~ 入力終了想定時間 - (入力受付時間 / 4f)の間で入力値をチェックする
        if (holdDurationInSec * (index + 1) + threshold + (index * deviceDelay) <= fixedTimer &&
            fixedTimer <= holdDurationInSec * (index + 2) - threshold + (index * deviceDelay) && 
            index < bitDataLength)
        {
            _bitData[index] = _isPressing ? 1 : 0;
            Log($"FixedTime: {fixedTimer} bit: {_isPressing}");
            index++;
        }
    }

    private void OnPointerDown(PointerEventData data)
    {
        _isPressing = true;
    }

    private void OnPointerUp(PointerEventData data)
    {
        _isPressing = false;
    }

    /// <summary>
    /// データ送信開始ビットを受け取ったら起動するコルーチン
    /// 10bitを受け取る時間が経過したら、_isDataReceivingフラグをOFFにする
    /// </summary>
    private IEnumerator DataReceivingCoroutine()
    {
        // データ送信開始タッチの後holdDurationInSec分待機時間が発生するためそれを待つ
        _isDataReceiving = true;
        fixedLock = false;
        yield return new WaitForSeconds(holdDurationInSec);
        yield return new WaitForSeconds(holdDurationInSec * bitDataLength + deviceDelay * bitDataLength);
        _isDataReceiving = false;
        fixedLock = true;
        fixedTimer = 0f;
        index = 0;
        LogBitToInt();
        if (isTestMode)
        {
            int read = BitToInt(_bitData);
            successCount += read == testCount ? 1 : 0;
            testCount++;
            if (testCount >= testMaxCount)
            {
                Debug.Log($"Success: {successCount}");
                Debug.Log($"TestCount: {testCount}");
                Debug.Log($"ACC: {successCount / (float)testCount}");
                testCount = 0;
                successCount = 0;
            }
        }
    }

    private int BitToInt(int[] bits)
    {
        int num = 0;
        for (var i = 0; i < bits.Length; i++)
        {
            num += bits[i] << i;
        }

        return num;
    }

    private void LogBitToInt()
    {
        int num = BitToInt(_bitData);
        Debug.Log($"num: {num}");
    }

    private void Log(string msg)
    {
        if (!needsLog)
        {
            return;
        }
        Debug.Log(msg);
    }
}
