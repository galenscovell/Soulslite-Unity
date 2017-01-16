using UnityEngine;


public class BulletCollisionObject : MonoBehaviour
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
