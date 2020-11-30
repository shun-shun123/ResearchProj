using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldDurationAccTestManager : MonoBehaviour
{
    /// <summary>
    /// 一度で行うテスト回数
    /// </summary>
    [SerializeField]
    private int testCount;

    /// <summary>
    /// ホールド時間（理想）
    /// </summary>
    [SerializeField]
    private int textHoldDuration;
    
    [SerializeField]
    private HoldEventReceiver holdEventReceiver;

    /// <summary>
    /// 別スレッドで走らせるタイマーのTick
    /// </summary>
    [SerializeField]
    private double threadTimerInMillis;

    /// <summary>
    /// 非同期でカウントするタイマー
    /// </summary>
    private int _threadTimer;

    /// <summary>
    /// タイマーロック
    /// </summary>
    private bool _timerLock;

    /// <summary>
    /// マルチスレッドブロック
    /// </summary>
    private readonly object _lockObj = new object();

    /// <summary>
    /// 現在の試行回数
    /// </summary>
    private int _currentTestIter;

    /// <summary>
    /// タイマーの合計（平均を出力するために使う）
    /// </summary>
    private int _totalTimer;
    
    private void Start()
    {
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
                    _threadTimer += (int)threadTimerInMillis;
                }
            }).AddTo(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentTestIter = 0;
        }
    }

    private void OnPointerDown(PointerEventData data)
    {
        // タイマーロックを解除してカウンターを0にする
        _timerLock = false;
        _currentTestIter++;
        lock (_lockObj)
        {
            _threadTimer = 0;
        }
    }

    private void OnPointerUp(PointerEventData data)
    {
        _timerLock = true;
        _totalTimer += _threadTimer;
        Debug.Log($"HoldDuration: {_threadTimer}");
        if (_currentTestIter == testCount)
        {
            Debug.Log($"Average: {_totalTimer / (float)testCount}");
        }
    }
}
