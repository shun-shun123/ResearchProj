using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
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

    [SerializeField] private int holdDurationInMillis;

    /// <summary>
    /// 別スレッドで走らせるタイマーのTick
    /// </summary>
    [SerializeField]
    private double threadTimerInMillis;

    [SerializeField]
    private HoldEventReceiver holdEventReceiver;

    private static readonly int DataSize = 10;
    
    private int[] _bitData = new int[DataSize];

    /// <summary>
    /// データの受信中フラグ
    /// </summary>
    private bool _isDataReceiving;

    /// <summary>
    /// 別スレッドでカウントするタイマー
    /// </summary>
    private int _threadTimer;

    private bool _timerLock;
    
    /// <summary>
    /// マルチスレッドブロック
    /// </summary>
    private readonly object _lockObj = new object();
    
    /// <summary>
    /// イベントデータを格納していくキュー
    /// </summary>
    private List<HoldEventRawData> eventDataList = new List<HoldEventRawData>();

    private HoldEventRawData.EventType _currentEventType = HoldEventRawData.EventType.Low;

    private int lastEventTime;

    private int releaseTime;
    private int pressTime;

    private void Start()
    {
        _bitData = new int[bitDataLength];
        QualitySettings.vSyncCount = vSyncCount;
        Application.targetFrameRate = targetFrameRate;

        holdEventReceiver.OnPointerDownAction = OnPointerDown;
        holdEventReceiver.OnPointerUpAction = OnPointerUp;
        
        CounterStart();
    }

    /// <summary>
    /// カウンターのスタート
    /// </summary>
    private void CounterStart()
    {
        Observable.Interval(TimeSpan.FromMilliseconds(threadTimerInMillis))
            .ObserveOn(Scheduler.ThreadPool)
            .Subscribe(_ =>
            {
                // ロック中はタイマーをカウントしない
                if (_timerLock)
                {
                    return;
                }
                lock (_lockObj)
                {
                    _threadTimer += (int) threadTimerInMillis;
                }
            }).AddTo(gameObject);
    }

    public void OnClickReceivingButton()
    {
        if (_isDataReceiving)
        {
            return;
        }
        _threadTimer = 0;
        _timerLock = false;
        pressTime = _threadTimer;
        releaseTime = _threadTimer;
        _currentEventType = HoldEventRawData.EventType.Low;
        StartCoroutine(DataReceivingCoroutine());
    }
    
    private void OnPointerDown(PointerEventData data)
    {
        if (_currentEventType == HoldEventRawData.EventType.High || _isDataReceiving == false)
        {
            return;
        }
        pressTime = _threadTimer;
        _currentEventType = HoldEventRawData.EventType.High;
        eventDataList.Add(new HoldEventRawData
        {
            StartAtInMillis = releaseTime,
            EndAtInMillis = pressTime,
            HoldEventType = HoldEventRawData.EventType.Low
        });
    }

    private void OnPointerUp(PointerEventData data)
    {
        if (_currentEventType == HoldEventRawData.EventType.Low || _isDataReceiving == false)
        {
            return;
        }
        releaseTime = _threadTimer;
        _currentEventType = HoldEventRawData.EventType.Low;
        eventDataList.Add(new HoldEventRawData
        {
            StartAtInMillis = pressTime,
            EndAtInMillis = releaseTime,
            HoldEventType = HoldEventRawData.EventType.High
        });
    }

    /// <summary>
    /// データ送信開始ビットを受け取ったら起動するコルーチン
    /// 10bitを受け取る時間が経過したら、_isDataReceivingフラグをOFFにする
    /// </summary>
    private IEnumerator DataReceivingCoroutine()
    {
        _isDataReceiving = true;
        yield return new WaitForSeconds(holdDurationInMillis / 1000.0f * bitDataLength + 0.5f);
        _isDataReceiving = false;
        PrintAllQueue();
        DecodeHoldEventRawDataListToBits(eventDataList);
    }

    private void PrintAllQueue()
    {
        foreach (var entry in eventDataList)
        {
            Debug.Log($"StartTime: {entry.StartAtInMillis}\nEndTime: {entry.EndAtInMillis}, Type: {entry.HoldEventType}");
        }
        eventDataList.Clear();
    }

    private void DecodeHoldEventRawDataListToBits(List<HoldEventRawData> data)
    {
        int index = 0;
        foreach (var d in data)
        {
            int duration = d.EndAtInMillis - d.StartAtInMillis;
            var type = d.HoldEventType;
            int length = duration / holdDurationInMillis;
            length += (duration % holdDurationInMillis) <= 20 ? 0 : 1;
            for (var i = 0; i < length; i++)
            {
                if (_bitData.Length < index)
                {
                    _bitData[index] = type == HoldEventRawData.EventType.High ? 1 : 0;
                    index++;
                }
            }
        }

        string log = "";
        foreach (var bit in _bitData)
        {
            log += bit;
        }
        Debug.Log($"DecodeBits: {log}");
    }

    public class HoldEventRawData
    {
        public int StartAtInMillis;
        public int EndAtInMillis;
        public EventType HoldEventType;

        /// <summary>
        /// イベントのタイプ
        /// </summary>
        public enum EventType
        {
            High,
            Low,
        }
    }
}
