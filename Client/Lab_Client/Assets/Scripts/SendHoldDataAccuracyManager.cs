using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    /// <summary>
    /// +-のレンジ(millis)
    /// </summary>
    [SerializeField]
    private int missRange;

    /// <summary>
    /// 最初のタッチを待つための時間
    /// </summary>
    [SerializeField]
    private float touchWaitInSeconds;

    /// <summary>
    /// 2進数で表示する数字
    /// </summary>
    [SerializeField]
    private Text bitText;

    /// <summary>
    /// 10進数で表示する数字
    /// </summary>
    [SerializeField]
    private Text digitText;

    /// <summary>
    /// PointerUp/Downの状態を可視化するためのオブジェクト
    /// OnPointerDown時にアクティブになり、OnPointerUp時に非アクティブになる
    /// </summary>
    [SerializeField]
    private GameObject eventVisualizer;

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
        StartCoroutine(DataReceivingCoroutine());
    }
    
    private void OnPointerDown(PointerEventData data)
    {
        eventVisualizer.gameObject.SetActive(true);
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
        eventVisualizer.gameObject.SetActive(false);
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
        _threadTimer = 0;
        pressTime = _threadTimer;
        releaseTime = _threadTimer;
        yield return new WaitForSeconds(touchWaitInSeconds);
        _timerLock = false;
        _currentEventType = HoldEventRawData.EventType.Low;
        yield return new WaitForSeconds(holdDurationInMillis / 1000.0f * bitDataLength + 0.5f);
        _isDataReceiving = false;
        _timerLock = true;
        PrintAllQueue();
        DecodeHoldEventRawDataListToBits(eventDataList);
        eventDataList.Clear();
    }

    private void PrintAllQueue()
    {
        foreach (var entry in eventDataList)
        {
            Debug.Log($"StartTime: {entry.StartAtInMillis}\nEndTime: {entry.EndAtInMillis}, Type: {entry.HoldEventType}");
        }
    }

    private void DecodeHoldEventRawDataListToBits(List<HoldEventRawData> data)
    {
        bitText.text = "";
        digitText.text = "";
        int index = 0;
        foreach (var d in data)
        {
            int duration = d.EndAtInMillis - d.StartAtInMillis;
            // 除算をしてとりあえず割れる分はlength, 剰余は長さによっては1bitの長さを付与することになる
            int length = duration / holdDurationInMillis + (duration % holdDurationInMillis <= missRange ? 0 : 1);
            for (var i = 0; i < length; i++)
            {
                if (index < _bitData.Length)
                {
                    _bitData[index] = d.HoldEventType == HoldEventRawData.EventType.High ? 1 : 0;
                    index++;
                }
            }
        }

        for (; index < _bitData.Length; index++)
        {
            _bitData[index] = 0;
        }

        int value = 0;
        for (var i = 0; i < _bitData.Length; i++)
        {
            value += _bitData[i] << i;
            bitText.text += _bitData[i];
        }

        digitText.text = value.ToString();
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
