using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    public static LevelManager levelManager;

    public PlayerAgent player;
    public List<LevelSection> levels;
    public List<GameObject> focalPoints;

    private AudioSource[] musicSources;
    private LevelSection currentSection;


    private void Start()
    {
        // Singleton, destroyed between scenes
        if (levelManager != null) Destroy(levelManager);
        else levelManager = this;

        musicSources = GetComponents<AudioSource>();

        // Fade in main music
        StartCoroutine(fadeInAudio(musicSources[0], 0.8f, 0.0025f));
        // Fade in ambient music
        StartCoroutine(fadeInAudio(musicSources[1], 0.4f, 0.001f));
        // Disable player input while initial scene fades in
        player.SetInput(false);
        StartCoroutine(temporarilyDisableInput(2));

        // Begin first level section
        currentSection = levels[0];
        currentSection.Enable();
        CameraController.cameraController.SetCameraBounds(currentSection.GetCameraBounds());
        CameraController.cameraController.FadeOutToBlack(0);
        CameraController.cameraController.FadeInFromBlack(3);
        SetPlayerAsFocalPoint();
    }


    /**************************
     *       Focal Point      *
     **************************/
    public void SetPlayerAsFocalPoint()
    {
        CameraController.cameraController.ChangeTarget(player.gameObject);
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
    public IEnumerator temporarilyDisableInput(float time)
    {
        yield return new WaitForSeconds(time);
        player.SetInput(true);
    }


    /**************************
     *       Transition       *
     **************************/
    public void transitionToSection(int sectionIndex, Vector2 transitionDirection)
    {
        player.SetInput(false);
        StartCoroutine(temporarilyDisableInput(2.5f));

        CameraController.cameraController.FadeOutToBlack(0.5f);

        currentSection.Disable();
        currentSection = levels[sectionIndex];
        currentSection.Enable();

        player.SetFacingDirection(transitionDirection);
        player.Transition(currentSection.GetEntrancePosition());
        CameraController.cameraController.SetCameraBounds(currentSection.GetCameraBounds());
        SetPlayerAsFocalPoint();

        CameraController.cameraController.FadeInFromBlack(2f);
    }
}
