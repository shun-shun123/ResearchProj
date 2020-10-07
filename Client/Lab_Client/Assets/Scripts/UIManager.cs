using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text countText;
    private int _count;
    
    public void GoToTestScene()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void GoToSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void GoToBitRecieveScene()
    {
        SceneManager.LoadScene("BitRecieve");
    }

    public void CountUp()
    {
        _count++;
        countText.text = _count.ToString();
    }
}
