using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SendHoldDataAccuracyManager : MonoBehaviour
{
    [SerializeField] private HoldEventReceiver holdEventReceiver;

    [Header("Test Static Parameters")]
    [Tooltip("送信するビットデータの長さ")]
    [SerializeField] private int bitDataLength;

    [Header("Test Dynamic Parameters")] 
    [SerializeField] private float holdDuration;
    [SerializeField] private float testMaxCount;

    private int[] _bitData;

    private bool _isPressing;

    /// <summary>
    /// データ受付中に走るコルーチンのキャッシュ
    /// </summary>
    private IEnumerator _dataReceivingCoroutine;

    /// <summary>
    /// データの受信中フラグ
    /// </summary>
    private bool _isDataReceiving;

    private void Start()
    {
        _bitData = new int[bitDataLength];
        holdEventReceiver.OnPointerDownAction = OnPointerDown;
        holdEventReceiver.OnPointerUpAction = OnPointerUp;
    }

    public void OnClickReceivingButton()
    {
        if (_isDataReceiving)
        {
            return;
        }

        if (_dataReceivingCoroutine != null)
        {
            StopCoroutine(_dataReceivingCoroutine);
            _dataReceivingCoroutine = null;
        }

        _dataReceivingCoroutine = DataReceivingCoroutine();
        StartCoroutine(_dataReceivingCoroutine);
    }

    private void OnPointerDown(PointerEventData data)
    {
        if (_isDataReceiving)
        {
            _isPressing = true;
        }
    }

    private void OnPointerUp(PointerEventData data)
    {
        _isPressing = false;
    }

    /// <summary>
    /// データ送信開始ビットを受け取ったら起動するコルーチン
    /// 10bitを受け取る時間が経過したら、_isDataReceivingフラグをOFFにする
    /// </summary>
    private IEnumerator DataReceivingCoroutine()
    {
        _isDataReceiving = true;
        for (var i = 0; i < _bitData.Length; i++)
        {
            yield return new WaitForSeconds(holdDuration);
            _bitData[i] = _isPressing ? 1 : 0;
        }
        _isDataReceiving = false;
        LogBitToInt();
    }

    private void LogBitToInt()
    {
        int num = 0;
        for (var i = 0; i < _bitData.Length; i++)
        {
            num += _bitData[i] << i;
        }
        Debug.Log(num);
    }
}
