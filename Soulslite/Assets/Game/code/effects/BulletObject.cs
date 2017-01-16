using UnityEngine;


public class BulletObject : MonoBehaviour
{
    private Rigidbody2D body;
    private int speed = 900;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        Vector2 direction = BulletSystem.bulletSystem.GetNextBulletDirection();
        body.velocity = direction * speed;
        transform.right = direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 collisionDirection = collision.transform.position - transform.position;
        BulletSystem.bulletSystem.DespawnBullet(gameObject, collisionDirection);
    }
}
