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
        if (holdDurationInSec * (index + 1) <= fixedTimer &&
            fixedTimer <= holdDurationInSec * (index + 1) + threshold && 
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
        yield return new WaitForSeconds(holdDurationInSec * bitDataLength);
        _isDataReceiving = false;
        fixedLock = true;
        fixedTimer = 0f;
        index = 0;
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
