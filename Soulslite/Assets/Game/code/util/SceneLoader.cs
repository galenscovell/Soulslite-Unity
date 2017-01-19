using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private Text loadingText;

    private bool loadingScene = false;


    private void Update()
    {
        if (Input.anyKey && loadingScene == false)
        {
            loadingScene = true;
            loadingText.text = "Loading";
            StartCoroutine(LoadNewScene());
        }

        if (loadingScene == true)
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        }
    }
    
    IEnumerator LoadNewScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
        {
            yield return null;
        }
    }
}