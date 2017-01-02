using UnityEngine;


public class CameraController : MonoBehaviour 
{
    private new Camera camera;
    private Vector3 playerOffset;
    private float dampTime = 0.2f;
    private Vector3 velocity = Vector3.zero;

    public int orthographicHeight = 120;
    public GameObject player;


    private void Awake()
    {
        camera = gameObject.GetComponent<Camera>();
        camera.orthographicSize = orthographicHeight;
    }

    private void Start()
    {
        playerOffset = transform.position - player.transform.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, player.transform.position + playerOffset, ref velocity, dampTime);
    }
}
