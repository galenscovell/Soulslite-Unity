using UnityEngine;


public class RainSplashObject : MonoBehaviour
{
    public RainSystem rainSystem;

    private float defaultLifetime;
    private float currentLifetime;


    private void Awake()
    {
        defaultLifetime = 0.4f;
    }

    private void OnEnable()
    {
        // Set starting lifetime to default lifetime established on init
        currentLifetime = defaultLifetime;
    }

    private void Update()
    {
        // If particle system is active
        if (gameObject.activeInHierarchy)
        {
            // And lifetime has expired
            if (currentLifetime < 0)
            {
                // Mark particle system as ready for despawning
                rainSystem.DespawnParticle(gameObject);
            }
            // Otherwise decrement lifetime
            else
            {
                currentLifetime -= Time.deltaTime;
            }
        }
	}
}
