using System.Collections.Generic;
using UnityEngine;


public class SceneLayout : MonoBehaviour
{
    public static SceneLayout sceneLayout;

    // FocalPoint[0] is always the player
    public List<GameObject> focalPoints;


    private void Start()
    {
        // Singleton, destroyed between scenes
        if (sceneLayout != null) Destroy(sceneLayout);
        else sceneLayout = this;

        SetPlayerAsFocalPoint();
    }

    public void SetPlayerAsFocalPoint()
    {
        CameraController.cameraController.ChangeTarget(focalPoints[0]);
    }

    public void SetTargetAsFocalPoint(int index)
    {
        CameraController.cameraController.ChangeTarget(focalPoints[index]);
    }
}
