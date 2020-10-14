using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BitReciever : MonoBehaviour
{
    [SerializeField] private Text bitDataLog;
    [SerializeField] private float dataDuration = 0.1f;
    [SerializeField] private AccuracyTestParameter testParameter;
    [SerializeField] private Text progressLogText;

    private int[] bitData = new int[10];

    private bool dataReceving = false;

    private bool recieveHighBit = false;

    private int currentTestValue = 0;

    private int successCount = 0;

    private int testCount = 0;

    private StringBuilder logMsg = new StringBuilder();

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
        testCount++;
        yield return new WaitForSeconds(testParameter.TestDurationInSec);
        for (var i = 0; i < bitData.Length; i++)
        {
            bitData[i] = 0;
            yield return new WaitForSeconds(testParameter.TestDurationInSec);
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
        if (data == currentTestValue)
        {
            successCount++;
            logMsg.Append($"Success: {data}\n");
        } else
        {
            logMsg.Append($"Failed: {data}\n");
        }
        float acc = 0f;
        bool isFinished = false;
        if (successCount != 0)
        {
            acc = successCount / (float)testCount;
        }
        if (currentTestValue >= testParameter.TestCount)
        {
            logMsg.Append($"ACC: {acc}, Duration: {testParameter.TestDurationInSec}");
            Debug.Log(logMsg);
            logMsg.Clear();
            if (testParameter.UpdateParameter())
            {
                currentTestValue = 0;
                successCount = 0;
                testCount = 0;
                isFinished = true;
            }
        }
        if (isFinished == false)
        {
            progressLogText.text = $"currentTestValue: {currentTestValue}, data: {data}, acc: {acc}, duration: {testParameter.TestDurationInSec}";
            currentTestValue++;
        }
        yield return new WaitForSeconds(0.5f);
        dataReceving = false;
    }
}
