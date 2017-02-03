using System.Collections.Generic;
using System.Linq;
using Tiled2Unity;
using UnityEngine;


public class LevelSection : MonoBehaviour
{
    private TiledMap tileMap;
    private EdgeCollider2D cameraBounds;
    private Dictionary<string, GameObject> transitions;


    public TiledMap GetTileMap()
    {
        return tileMap;
    }

    public EdgeCollider2D GetCameraBounds()
    {
        return cameraBounds;
    }

    public Vector2 GetEntrancePosition(string position)
    {
        GameObject entrance;
        transitions.TryGetValue(position, out entrance);
        return entrance.GetComponent<TransitionZone>().GetZoneCenter();
    }

    public void Enable()
    {
        gameObject.SetActive(true);

        // Find tile map object
        string mapPrefabName = gameObject.name + "_Map";
        tileMap = transform.Find(mapPrefabName).GetComponent<TiledMap>();

        // Find all transition points
        transitions = new Dictionary<string, GameObject>();
        Transform transitionsParent = tileMap.transform.Find("Transitions");
        foreach (Transform transitionChild in transitionsParent)
        {
            string transitionName = transitionChild.name;
            transitions.Add(transitionName, transitionChild.gameObject);
        }

        // Find camera bounds
        cameraBounds = tileMap.transform.Find("CameraBoundaries").transform.Find("CameraBounds").GetComponent<EdgeCollider2D>();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
