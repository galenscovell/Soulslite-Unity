using System.Collections;
using UnityEngine;


public class TimeSystem : MonoBehaviour
{
    public static TimeSystem timeSystem;


    private void Awake()
    {
        if (timeSystem != null) Destroy(timeSystem);
        else timeSystem = this;
        DontDestroyOnLoad(this);
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }

    public void SlowTime(float timeScale, float forTime)
    {
        StartCoroutine(slowTime(timeScale, forTime));
    }

    private IEnumerator slowTime(float timeScale, float forTime)
    {
        float originalTimeScale = Time.timeScale;

        Time.timeScale = timeScale;

        yield return new WaitForSeconds(forTime);

        Time.timeScale = originalTimeScale;
    }
}
