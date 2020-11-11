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
    [Tooltip("デバイスの処理速度によって発生する微妙な遅延を調整するための値")]
    [SerializeField] private float deviceDelayAdjustInSec;
    [SerializeField] private bool needsLog;

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
        Log($"FixedTime: {fixedTimer} bit: {_isPressing}");
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
        yield return new WaitForSeconds(holdDurationInSec * bitDataLength);
        _isDataReceiving = false;
        fixedLock = true;
        LogBitToInt();
    }

    private void LogBitToInt()
    {
        int num = 0;
        for (var i = 0; i < _bitData.Length; i++)
        {
            num += _bitData[i] << i;
        }
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
