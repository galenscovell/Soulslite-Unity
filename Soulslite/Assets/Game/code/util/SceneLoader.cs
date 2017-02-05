using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public Text loadingText;

    private bool loadingScene = false;


    private void Update()
    {
        if (Input.anyKey && loadingScene == false)
        {
            loadingScene = true;
            loadingText.text = "Loading";
            SceneManager.LoadScene(sceneName);
            // StartCoroutine(LoadNewScene());
            LevelSystem.levelSystem.BeginGame(sceneName);
        }

        if (loadingScene == true)
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        }
    }
    
    public IEnumerator LoadNewScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            yield return null;
        }
    }
}