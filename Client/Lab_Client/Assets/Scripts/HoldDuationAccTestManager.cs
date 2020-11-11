using System;
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

    private bool fixedLock = true;

    private void Start()
    {
        _diffRecords = new List<float>(testMaxCount);
        holdEventReceiver.OnPointerDownAction = OnPointerDown;
        holdEventReceiver.OnPointerUpAction = OnPointerUp;
    }

    private void FixedUpdate()
    {
        if (fixedLock)
        {
            return;
        }
        _holdDuration += Time.fixedDeltaTime;
    }


    private void OnPointerDown(PointerEventData data)
    {
        _holdDuration = 0f;
        fixedLock = false;
    }

    private void OnPointerUp(PointerEventData data)
    {
        fixedLock = true;
        _totalDiff += holdDuration - _holdDuration;
        _diffRecords.Add(holdDuration - _holdDuration);
        _testCount++;
        if (_testCount >= testMaxCount)
        {
            _totalDiff /= _testCount;
            Debug.Log($"Diff.Ave: {_totalDiff}");
            Debug.Log(GetAllLog());
            ResetAll();
        }

        fixedLock = false;
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
