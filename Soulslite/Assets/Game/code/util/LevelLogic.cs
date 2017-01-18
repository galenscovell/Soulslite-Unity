using System.Collections.Generic;
using UnityEngine;


public class LevelLogic : MonoBehaviour
{
    public static LevelLogic levelLogic;

    // FocalPoint[0] is always the player
    public List<GameObject> focalPoints;


    private void Start()
    {
        // Singleton
        if (levelLogic != null) Destroy(levelLogic);
        else levelLogic = this;
        DontDestroyOnLoad(this);

        TargetPlayer();
    }

    public void TargetPlayer()
    {
        CameraController.cameraController.ChangeTarget(focalPoints[0]);
    }

    public void TargetFocalPoint(int index)
    {
        CameraController.cameraController.ChangeTarget(focalPoints[index]);
    }
}
