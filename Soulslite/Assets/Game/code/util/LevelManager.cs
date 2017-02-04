using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public static LevelManager levelManager;

    public PlayerAgent player;
    public List<GameObject> focalPoints;

    private AudioSource[] musicSources;
    private string sceneName;
    private string connectingTransition;

    private Dictionary<string, GameObject> sceneTransitions;


    private void Start()
    {
        if (levelManager != null) Destroy(levelManager);
        else levelManager = this;
        DontDestroyOnLoad(this);

        musicSources = GetComponents<AudioSource>();

        // Fade in main music
        StartCoroutine(fadeInAudio(musicSources[0], 0.8f, 0.0025f));
        // Fade in ambient music
        StartCoroutine(fadeInAudio(musicSources[1], 0.4f, 0.001f));
        // Disable player input while initial scene fades in
        player.SetInput(false);
        StartCoroutine(temporarilyDisableInput(1));

        // Begin first level section
        SetupScene();
        CameraController.cameraController.FadeOutToBlack(0);
        SetPlayerAsFocalPoint();
        CameraController.cameraController.FadeInFromBlack(2);
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
    public void BeginTransition(string nextSceneName, string connecting)
    {
        player.SetInput(false);
        StartCoroutine(temporarilyDisableInput(2f));
        sceneName = nextSceneName;
        connectingTransition = connecting;

        CameraController.cameraController.SetDampTime(0);
        CameraController.cameraController.FadeOutToBlack(0.75f).setOnComplete(AsyncLoadScene);
    }

    private void AsyncLoadScene()
    {
        StartCoroutine(LoadNewScene(sceneName));
    }

    private void EndTransition()
    {
        SetPlayerAsFocalPoint();
        player.Transition(GetSceneEntrance(connectingTransition));

        CameraController.cameraController.FadeInFromBlack(0.5f).setOnComplete(ResetTransitionSettings);
    }

    private void ResetTransitionSettings()
    {
        player.SetInput(true);
        CameraController.cameraController.RestoreDefaultDampTime();
    }


    public IEnumerator LoadNewScene(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            yield return null;
        }
        EndTransition();
    }


    /**************************
     *       SceneSetup       *
     **************************/
    private void SetupScene()
    {
        Transform tileMap = GameObject.Find("Map").transform;

        // Find all transition points
        sceneTransitions = new Dictionary<string, GameObject>();
        Transform transitionsParent = tileMap.transform.Find("Transitions");
        foreach (Transform transitionChild in transitionsParent)
        {
            string transitionName = transitionChild.name;
            sceneTransitions.Add(transitionName, transitionChild.gameObject);
        }

        // Find camera bounds
        EdgeCollider2D cameraBounds = tileMap.transform.Find("CameraBoundaries").transform.Find("CameraBounds").GetComponent<EdgeCollider2D>();

        CameraController.cameraController.SetCameraBounds(cameraBounds);
    }

    public Vector2 GetSceneEntrance(string position)
    {
        GameObject entrance;
        sceneTransitions.TryGetValue(position, out entrance);
        return entrance.GetComponent<TransitionZone>().GetZoneCenter();
    }
}
