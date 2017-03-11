using UnityEngine;


public class DashTrailObject : MonoBehaviour
{
    private Vector2 position;

    private float displayTime;
    private float timeDisplayed;
    private bool inUse;

    public SpriteRenderer spriteRenderer;

    private Color color;


    private void Update()
    {
        if (inUse)
        {
            transform.position = position;
            spriteRenderer.color = color;

            timeDisplayed -= Time.deltaTime;
            color.a = timeDisplayed / displayTime;
                
            if (timeDisplayed <= 0)
            {
                TrailSystem.trailSystem.RemoveTrailObject(this);
                inUse = false;
            }
        }
    }

    public void SetActive(bool setting)
    {
        gameObject.SetActive(setting);
    }

    public void Initiate(float time, Sprite sprite, Vector2 pos, Color c)
    {
        displayTime = time;
        spriteRenderer.sprite = sprite;
        position = pos;
        timeDisplayed = time;
        color = c;
        inUse = true;
    }
}
