using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSystem : MonoBehaviour
{
    public static LevelSystem levelSystem;

    public PlayerAgent player;
    public List<GameObject> focalPoints;

    private AudioSource[] musicSources;
    private string sceneName;
    private string lastUsedEntrance = "PlayerSpawn";

    private Dictionary<string, GameObject> sceneTransitions;


    private void Start()
    {
        if (levelSystem != null) Destroy(levelSystem);
        else levelSystem = this;
        DontDestroyOnLoad(this);

        musicSources = GetComponents<AudioSource>();

        RainSystem.rainSystem.gameObject.SetActive(false);
        CameraSystem.cameraSystem.FadeOutToBlack(0);
    }

    public void BeginGame(string startingSceneName)
    {
        RainSystem.rainSystem.gameObject.SetActive(true);

        // Fade in main music
        StartCoroutine(fadeInAudio(musicSources[0], 0.8f, 0.0025f));
        // Fade in ambient music
        StartCoroutine(fadeInAudio(musicSources[1], 0.4f, 0.001f));

        // Disable player input while initial scene fades in
        player.SetInput(false);
        StartCoroutine(temporarilyDisableInput(1));
    }


    /**************************
     *       Focal Point      *
     **************************/
    public void SetPlayerAsFocalPoint()
    {
        CameraSystem.cameraSystem.ChangeTarget(player.gameObject);
    }

    public void SetTargetAsFocalPoint(int index)
    {
        CameraSystem.cameraSystem.ChangeTarget(focalPoints[index]);
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
        sceneName = nextSceneName;
        lastUsedEntrance = connecting;

        player.SetNextVelocity(GetTransitionVelocity());
        CameraSystem.cameraSystem.FadeOutToBlack(0.75f).setOnComplete(LoadNextScene);
    }

    public void ReviveTransition()
    {
        player.SetInput(false);
        player.SetNextVelocity(Vector2.zero);

        lastUsedEntrance = "PlayerSpawn";
        LoadNextScene();
    }

    private Vector2 GetTransitionVelocity()
    {
        Vector2 transitionVelocity = new Vector2(0, 0);
        if (lastUsedEntrance == "N_Entrance")
        {
            transitionVelocity.y = -1;
        }
        else if (lastUsedEntrance == "E_Entrance")
        {
            transitionVelocity.x = 1;
        }
        else if (lastUsedEntrance == "S_Entrance")
        {
            transitionVelocity.y = 1;
        }
        else if (lastUsedEntrance == "W_Entrance")
        {
            transitionVelocity.x = -1;
        }
        else
        {
            transitionVelocity.x = 0;
            transitionVelocity.y = 0;
        }
        return transitionVelocity * player.GetDefaultSpeed();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnLevelWasLoaded(int level)
    {
        SetupScene();
        CameraSystem.cameraSystem.SetDampTime(0);
        SetPlayerAsFocalPoint();

        player.SetNextVelocity(GetTransitionVelocity());
        player.Transition(GetSceneEntrance(lastUsedEntrance));

        CameraSystem.cameraSystem.FadeInFromBlack(1).setOnComplete(ResetTransitionSettings);
    }

    private void ResetTransitionSettings()
    {
        CameraSystem.cameraSystem.RestoreDefaultDampTime();
        StartCoroutine(RestorePlayerControl(0.2f));
    }

    private IEnumerator RestorePlayerControl(float time)
    {
        yield return new WaitForSeconds(time);
        player.SetInput(true);
        player.EndTransition();
    }


    /**************************
     *       SceneSetup       *
     **************************/
    private void SetupScene()
    {
        sceneName = SceneManager.GetActiveScene().name;

        // Find tilemap parent
        string tileMapObjectName = sceneName + "_Map";
        Transform tileMap = GameObject.Find(tileMapObjectName).transform;

        // Find all scene transition points (entrances)
        sceneTransitions = new Dictionary<string, GameObject>();
        Transform transitionsParent = tileMap.transform.Find("Transitions");
        foreach (Transform transitionChild in transitionsParent)
        {
            string transitionName = transitionChild.name;
            sceneTransitions.Add(transitionName, transitionChild.gameObject);
        }

        // Find and set scene camera bounds
        EdgeCollider2D cameraBounds = tileMap.transform.Find("CameraBoundaries").transform.Find("CameraBounds").GetComponent<EdgeCollider2D>();

        CameraSystem.cameraSystem.SetCameraBounds(cameraBounds);
    }

    public Vector2 GetSceneEntrance(string position)
    {
        GameObject entrance;
        sceneTransitions.TryGetValue(position, out entrance);
        return entrance.GetComponent<TransitionZone>().GetZoneCenter();
    }
}
