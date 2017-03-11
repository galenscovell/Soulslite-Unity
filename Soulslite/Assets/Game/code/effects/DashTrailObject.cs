using UnityEngine;


public class DashTrailObject : MonoBehaviour
{
    private Vector2 position;

    private float displayTime;
    private float timeDisplayed;
    private bool inUse;

    private float blinkTimer;

    public SpriteRenderer spriteRenderer;

    private Color color;


    private void Update()
    {
        if (inUse)
        {
            transform.position = position;
            spriteRenderer.color = color;

            timeDisplayed -= Time.deltaTime;
            blinkTimer += Time.deltaTime;

            if (blinkTimer > 0.2f)
            {
                color.a = 0;
                blinkTimer = 0;
            }
            else
            {
                color.a = timeDisplayed / displayTime;
            }
                
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
        blinkTimer = 0;
        color = c;
        inUse = true;
    }
}
