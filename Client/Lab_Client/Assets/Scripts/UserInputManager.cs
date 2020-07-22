using System;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

public class UserInputManager : MonoBehaviour
{

    [SerializeField] private NetworkManager m_NetworkManager;

    [Header("ログに表示したい内容")] 
    [SerializeField] private bool ShowOnPointerDownAsObservable = true;
    [SerializeField] private bool ShowSingleTouchObservable = true;
    [SerializeField] private bool ShowDoubleTouchObservable = true;
    [SerializeField] private bool ShowHoldObservable = true;
    [SerializeField] private bool ShowDragObservable = true;
    [SerializeField] private bool ShowPinchInObservable = true;

    private TouchInfo m_TouchInfo = new TouchInfo();

    public static float WIDTH => Screen.width;
    
    public static float HEIGHT => Screen.height;

    private const float DOUBLE_TOUCH_DURATION = 0.2f;

    private const float HOLD_DURATION = 0.2f;

    private float m_LastReleaseTime;
    
    private TouchInfo m_DragStartInfo = new TouchInfo();

    private Vector2 m_PinchInStartTouch1;
    private Vector2 m_PinchInStartTouch2;
    private Vector2 m_PinchInEndTouch1;
    private Vector2 m_PinchInEndTouch2;

    private Vector2 m_PinchOutStart;
    private Vector2 m_PinchOutEnd;
    

    public struct TouchInfo
    {
        public Vector2 StartPoint { get; private set; }
        public float StartTime;

        public void SetStartPosition(Vector2 screenPos)
        {
            StartPoint = UserInputManager.CalcScreenUv(screenPos);
        }
    }

    private static Vector2 CalcScreenUv(Vector2 pos)
    {
        pos.x /= WIDTH;
        pos.y /= HEIGHT;
        return pos;
    }
    
    private void Start()
    {
        var eventTrigger = this.gameObject.AddComponent<ObservableEventTrigger>();
        eventTrigger.OnPointerDownAsObservable()
            .Subscribe(data =>
            {
                m_TouchInfo.SetStartPosition(data.position);
                m_TouchInfo.StartTime = Time.time;
                if (ShowOnPointerDownAsObservable)
                {
                    Debug.Log($"Touch Start. Point at {m_TouchInfo.StartPoint}");
                }
            });
        
        // 各種タッチ処理のイベント登録
        SingleTouch(eventTrigger);
        DoubleTouch(eventTrigger);
        Hold(eventTrigger);
        PressAndTouch(eventTrigger);
        Scroll(eventTrigger);
        PinchIn(eventTrigger);
        PinchOut(eventTrigger);
        Rotate(eventTrigger);
    }

    private void SingleTouch(ObservableEventTrigger trigger)
    {
        trigger.OnPointerUpAsObservable()
            .Where(data =>
            {
                // 前回指を離した時間から0.2sec以下ならダブルクリックとしてSingleTouchは発火しない
                if (Time.time - m_LastReleaseTime <= DOUBLE_TOUCH_DURATION || 
                    Time.time - m_TouchInfo.StartTime >= DOUBLE_TOUCH_DURATION)
                {
                    return false;
                }
                m_LastReleaseTime = Time.time;
                return true;
            })
            .Subscribe(data =>
            {
                // TODO: SingleTouch時の処理
                var uv = CalcScreenUv(data.position);
                m_NetworkManager.SetCommand($"1,{uv.x},{uv.y}");
                if (ShowSingleTouchObservable)
                {
                    Debug.Log($"SingleTouch: {uv}");
                }
            }).AddTo(gameObject);
    }

