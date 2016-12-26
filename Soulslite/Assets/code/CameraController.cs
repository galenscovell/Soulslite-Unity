using UnityEngine;


public class CameraController : MonoBehaviour 
{
    private new Camera camera;
    private Vector3 playerOffset;
    private float dampTime = 0.2f;
    private Vector3 velocity = Vector3.zero;

    public int orthographicHeight = 120;
    public GameObject player;


    void Awake() 
    {
        camera = gameObject.GetComponent<Camera>();
        camera.orthographicSize = orthographicHeight;
    }

    void Start() 
    {
        playerOffset = transform.position - player.transform.position;
    }

    void LateUpdate() 
    {
        transform.position = Vector3.SmoothDamp(transform.position, player.transform.position + playerOffset, ref velocity, dampTime);
    }
}
