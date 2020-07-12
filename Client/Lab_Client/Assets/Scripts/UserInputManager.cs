using System;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

public class UserInputManager : MonoBehaviour
{

    [SerializeField] private NetworkManager m_NetworkManager;

    private TouchInfo m_TouchInfo = new TouchInfo();

    public static float WIDTH => Screen.width;
    
    public static float HEIGHT => Screen.height;

    private const float FAST_TOUCH_SEC = 0.2f;
    

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
                Debug.Log($"Touch Start. Point at {m_TouchInfo.StartPoint}");
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
                var screenAspectPoint = new Vector2(data.position.x / WIDTH, data.position.y / HEIGHT);
                // 条件1: タッチ開始地点から動いていないか
                bool isNotMoved = Vector2.Distance(screenAspectPoint, m_TouchInfo.StartPoint) <= 0.1f;

                // 条件2: タッチ開始からの経過時間が0.2sec以下か
                bool isFastTouch = (Time.time - m_TouchInfo.StartTime) <= FAST_TOUCH_SEC;

                return isNotMoved && isFastTouch;
            })
            .Subscribe(data =>
            {
                // TODO: SingleTouch時の処理
                Debug.Log($"SingleTouch: {CalcScreenUv(data.position)}");
            }).AddTo(gameObject);
    }

    private void DoubleTouch(ObservableEventTrigger trigger)
    {
    }

    private void Hold(ObservableEventTrigger trigger)
    {
        
    }

    private void PressAndTouch(ObservableEventTrigger trigger)
    {
        
    }

    private void Scroll(ObservableEventTrigger trigger)
    {
        
    }

    private void PinchIn(ObservableEventTrigger trigger)
    {
        
    }

    private void PinchOut(ObservableEventTrigger trigger)
    {
        
    }

    private void Rotate(ObservableEventTrigger trigger)
    {
        
    }
}