    private void DoubleTouch(ObservableEventTrigger trigger)
    {
        // 実装参考URL
        // https://qiita.com/miyaoka/items/6a643434a3f1548fb489
        var tapStream = Observable.EveryUpdate().Where(_ =>
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).phase == TouchPhase.Began;
            }
            return false;
        });
        tapStream.TimeInterval()
            .Select(t => t.Interval.TotalMilliseconds)
            .Buffer(2, 1)
            .Where(list => list[0] > 200)
            .Where(list => list[1] <= 200)
            .Where(_ => Input.touchCount == 1)
            .Select(_ => Input.GetTouch(0))
            .Subscribe(touch =>
            {
                var uv = CalcScreenUv(touch.position);
                m_NetworkManager.SetCommand($"2,{uv.x},{uv.y},0.2");
                if (ShowDoubleTouchObservable)
                {
                    Debug.Log($"DoubleTouch: {uv}");
                }
            }).AddTo(gameObject);
    }

    private void Hold(ObservableEventTrigger trigger)
    {
        this.UpdateAsObservable()
            .SkipUntil(trigger.OnPointerDownAsObservable())
            .TakeUntil(trigger.OnPointerUpAsObservable()
                .Do(_ =>
                {
                    if (Time.time - m_TouchInfo.StartTime >= HOLD_DURATION)
                    {
                        var uv = m_TouchInfo.StartPoint;
                        m_NetworkManager.SetCommand($"3,{uv.x},{uv.y},0.5");
                        if (ShowHoldObservable)
                        {
                            Debug.Log($"Hold: ");
                        }
                    }
                })
            )
            .RepeatUntilDestroy(this)
            .Subscribe(_ =>
            {
                if (ShowHoldObservable)
                {
                    Debug.Log("Holding...");
                }
            }).AddTo(gameObject);
    }

    private void PressAndTouch(ObservableEventTrigger trigger)
    {
        
    }

    private void Scroll(ObservableEventTrigger trigger)
    {
        trigger.OnPointerDownAsObservable()
            .Subscribe(data =>
            {
                m_DragStartInfo.SetStartPosition(data.position);
                m_DragStartInfo.StartTime = Time.time;
            }).AddTo(gameObject);
        
        this.UpdateAsObservable()
            .SkipUntil(trigger.OnBeginDragAsObservable())
            .TakeUntil(trigger.OnEndDragAsObservable().Do(data =>
            {
                // 二本指以上でのスクロールはピンチイン・アウトの可能性があるから
                if (Input.touchCount < 2)
                {
                    var startUv = m_DragStartInfo.StartPoint;
                    var endUv = CalcScreenUv(data.position);
                    m_NetworkManager.SetCommand($"5,{startUv.x},{startUv.y},{endUv.x},{endUv.y}");
                    if (ShowDragObservable)
                    {
                        Debug.Log($"Scroll: 5,{startUv.x},{startUv.y},{endUv.x},{endUv.y}");
                    }
                }
            }))
            .RepeatUntilDestroy(this)
            .Subscribe(_ =>
            {
                if (ShowDragObservable)
                {
                    Debug.Log("Scrolling...");
                }
            }).AddTo(gameObject);
    }

    private void PinchIn(ObservableEventTrigger trigger)
    {
        this.UpdateAsObservable()
            .Where(_ => Input.touchCount >= 2)
            .SkipUntil(trigger.OnPointerDownAsObservable().Do(_ =>
            {
                if (Input.touchCount >= 2)
                {
                    var touch1 = Input.GetTouch(0);
                    var touch2 = Input.GetTouch(1);
                    m_PinchInStartTouch1 = CalcScreenUv(touch1.position);
                    m_PinchInStartTouch2 = CalcScreenUv(touch2.position);
                }
            }))
            .TakeUntil(trigger.OnPointerUpAsObservable().Do(_ =>
            {
                if (Input.touchCount >= 2)
                {
                    var touch1 = Input.GetTouch(0);
                    var touch2 = Input.GetTouch(1);
                    m_PinchInEndTouch1 = CalcScreenUv(touch1.position);
                    m_PinchInEndTouch2 = CalcScreenUv(touch2.position);
                    // ピンチイン開始のタッチ指間の距離 < ピンチイン終了時のタッチ指間の距離
                    // がそもそもピンチインの定義
                    var startDist = Vector2.Distance(m_PinchInStartTouch1, m_PinchInStartTouch2);
                    var endDist = Vector2.Distance(m_PinchInEndTouch1, m_PinchInEndTouch2);
                    if (startDist < endDist)
                    {
                        var position = (m_PinchInStartTouch1 + m_PinchInStartTouch2) / 2.0f;
                        m_NetworkManager.SetCommand($"6,{position.x},{position.y},{endDist - startDist}");
                        if (ShowPinchInObservable)
                        {
                            Debug.Log($"PinchIn: 6,{position.x},{position.y},{endDist - startDist}");
                        }
                    }
                    else
                    {
                        var position = (m_PinchInStartTouch1 + m_PinchInStartTouch2) / 2.0f;
                        m_NetworkManager.SetCommand($"7,{position.x},{position.y},{startDist - endDist}");
                        if (ShowPinchInObservable)
                        {
                            Debug.Log($"PinchOut: 7,{position.x},{position.y},{startDist - endDist}");
                        }
                    }
                }
            }))
            .RepeatUntilDestroy(this)
            .Subscribe(_ =>
            {
                if (ShowPinchInObservable)
                {
                    Debug.Log("ピンチイン中");
                }
            }).AddTo(gameObject);
    }

    private void PinchOut(ObservableEventTrigger trigger)
    {
        
    }

    private void Rotate(ObservableEventTrigger trigger)
    {
        
    }
}