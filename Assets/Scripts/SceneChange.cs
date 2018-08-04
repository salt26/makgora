using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneChange : MonoBehaviour {

    public GameObject loadingPanel;
    AsyncOperation asyncLoad = null;

    public void QuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else 
		Application.Quit();
#endif
    }

    public void LoadTutorial()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(LoadTutorialScene());
    }

    public void LoadVagabondrHard()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(LoadVagabondHardScene());
    }
    
    IEnumerator LoadTutorialScene()
    {
        yield return new WaitForSeconds(1f);
        asyncLoad = SceneManager.LoadSceneAsync("Tutorial");
        while (!asyncLoad.isDone)
        {
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    IEnumerator LoadVagabondHardScene()
    {
        yield return new WaitForSeconds(1f);
        asyncLoad = SceneManager.LoadSceneAsync("VagabondH");
        while (!asyncLoad.isDone)
        {
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
}
