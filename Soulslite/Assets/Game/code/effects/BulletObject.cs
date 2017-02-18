using UnityEngine;


public class BulletObject : MonoBehaviour
{
    private Rigidbody2D body;
    private int speed = 300;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
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
        Vector2 collisionDirection = transform.position - collision.transform.position;
        BulletSystem.bulletSystem.DespawnBullet(gameObject, collisionDirection);
    }
}
