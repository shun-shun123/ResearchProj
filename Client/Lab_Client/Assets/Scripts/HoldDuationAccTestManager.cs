using UnityEngine;
using UnityEngine.EventSystems;

public class HoldDuationAccTestManager : MonoBehaviour
{
    [SerializeField]
    private int testMaxCount;

    [SerializeField]
    private float holdDuration;

    [SerializeField]
    private HoldEventReceiver holdEventReceiver;

    private float _holdDuration;

    private int _testCount;

    private float _totalDiff;
    
    private void Start()
    {
        holdEventReceiver.OnPointerDownAction = OnPointerDown;
        holdEventReceiver.OnPointerUpAction = OnPointerUp;
    }


    private void OnPointerDown(PointerEventData data)
    {
        _holdDuration = Time.realtimeSinceStartup;
    }

    private void OnPointerUp(PointerEventData data)
    {
        _totalDiff = holdDuration - (Time.realtimeSinceStartup - _holdDuration);
        _testCount++;
        if (_testCount >= testMaxCount)
        {
            _totalDiff /= (float) _testCount;
            Debug.Log($"Diff.Ave: {_totalDiff}");
            _testCount = 0;
        }
    }
}
