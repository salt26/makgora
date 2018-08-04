using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {

    public GameObject loadingPanel;

    public void LoadTutorial()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(LoadTutorialScene());
        //SceneManager.LoadScene("Tutorial");
    }
            
    IEnumerator LoadTutorialScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Tutorial");
        while(!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
