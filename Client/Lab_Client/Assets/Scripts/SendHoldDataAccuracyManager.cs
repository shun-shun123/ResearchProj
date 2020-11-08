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
    [SerializeField] private float holdDurationInMillis;
    [SerializeField] private float testMaxCount;
    [Tooltip("デバイスの処理速度によって発生する微妙な遅延を調整するための値")]
    [SerializeField] private float deviceDelayAdjustInMillis;

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
        
        // millis→secへ変換する(コルーチンの待機はsecなので）
        holdDurationInMillis /= 1000.0f;
        deviceDelayAdjustInMillis /= 1000.0f;

    }

    public void OnClickReceivingButton()
    {
        if (_isDataReceiving)
        {
            Debug.Log("Data receiving... not working");
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
            Debug.Log("_isPressed On");
            _isPressing = true;
        }
    }

    private void OnPointerUp(PointerEventData data)
    {
        Debug.Log("_isPressed Off");
        _isPressing = false;
    }

    /// <summary>
    /// データ送信開始ビットを受け取ったら起動するコルーチン
    /// 10bitを受け取る時間が経過したら、_isDataReceivingフラグをOFFにする
    /// </summary>
    private IEnumerator DataReceivingCoroutine()
    {
        Debug.Log("Data Receiving Start.");
        _isDataReceiving = true;
        for (var i = 0; i < _bitData.Length; i++)
        {
            yield return new WaitForSeconds(holdDurationInMillis + deviceDelayAdjustInMillis);
            _bitData[i] = _isPressing ? 1 : 0;
        }
        _isDataReceiving = false;
        Debug.Log("Data Receiving Stop.");
        LogBitToInt();
    }

    private void LogBitToInt()
    {
        int num = 0;
        for (var i = 0; i < _bitData.Length; i++)
        {
            num += _bitData[i] << i;
        }
        Debug.Log($"num: {num}");
    }
}
