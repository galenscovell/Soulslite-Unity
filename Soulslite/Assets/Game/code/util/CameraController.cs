using UnityEngine;


public class CameraController : MonoBehaviour 
{
    public static CameraController cameraController;

    public float initialDampTime;
    public int orthographicHeight = 120;
    public GameObject vignette;
    public GameObject blackFade;
    public EdgeCollider2D levelBounds;

    private new Camera camera;
    private GameObject targetObject;
    private Rigidbody2D targetBody;
    private Vector3 cameraOffset = new Vector3(0, 0, -10);
    private Vector3 velocity = Vector3.zero;
    private float dampTime;
    private float shakeAmt = 0;
    private SpriteRenderer vignetteRenderer;
    private Vector2 minPosition;
    private Vector2 maxPosition;


    private void Awake()
    {
        camera = gameObject.GetComponent<Camera>();
        camera.orthographicSize = orthographicHeight;

        dampTime = initialDampTime;

        // Singleton
        if (cameraController != null) Destroy(cameraController);
        else cameraController = this;
        DontDestroyOnLoad(this);

        vignetteRenderer = vignette.GetComponent<SpriteRenderer>();

        FadeOutToBlack(0);
        FadeInFromBlack(5);

        minPosition = levelBounds.bounds.min;
        maxPosition = levelBounds.bounds.max;
    }

    private void LateUpdate()
    {
        Vector3 targetCenter = targetObject.transform.position;
        if (targetObject.name == "player")
        {
            Vector2 bodyVelocity = targetBody.velocity.normalized * 60;
            targetCenter.x += bodyVelocity.x;
            targetCenter.y += bodyVelocity.y;
        }

        Vector3 focusPosition = Vector3.SmoothDamp(transform.position, targetCenter + cameraOffset, ref velocity, dampTime);
        focusPosition = Vector3.Min(focusPosition, maxPosition);
        focusPosition = Vector3.Max(focusPosition, minPosition);

        transform.position = focusPosition + Vector3.forward * -10;
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


    /**************************
     *        Vignette        *
     **************************/
    public void FadeOutVignette(float overTime)
    {
        LeanTween.alpha(vignette, 0, overTime);
    }

    public void FadeInVignette(Color c, float overTime)
    {
        vignetteRenderer.color = new Color(c.r, c.g, c.b, 0);
        LeanTween.alpha(vignette, 1, overTime);
    }


    /**************************
     *          Fade          *
     **************************/
    public void FadeOutToBlack(float overTime)
    {
        LeanTween.alpha(blackFade, 1, overTime);
    }

    public void FadeInFromBlack(float overTime)
    {
        LeanTween.alpha(blackFade, 0, overTime);
    }
}
