using UnityEngine;


public class TransitionZone : MonoBehaviour
{
    public string connectingTransition;
    public string nextSceneName;

    private BoxCollider2D bounds;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            LevelManager.levelManager.BeginTransition(nextSceneName, connectingTransition);
        }
    }

    private void OnEnable()
    {
        bounds = GetComponent<BoxCollider2D>();
    }

    public Vector2 GetZoneCenter()
    {
        Vector2 center = new Vector2(
            transform.position.x + (bounds.size.x / 2),
            transform.position.y + (bounds.size.y / 2)
        );
        return center;
    }
}
