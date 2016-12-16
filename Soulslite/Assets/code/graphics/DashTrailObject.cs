using UnityEngine;


public class DashTrailObject : MonoBehaviour
{
    private DashTrail spawner;
    private Vector2 position;

    private float displayTime;
    private float timeDisplayed;
    private bool inUse;

    public SpriteRenderer spriteRenderer;
    public Color startColor, endColor;


    private void Start()
    {
        spriteRenderer.enabled = false;
    }

    private void Update()
    {
        if (inUse)
        {
            transform.position = position;
            timeDisplayed += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, endColor, timeDisplayed / displayTime);

            if (timeDisplayed >= displayTime)
            {
                spawner.RemoveTrailObject(this);
                inUse = false;
                spriteRenderer.enabled = false;
            }
        }
    }

    public void Initiate(float time, Sprite sprite, Vector2 pos, DashTrail trail)
    {
        displayTime = time;
        spriteRenderer.sprite = sprite;
        spriteRenderer.enabled = true;
        position = pos;
        timeDisplayed = 0;
        spawner = trail;
        inUse = true;
    }
}
