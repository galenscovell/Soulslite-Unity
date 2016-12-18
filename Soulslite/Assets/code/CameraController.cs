using UnityEngine;


public class CameraController : MonoBehaviour 
{
    private new Camera camera;
    private Vector3 playerOffset;

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
        transform.position = player.transform.position + playerOffset;
    }
}
