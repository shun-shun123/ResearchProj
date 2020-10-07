using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BitReciever : MonoBehaviour
{
    [SerializeField] private Text bitDataLog;
    [SerializeField] private float dataDuration = 0.1f;

    private int[] bitData = new int[10];

    private bool dataReceving = false;

    private bool recieveHighBit = false;

    void Update()
    {
        
    }

    public void OnClickRecieveButton()
    {
        // データの受信スタート
        if (dataReceving == false)
        {
            dataReceving = true;
            StartCoroutine(DataRecieveCoroutine());
        } else
        {
            recieveHighBit = true;
        }
    }

    private IEnumerator DataRecieveCoroutine()
    {
        bitDataLog.text = "";
        yield return new WaitForSeconds(dataDuration);
        for (var i = 0; i < bitData.Length; i++)
        {
            bitData[i] = 0;
            yield return new WaitForSeconds(dataDuration);
            bitData[i] = recieveHighBit ? 1 : 0;
            bitDataLog.text = (recieveHighBit ? "1" : "0") + bitDataLog.text;
            recieveHighBit = false;
        }
        int data = 0;
        for (var i = 0; i < bitData.Length; i++)
        {
            data += bitData[i] * (1 << i);
        }
        bitDataLog.text += $": {data}";
        yield return new WaitForSeconds(0.5f);
        dataReceving = false;
    }
}
