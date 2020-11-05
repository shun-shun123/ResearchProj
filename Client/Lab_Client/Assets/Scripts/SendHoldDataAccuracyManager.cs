using UnityEngine;
using UnityEngine.EventSystems;

public class SendHoldDataAccuracyManager : MonoBehaviour
{
    [SerializeField] private HoldEventReceiver holdEventReceiver;

    [Header("Test Parameters")] 
    [SerializeField] private float holdDuration;
    [SerializeField] private float testMaxCount;
    
    private float lastTime;

    private readonly int[] _bitData = new int[10];

    /// <summary>
    /// データの受信中フラグ
    /// </summary>
    private bool _isDataReceiving;

    private void Start()
    {
        holdEventReceiver.OnPointerDownAction = OnPointerDown;
        holdEventReceiver.OnPointerUpAction = OnPointerUp;
    }

    private void OnPointerDown(PointerEventData data)
    {
        lastTime = Time.time;
        if (_isDataReceiving)
        {
            // TODO: データ受信中の処理
        }
        else
        {
            _isDataReceiving = true;
        }
    }

    private void OnPointerUp(PointerEventData data)
    {
        Debug.Log($"Difference: {Time.time - lastTime}");
    }
}
