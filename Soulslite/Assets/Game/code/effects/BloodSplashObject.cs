﻿using UnityEngine;


public class BloodSplashObject : MonoBehaviour
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
            BloodSystem.bloodSystem.DespawnBlood(gameObject);
        }
    }
}
