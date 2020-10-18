using UnityEngine;

public class BitReceiverManager : MonoBehaviour
{
    [SerializeField] private float touchDurationInSec;

    private BitTouchReceiveModule _bitTouchReceiveModule;

    private void Start()
    {
        // bitReceiveModuleの生成
        _bitTouchReceiveModule = new BitTouchReceiveModule(this, touchDurationInSec, (data, bits) =>
        {
            Debug.Log($"Data: {data}\nbits: {bits}");
        });
    }

    public void OnClickReceiveButton()
    {
        _bitTouchReceiveModule.OnClickReceiveButtonAction();
    }

    private void OnValidate()
    {
        _bitTouchReceiveModule.TouchDurationInSec = touchDurationInSec;
    }
}
