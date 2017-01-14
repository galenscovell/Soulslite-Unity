using System.Collections.Generic;
using UnityEngine;


public class BloodSystem : MonoBehaviour
{
    public static BloodSystem bloodSystem;

    public GameObject bloodSplashObject;

    private List<GameObject> bloodParticleObjects;
    private int spawnedBloodObjects;
    private int bloodObjectIndex;
    private int maxBloodObjects = 30;


    private void Awake()
    {
        // Singleton
        if (bloodSystem != null) Destroy(bloodSystem);
        else bloodSystem = this;

        DontDestroyOnLoad(this);
    }

    private void Start () {
        // Create an object pool for blood particle objects
        bloodParticleObjects = new List<GameObject>();

        for (int i = 0; i < maxBloodObjects; i++)
        {
            GameObject bloodObj = Instantiate(bloodSplashObject);
            bloodObj.SetActive(false);
            bloodObj.transform.parent = transform;
            bloodParticleObjects.Add(bloodObj);
        }

        bloodObjectIndex = 0;
    }

    public void SpawnBloodObject(Vector2 position, Vector2 direction)
    {
        // Spawn blood effect at position moving in a direction
        if (spawnedBloodObjects < maxBloodObjects)
        {
            // Ensure object index is within pool size
            if (bloodObjectIndex >= maxBloodObjects) bloodObjectIndex = 0;

            // Pull out an object, put it under parent object and mark it active
            GameObject nextObj = bloodParticleObjects[bloodObjectIndex];

            // Rotate blood to spray in collision direction and set it to target position
            nextObj.transform.rotation = Quaternion.LookRotation(direction);
            nextObj.transform.position = position;
            nextObj.SetActive(true);

            spawnedBloodObjects++;
            bloodObjectIndex++;
        }

    }

    public void DespawnBloodObject(GameObject gameObj)
    {
        // Disable object
        gameObj.SetActive(false);
        spawnedBloodObjects--;
        bloodObjectIndex++;
    }
}
