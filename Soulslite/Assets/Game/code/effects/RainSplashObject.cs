using UnityEngine;


public class RainSplashObject : MonoBehaviour
{
    private ParticleSystem pSystem;


    private void Awake()
    {
        pSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy && !pSystem.IsAlive())
        {
            RainSystem.rainSystem.DespawnParticle(gameObject);
        }
	}
}
