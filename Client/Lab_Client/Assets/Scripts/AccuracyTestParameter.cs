using UnityEngine;

public class AccuracyTestParameter : MonoBehaviour
{
    public int TestCount;
    public float TestDurationInSec;
    public float TestMinDurationInSec;

    public bool UpdateParameter()
    {
        TestDurationInSec -= 0.01f;
        if (TestDurationInSec < TestMinDurationInSec)
        {
            return false;
        }
        return true;
    }
}
