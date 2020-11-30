using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldDurationAccTestManager : MonoBehaviour
{
    [SerializeField]
    private HoldEventReceiver holdEventReceiver;

    [SerializeField]
    private double threadTimerInMillis;

    /// <summary>
    /// 非同期でカウントするタイマー
    /// </summary>
    private double _threadTimer;

    /// <summary>
    /// タイマーロック
    /// </summary>
    private bool _timerLock;

    /// <summary>
    /// マルチスレッドブロック
    /// </summary>
    private readonly object _lockObj = new object();
    
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
                    _threadTimer += threadTimerInMillis;
                }
            }).AddTo(gameObject);
    }

    private void OnPointerDown(PointerEventData data)
    {
        // タイマーロックを解除してカウンターを0にする
        _timerLock = false;
        lock (_lockObj)
        {
            _threadTimer = 0;
        }
    }

    private void OnPointerUp(PointerEventData data)
    {
        _timerLock = true;
        Debug.Log($"HoldTime: {_threadTimer}");
    }
}
