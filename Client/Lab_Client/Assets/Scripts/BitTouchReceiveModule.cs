using System;
using System.Collections;
using UnityEngine;

public class BitTouchReceiveModule
{
    public float TouchDurationInSec { get; set; }

    private readonly int[] _bitData = new int[10];

    private bool _dataReceiving = false;

    private bool _receiveHighBit = false;

    private readonly Action<int, int[]> _onBitDataReceivedAction;

    private readonly MonoBehaviour _coroutineExecutor;

    public BitTouchReceiveModule(MonoBehaviour mb, float touchDurationInSec, Action<int, int[]> onBitDataReceivedAction)
    {
        _coroutineExecutor = mb;
        TouchDurationInSec = touchDurationInSec;
        _onBitDataReceivedAction = onBitDataReceivedAction;
    }

    public void OnClickReceiveButtonAction()
    {
        if (_dataReceiving == false)
        {
            _dataReceiving = true;
            _coroutineExecutor.StartCoroutine(DataReceiveCoroutine());
        }
        else
        {
            _receiveHighBit = true;
        }
    }
    
    private IEnumerator DataReceiveCoroutine()
    {
        // データ送信開始タッチのあとtouchDurationInSec分待機時間が発生するためそれを待つ
        yield return new WaitForSeconds(TouchDurationInSec);
        
        // 10bitのデータ受信開始
        for (var i = 0; i < _bitData.Length; i++)
        {
            // 押下→離すまでtouchDurationInSecあって、その後さらにtouchDurationInSec待機するので、
            yield return new WaitForSeconds(TouchDurationInSec * 2f);
            _bitData[i] = _receiveHighBit ? 1 : 0;
            _receiveHighBit = false;
        }
        int data = 0;
        for (var i = 0; i < _bitData.Length; i++)
        {
            data += _bitData[i] << i;
        }

        _onBitDataReceivedAction(data, _bitData);
        yield return new WaitForSeconds(0.5f);
        _dataReceiving = false;
    }
}
