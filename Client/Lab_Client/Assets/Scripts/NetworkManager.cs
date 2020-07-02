using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    private const string URL = "http://127.0.0.1:5000";
    
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
}
