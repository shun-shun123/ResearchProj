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
    

    public struct TouchInfo
    {
        public Vector2 StartPoint { get; private set; }
        public float StartTime;

        public void SetStartPosition(Vector2 screenPos)
        {
            screenPos.x /= WIDTH;
            screenPos.y /= HEIGHT;
            StartPoint = screenPos;
        }
    }
    
    private void Start()
    {
        var eventTrigger = this.gameObject.AddComponent<ObservableEventTrigger>();
        eventTrigger.OnPointerDownAsObservable()
            .Subscribe(data =>
            {
                m_TouchInfo.SetStartPosition(data.position);
                m_TouchInfo.StartTime = data.clickTime;
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