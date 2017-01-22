using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneMain : MonoBehaviour
{
    public static SceneMain sceneMain;
    public GameObject blackFade;
    public List<AudioSource> musicSources;

    // FocalPoint[0] is always the player
    public List<GameObject> focalPoints;


    private void Start()
    {
        LeanTween.alpha(blackFade, 1, 0);

        // Singleton, destroyed between scenes
        if (sceneMain != null) Destroy(sceneMain);
        else sceneMain = this;

        SetPlayerAsFocalPoint();

        // Fade in main music
        StartCoroutine(fadeInAudio(musicSources[0], 0.4f));
        // Fade in ambient music
        StartCoroutine(fadeInAudio(musicSources[1], 0.9f));

        LeanTween.alpha(blackFade, 0, 2);
    }

    public void SetPlayerAsFocalPoint()
    {
        CameraController.cameraController.ChangeTarget(focalPoints[0]);
    }

    public void SetTargetAsFocalPoint(int index)
    {
        CameraController.cameraController.ChangeTarget(focalPoints[index]);
    }

    private IEnumerator fadeInAudio(AudioSource audioSource, float targetVolume)
    {
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime);
            yield return 0;
        }
    }
}
