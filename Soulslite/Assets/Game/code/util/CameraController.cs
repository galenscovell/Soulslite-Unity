using UnityEngine;


public class CameraController : MonoBehaviour 
{
    public static CameraController cameraController;

    public float dampTime;
    public int orthographicHeight = 120;

    private new Camera camera;
    private GameObject targetObject;
    private Vector3 cameraOffset = new Vector3(0, 0, -10);
    private Vector3 velocity = Vector3.zero;
    private float shakeAmt = 0;


    private void Awake()
    {
        camera = gameObject.GetComponent<Camera>();
        camera.orthographicSize = orthographicHeight;

        // Singleton
        if (cameraController != null) Destroy(cameraController);
        else cameraController = this;
        DontDestroyOnLoad(this);
    }

    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetObject.transform.position + cameraOffset, ref velocity, dampTime);

        // transform.position = Vector3.SmoothDamp(transform.position, targetObject.transform.position + cameraOffset, ref velocity, dampTime);
    }

    public void ChangeTarget(GameObject target)
    {
        targetObject = target;
        // cameraOffset = transform.position = targetObject.transform.position;
    }


    /**************************
     *         Shake          *
     **************************/
    public void ActivateShake()
    {
        shakeAmt = 100 * .02f;
        InvokeRepeating("CameraShake", 0, .01f);
        Invoke("DeactivateShake", 0.2f);
    }

    private void CameraShake()
    {
        if (shakeAmt > 0)
        {
            float quakeAmt = Random.value * shakeAmt * 2 - shakeAmt;
            Vector3 pp = camera.transform.position;
            pp.y += quakeAmt;
            pp.x += quakeAmt;
            camera.transform.position = pp;
        }
    }

    private void DeactivateShake()
    {
        CancelInvoke("CameraShake");
    }
}
