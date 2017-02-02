using UnityEngine;


public class TransitionZone : MonoBehaviour
{
    public Vector2 transitionDirection;
    public int nextSectionIndex;


    private void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            LevelManager.levelManager.transitionToSection(nextSectionIndex, transitionDirection);
        }
    }
}
