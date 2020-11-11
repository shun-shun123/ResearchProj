using System.Collections.Generic;
using System.Text;
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

    private List<float> _diffRecords;

    private int _testCount;

    private float _totalDiff;
    
    private void Start()
    {
        _diffRecords = new List<float>(testMaxCount);
        holdEventReceiver.OnPointerDownAction = OnPointerDown;
        holdEventReceiver.OnPointerUpAction = OnPointerUp;
    }


    private void OnPointerDown(PointerEventData data)
    {
        _holdDuration = Time.realtimeSinceStartup;
    }

    private void OnPointerUp(PointerEventData data)
    {
        var diff = Time.realtimeSinceStartup - _holdDuration;
        _totalDiff += holdDuration - diff;
        _diffRecords.Add(holdDuration - diff);
        _testCount++;
        if (_testCount >= testMaxCount)
        {
            _totalDiff /= _testCount;
            Debug.Log($"Diff.Ave: {_totalDiff}");
            Debug.Log(GetAllLog());
            ResetAll();
        }
    }

    private string GetAllLog()
    {
        StringBuilder sb =new StringBuilder();
        for (var i = 0; i < _diffRecords.Count; i++)
        {
            sb.Append($"{i}: {_diffRecords[i] * 1000.0f}(ms)\n");
        }

        return sb.ToString();
    }

    private void ResetAll()
    {
        _diffRecords.Clear();
        _testCount = 0;
        _totalDiff = 0f;
    }
}
