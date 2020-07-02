using UnityEngine;

public class UserInputManager : MonoBehaviour
{

    [SerializeField] private NetworkManager m_NetworkManager;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(m_NetworkManager.On());
        } else if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(m_NetworkManager.Off());
        }
    }
}
