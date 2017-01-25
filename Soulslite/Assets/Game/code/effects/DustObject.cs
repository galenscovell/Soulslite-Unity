using UnityEngine;


public class DustObject : MonoBehaviour
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
            DustSystem.dustSystem.DespawnDust(gameObject);
        }
    }
}
