using UnityEngine;


public class CameraController : MonoBehaviour 
{
    public static CameraController cameraController;

    private new Camera camera;
    private Vector3 playerOffset;
    private Vector3 velocity = Vector3.zero;
    private float shakeAmt = 0;

    public float dampTime = 0.2f;
    public int orthographicHeight = 120;
    public GameObject player;


    private void Awake()
    {
        camera = gameObject.GetComponent<Camera>();
        camera.orthographicSize = orthographicHeight;

        // Singleton
        if (cameraController != null) Destroy(cameraController);
        else cameraController = this;

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        playerOffset = transform.position - player.transform.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, player.transform.position + playerOffset, ref velocity, dampTime);
    }


    /****************
     * SHAKE
     ****************/
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
