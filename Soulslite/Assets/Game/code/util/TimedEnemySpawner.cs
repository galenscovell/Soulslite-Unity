using UnityEngine;


public class TimedEnemySpawner : MonoBehaviour
{
    public EnemyRangedAgent entity;
    public int spawnTime;

    private float timer = 0;
    private float randomizedSpawnTime;


    private void Start()
    {
        ResetSpawnTimer();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= randomizedSpawnTime)
        {
            timer = 0;
            ResetSpawnTimer();

            EnemyRangedAgent enemy = Instantiate(entity);
            enemy.transform.position = transform.position;
            enemy.visionRange = 320;
        }
    }


    private void ResetSpawnTimer()
    {
        randomizedSpawnTime = Random.Range(spawnTime * 0.4f, spawnTime * 1.4f);
    }
}
