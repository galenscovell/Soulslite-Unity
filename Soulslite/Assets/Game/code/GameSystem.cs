using UnityEngine.SceneManagement;
using UnityEngine;


public class GameSystem : MonoBehaviour
{
    public static GameSystem gameSystem;


    private void Start()
    {
		
	}
	
	public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
