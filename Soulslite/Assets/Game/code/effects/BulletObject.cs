using UnityEngine;


public class BulletObject : MonoBehaviour
{
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private int speed = 300;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Sprite s, string tag, int layer)
    {
        spriteRenderer.sprite = s;
        gameObject.tag = tag;
        gameObject.layer = layer;
    }

    public void SetActive(bool setting)
    {
        gameObject.SetActive(setting);
    }

    private void OnEnable()
    {
        Vector2 direction = BulletSystem.bulletSystem.GetNextBulletDirection();
        body.velocity = direction * speed;
    }

    private void OnDisable()
    {
        body.velocity = Vector2.zero;
        transform.right = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore collisions with critters
        if (collision.tag == "CritterTag")
        {
            return;
        }

        Vector2 collisionDirection = transform.position - collision.transform.position;
        BulletSystem.bulletSystem.DespawnBullet(gameObject, collisionDirection, collision.name);
    }
}
