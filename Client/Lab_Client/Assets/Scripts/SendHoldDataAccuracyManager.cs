using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class SendHoldDataAccuracyManager : MonoBehaviour
{
    [SerializeField] private HoldEventReceiver holdEventReceiver;

    [Header("Test Static Parameters")]
    [Tooltip("送信するビットデータの長さ")]
    [SerializeField] private int bitDataLength;

    [Header("Test Dynamic Parameters")] 
    [SerializeField] private float holdDurationInSec;
    [SerializeField] private float testMaxCount;
    [Tooltip("デバイスの処理速度によって発生する微妙な遅延を調整するための値")]
    [SerializeField] private float deviceDelayAdjustInSec;

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

    private StringBuilder sb = new StringBuilder();

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
            sb.Append($"OnPointerDown at {Time.time}\n");
        }
    }

    private void OnPointerUp(PointerEventData data)
    {
        Debug.Log("_isPressed Off");
        _isPressing = false;
        sb.Append($"OnPointerUp at {Time.time}\n");
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
            yield return new WaitForSeconds(holdDurationInSec + deviceDelayAdjustInSec);
            _bitData[i] = _isPressing ? 1 : 0;
        }
        _isDataReceiving = false;
        Debug.Log("Data Receiving Stop.");
        LogBitToInt();
        Debug.Log(sb.ToString());
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
