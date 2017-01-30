using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    public static LevelManager levelManager;
    public PlayerAgent playerAgent;
    public List<AudioSource> musicSources;

    // FocalPoint[0] is always the player
    public List<GameObject> focalPoints;


    private void Start()
    {
        // Singleton, destroyed between scenes
        if (levelManager != null) Destroy(levelManager);
        else levelManager = this;

        SetPlayerAsFocalPoint();

        // Fade in main music
        StartCoroutine(fadeInAudio(musicSources[0], 0.4f, 0.0005f));
        // Fade in ambient music
        StartCoroutine(fadeInAudio(musicSources[1], 0.9f, 0.001f));
        // Disable player input while initial scene fades in
        StartCoroutine(temporarilyDisableInput());
    }


    /**************************
     *       Focal Point      *
     **************************/
    public void SetPlayerAsFocalPoint()
    {
        CameraController.cameraController.ChangeTarget(focalPoints[0]);
    }

    public void SetTargetAsFocalPoint(int index)
    {
        CameraController.cameraController.ChangeTarget(focalPoints[index]);
    }


    /**************************
     *          Audio         *
     **************************/
    public IEnumerator fadeInAudio(AudioSource audioSource, float targetVolume, float speed)
    {
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, speed);
            yield return 0;
        }
    }

    public IEnumerator fadeOutAudio(AudioSource audioSource, float targetVolume, float speed)
    {
        while (audioSource.volume > targetVolume)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, speed);
            yield return 0;
        }
    }


    /**************************
     *          Input         *
     **************************/
    public IEnumerator temporarilyDisableInput()
    {
        yield return new WaitForSeconds(2f);
        playerAgent.SetInput(true);
    }
}
