using UnityEngine;


public class SpawnTrigger : MonoBehaviour
{
    public GameObject entity;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Activate given entity when player triggers this collider
        entity.SetActive(true);

        Destroy(gameObject);
    }
}
