using Tiled2Unity;
using UnityEngine;


public class LevelSection : MonoBehaviour
{
    private TiledMap tileMap;
    private EdgeCollider2D cameraBounds;

    private Vector2 entrancePoint;
    private BoxCollider2D exitZone;


    public TiledMap GetTileMap()
    {
        return tileMap;
    }

    public EdgeCollider2D GetCameraBounds()
    {
        return cameraBounds;
    }

    public Vector2 GetEntrancePosition()
    {
        return entrancePoint;
    }

    public Vector2 GetExitPosition()
    {
        return exitZone.offset;
    }

    public void Enable()
    {
        gameObject.SetActive(true);

        tileMap = GetComponent<TiledMap>();
        cameraBounds = GameObject.Find("CameraBounds").GetComponent<EdgeCollider2D>();
        entrancePoint = GameObject.Find("EntrancePoint").transform.position;
        exitZone = GameObject.Find("ExitZone").GetComponent<BoxCollider2D>();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
