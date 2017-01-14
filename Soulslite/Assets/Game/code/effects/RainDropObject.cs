using UnityEngine;


public class RainDropObject : MonoBehaviour
{
    public RainSystem rainSystem;

    private float defaultLifetime;
    private float currentLifetime;
    private float speed;


    private void Awake()
    {
        defaultLifetime = Random.Range(0.1f, 0.3f);
        speed = 12f;
    }

    private void OnEnable()
    {
        // Set starting lifetime to the default lifetime established on init
        currentLifetime = defaultLifetime;
    }

    private void Update()
    {
        // If raindrop is active
        if (gameObject.activeInHierarchy)
        {
            // And it hasn't finished its lifetime yet
            if (currentLifetime > 0)
            {
                // Move it downwards and slightly to the left
                transform.position = new Vector3(
                    transform.position.x - 3f,
                    transform.position.y - speed
                );
                currentLifetime -= Time.deltaTime;
            }
            // Otherwise mark it as ready for despawning
            else
            {
                rainSystem.DespawnRaindrop(gameObject);
            }
        }
	}
}
