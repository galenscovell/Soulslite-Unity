using UnityEngine;


public class RainDropObject : MonoBehaviour
{
    private float defaultLifetime;
    private float currentLifetime;
    private int speed = 11;


    private void Awake()
    {
        defaultLifetime = Random.Range(0.1f, 0.3f);
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
                    transform.position.x - 2f,
                    transform.position.y - speed
                );
                currentLifetime -= Time.deltaTime;
            }
            // Otherwise mark it as ready for despawning
            else
            {
                RainSystem.rainSystem.DespawnRaindrop(gameObject);
            }
        }
	}
}
