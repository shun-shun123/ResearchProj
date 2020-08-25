using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private Text m_TextLog;
    
#if UNITY_EDITOR
    private const string URL = "http://127.0.0.1:5000";
#else
    private const string URL = "http://192.168.100.25:5000";
#endif
    

    private string m_ReceivedCommand;

    public IEnumerator On()
    {
        using (var req = UnityWebRequest.Get(URL + "/on"))
        {
            yield return req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError("NetworkError");
            }
        }   
    }

    public IEnumerator Off()
    {
        using (var req = UnityWebRequest.Get(URL + "/off"))
        {
            yield return req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError("NetworkError");
            }
        }
    }

    private void Start()
    {
        Observable
            .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
            .Where(_ => string.IsNullOrEmpty(m_ReceivedCommand) == false)
            .Subscribe(_ =>
            {
                StartCoroutine(Command(m_ReceivedCommand));
                switch (m_ReceivedCommand[0])
                {
                    case '1':
                        m_TextLog.text = "SingleTouch";
                        break;
                    case '2':
                        m_TextLog.text = "DoubleTouch";
                        break;
                    case '3':
                        m_TextLog.text = "Hold";
                        break;
                    case '4':
                        break;
                    case '5':
                        m_TextLog.text = "Scroll";
                        break;
                    case '6':
                        m_TextLog.text = "Pinch-In";
                        break;
                    case '7':
                        m_TextLog.text = "Pinch-Out";
                        break;
                    default:
                        m_TextLog.text = "Not Found";
                        break;
                }
                m_ReceivedCommand = null;
            }).AddTo(gameObject);
    }

    public IEnumerator Command(string cmd)
    {
        Debug.Log($"Network.Command: {cmd[0]}");
        using (var req = UnityWebRequest.Get(URL + "/command?cmd=" + cmd))
        {
            yield return req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError("NetworkError");
            }
        }
    }

    /// <summary>
    /// 外部からコマンドをセットすることができる
    /// </summary>
    /// <param name="cmd"></param>
    public void SetCommand(string cmd) => m_ReceivedCommand = cmd;
}
