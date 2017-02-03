using UnityEngine;


public class TransitionZone : MonoBehaviour
{
    public string connectingTransition;
    public int nextSectionIndex;

    private BoxCollider2D bounds;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            LevelManager.levelManager.transitionToSection(nextSectionIndex, connectingTransition);
        }
    }

    private void OnEnable()
    {
        bounds = GetComponent<BoxCollider2D>();
    }

    public Vector2 GetZoneCenter()
    {
        return bounds.offset;
    }
}
