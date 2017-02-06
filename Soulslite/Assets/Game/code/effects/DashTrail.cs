using System.Collections.Generic;
using UnityEngine;


public class DashTrail : MonoBehaviour
{
    private List<DashTrailObject> trailObjectsInUse;
    private Queue<DashTrailObject> trailObjectsNotInUse;

    private float spawnInterval;
    private float spawnTimer;
    private bool on;

    public SpriteRenderer leadingSprite;
    public DashTrailObject trailObject;
    public int segments;
    public float time;


    private void Start()
    {
        DontDestroyOnLoad(this);

        spawnInterval = time / segments / 2;
        trailObjectsInUse = new List<DashTrailObject>();
        trailObjectsNotInUse = new Queue<DashTrailObject>();

        for (int i = 0; i < segments; i++)
        {
            DashTrailObject trail = Instantiate(trailObject);
            trail.transform.SetParent(transform);
            trailObjectsNotInUse.Enqueue(trail);
        }

        on = false;
    }

    private void Update()
    {
        if (on)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                DashTrailObject trail = null;
                if (trailObjectsNotInUse.Count > 0)
                {
                    trail = trailObjectsNotInUse.Dequeue();
                }

                if (trail != null)
                {
                    DashTrailObject trailObject = trail.GetComponent<DashTrailObject>();
                    trailObject.Initiate(time, leadingSprite.sprite, transform.position, this);
                    trailObjectsInUse.Add(trail);
                    spawnTimer = 0;
                }
            }
        }
    }

    public void RemoveTrailObject(DashTrailObject obj)
    {
        trailObjectsInUse.Remove(obj);
        trailObjectsNotInUse.Enqueue(obj);
    }

    public void SetEnabled(bool setting)
    {
        on = setting;
        if (setting) spawnTimer = spawnInterval;
    }

}