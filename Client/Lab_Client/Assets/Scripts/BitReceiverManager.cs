using System.Collections;
using UnityEngine;

public class BitReceiverManager : MonoBehaviour
{
    [SerializeField] private float touchDurationInSec;

    private readonly int[] _bitData = new int[10];

    private bool _dataReceiving = false;

    private bool _receiveHighBit = false;

    private int currentTestValue = 0;

    public void OnClickReceiveButton()
    {
        // データの受信スタート
        if (_dataReceiving == false)
        {
            _dataReceiving = true;
            StartCoroutine(DataReceiveCoroutine());
        } else
        {
            _receiveHighBit = true;
        }
    }

    private IEnumerator DataReceiveCoroutine()
    {
        // データ送信開始タッチのあとtouchDurationInSec分待機時間が発生するためそれを待つ
        yield return new WaitForSeconds(touchDurationInSec);
        
        // 10bitのデータ受信開始
        for (var i = 0; i < _bitData.Length; i++)
        {
            // 押下→離すまでtouchDurationInSecあって、その後さらにtouchDurationInSec待機するので、
            yield return new WaitForSeconds(touchDurationInSec * 2f);
            _bitData[i] = _receiveHighBit ? 1 : 0;
            _receiveHighBit = false;
        }
        int data = 0;
        for (var i = 0; i < _bitData.Length; i++)
        {
            data += _bitData[i] << i;
        }
        Debug.Log(data);
        yield return new WaitForSeconds(0.5f);
        _dataReceiving = false;
    }
}
