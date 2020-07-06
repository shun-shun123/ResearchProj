using UnityEngine;
using UnityEngine.UI;

public class UserInputManager : MonoBehaviour
{
    [SerializeField] private Button countButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Text countText;

    private int _count;

    private void Start()
    {
        Bind();   
    }

    private void Bind()
    {
        countButton.onClick.AddListener(CountUp);
        resetButton.onClick.AddListener(Reset);
    }

    private void CountUp()
    {
        _count++;
        countText.text = _count.ToString();
    }

    private void Reset()
    {
        _count = 0;
        countText.text = _count.ToString();
    }
}
