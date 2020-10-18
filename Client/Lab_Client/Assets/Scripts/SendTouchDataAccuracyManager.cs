using UnityEngine;

public class SendTouchDataAccuracyManager : MonoBehaviour
{
    [SerializeField] private float touchDurationInSec;
    [SerializeField] private int testMaxCount;

    private BitTouchReceiveModule _bitTouchReceiveModule;

    private int _currentTestCount;
    
    void Start()
    {
        _bitTouchReceiveModule = new BitTouchReceiveModule(this, touchDurationInSec, OnBitDataReceivedAction);
    }

    public void OnClickReceiveButton()
    {
        _bitTouchReceiveModule.OnClickReceiveButtonAction();
    }

    private void OnBitDataReceivedAction(int data, int[] bits)
    {
        if (data == _currentTestCount)
        {
            Debug.Log("TRUE");
        }
        else
        {
            Debug.LogError($"FALSE: data: {data}, currentTestCount: {_currentTestCount}");
        }

        _currentTestCount++;
        if (testMaxCount == _currentTestCount)
        {
            Debug.Log("TEST FINISHED");
        }
    }

    private void OnValidate()
    {
        if (_bitTouchReceiveModule != null)
        {
            _bitTouchReceiveModule.TouchDurationInSec = touchDurationInSec;
        }
    }
}
