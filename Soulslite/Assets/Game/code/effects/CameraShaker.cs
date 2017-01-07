using UnityEngine;


public class CameraShaker : MonoBehaviour
{
    private float shakeAmt = 0;

    public Camera mainCamera;


    public void Activate()
    {
        shakeAmt = 100 * .02f;
        InvokeRepeating("CameraShake", 0, .01f);
        Invoke("StopShaking", 0.2f);
    }

    private void CameraShake()
    {
        if (shakeAmt > 0)
        {
            float quakeAmt = Random.value * shakeAmt * 2 - shakeAmt;
            Vector3 pp = mainCamera.transform.position;
            pp.y += quakeAmt;
            pp.x += quakeAmt;
            mainCamera.transform.position = pp;
        }
    }

    private void StopShaking()
    {
        CancelInvoke("CameraShake");
    }
}
