using UnityEngine;


public class CameraShaker : MonoBehaviour
{
    private float shakeAmt = 0;

    public Camera mainCamera;


    private void OnTriggerEnter2D(Collider2D collider)
    {
        shakeAmt = 100 * .01f;
        InvokeRepeating("CameraShake", 0, .01f);
        Invoke("StopShaking", 0.3f);
    }

    private void CameraShake()
    {
        if (shakeAmt > 0)
        {
            float quakeAmt = Random.value * shakeAmt * 2 - shakeAmt;
            Vector3 pp = mainCamera.transform.position;
            pp.y += quakeAmt; // can also add to x and/or z
            pp.x += quakeAmt;
            mainCamera.transform.position = pp;
        }
    }

    private void StopShaking()
    {
        CancelInvoke("CameraShake");
    }
}
