using UnityEngine;


public class CameraController : MonoBehaviour 
{
    public static CameraController cameraController;

    public float initialDampTime;
    public int orthographicHeight = 120;

    private new Camera camera;
    private GameObject targetObject;
    private Rigidbody2D targetBody;
    private Vector3 cameraOffset = new Vector3(0, 0, -10);
    private Vector3 velocity = Vector3.zero;
    private float dampTime;
    private float shakeAmt = 0;


    private void Awake()
    {
        camera = gameObject.GetComponent<Camera>();
        camera.orthographicSize = orthographicHeight;

        dampTime = initialDampTime;

        // Singleton
        if (cameraController != null) Destroy(cameraController);
        else cameraController = this;
        DontDestroyOnLoad(this);
    }

    private void LateUpdate()
    {
        Vector3 targetCenter = targetObject.transform.position;
        if (targetBody != null)
        {
            Vector2 bodyVelocity = targetBody.velocity.normalized * 20;
            targetCenter.x += bodyVelocity.x;
            targetCenter.y += bodyVelocity.y;
        }
        transform.position = Vector3.SmoothDamp(transform.position, targetCenter + cameraOffset, ref velocity, dampTime);
    }


    /**************************
     *         Modify         *
     **************************/
    public void ChangeTarget(GameObject target)
    {
        targetObject = target;
        targetBody = targetObject.GetComponent<Rigidbody2D>();
    }

    public void SetDampTime(float time)
    {
        dampTime = time;
    }

    public void RestoreDefaultDampTime()
    {
        dampTime = initialDampTime;
    }


    /**************************
     *         Shake          *
     **************************/
    public void ActivateShake(int magnitude, float length)
    {
        shakeAmt = 100 * (magnitude / 100f);
        InvokeRepeating("CameraShake", 0, 0.01f);
        Invoke("DeactivateShake", length);
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
