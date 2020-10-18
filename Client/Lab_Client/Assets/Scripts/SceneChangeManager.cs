using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public void GoToTestScene()
    {
        SceneManager.LoadScene("TestSendTouchDataAccuracy");
    }

    public void GoToSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void GoToBitReceiveScene()
    {
        SceneManager.LoadScene("BitReceive");
    }

    public void GoToTouchDurationRapidTestScene()
    {
        SceneManager.LoadScene("TouchDurationRapidTest");
    }
}
