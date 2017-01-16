using UnityEngine;


public class BulletFireObject : MonoBehaviour
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
            BulletSystem.bulletSystem.DespawnParticle(gameObject);
        }
    }
}
