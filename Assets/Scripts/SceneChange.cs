using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneChange : MonoBehaviour {

    public GameObject loadingPanel;
    public GameObject inDevelopmentPanel;
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

    public void LoadConcentration()
    {
        //loadingPanel.SetActive(true);
        StartCoroutine(LoadConcentrationScene());
    }

    public void LoadSufferanceEasy()
    {
        //loadingPanel.SetActive(true);
        StartCoroutine(LoadSufferanceEasyScene());
    }

    public void LoadSufferanceHard()
    {
        //loadinfPanel.SetActive(true);
        StartCoroutine(LoadSufferanceHardScene());
    }

    public void LoadProtectorEasy()
    {
        //loadingPanel.SetActive(true);
        StartCoroutine(LoadProtectorEasyScene());
    }

    public void LoadProtectorHard()
    {
        //loadingPanel.SetActive(true);
        StartCoroutine(LoadProtectorHardScene());
    }

    public void LoadVagabondEasy()
    {
        //loadingPanel.SetActive(true);
        StartCoroutine(LoadVagabondEasyScene());
    }

    public void LoadVagabondHard()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(LoadVagabondHardScene());
    }

    public void LoadTrackerEasy()
    {
        //loadingPanel.SetActive(true);
        StartCoroutine(LoadTrackerEasyScene());
    }

    public void LoadTrackerHard()
    {
        //loadingPanel.SetActive(true);
        StartCoroutine(LoadTrackerHardScene());
    }

    public void LoadNetMakgora()
    {
        //loadingPanel.SetActive(true);
        StartCoroutine(LoadNetMakgoraScene());
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

    IEnumerator LoadConcentrationScene()
    {
        while (!inDevelopmentPanel.activeInHierarchy)
        {
            inDevelopmentPanel.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        inDevelopmentPanel.SetActive(false);
        yield return null;
    }

    IEnumerator LoadSufferanceEasyScene()
    {
        while (!inDevelopmentPanel.activeInHierarchy)
        {
            inDevelopmentPanel.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        inDevelopmentPanel.SetActive(false);
        yield return null;
    }

    IEnumerator LoadSufferanceHardScene()
    {
        while (!inDevelopmentPanel.activeInHierarchy)
        {
            inDevelopmentPanel.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        inDevelopmentPanel.SetActive(false);
        yield return null;
    }

    IEnumerator LoadProtectorEasyScene()
    {
        while (!inDevelopmentPanel.activeInHierarchy)
        {
            inDevelopmentPanel.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        inDevelopmentPanel.SetActive(false);
        yield return null;
    }

    IEnumerator LoadProtectorHardScene()
    {
        while (!inDevelopmentPanel.activeInHierarchy)
        {
            inDevelopmentPanel.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        inDevelopmentPanel.SetActive(false);
        yield return null;
    }

    IEnumerator LoadVagabondEasyScene()
    {
        while (!inDevelopmentPanel.activeInHierarchy)
        {
            inDevelopmentPanel.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        inDevelopmentPanel.SetActive(false);
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

    IEnumerator LoadTrackerEasyScene()
    {
        while (!inDevelopmentPanel.activeInHierarchy)
        {
            inDevelopmentPanel.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        inDevelopmentPanel.SetActive(false);
        yield return null;
    }

    IEnumerator LoadTrackerHardScene()
    {
        while (!inDevelopmentPanel.activeInHierarchy)
        {
            inDevelopmentPanel.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        inDevelopmentPanel.SetActive(false);
        yield return null;
    }

    IEnumerator LoadNetMakgoraScene()
    {
        while (!inDevelopmentPanel.activeInHierarchy)
        {
            inDevelopmentPanel.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        inDevelopmentPanel.SetActive(false);
        yield return null;
    }
}
