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
        Manager.instance.Mode = Manager.GameMode.Tutorial;
        Manager.instance.Level = Manager.GameLevel.Hard;
        StartCoroutine(LoadTutorialScene());
    }

    public void LoadConcentrationEasy()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Shooting;
        Manager.instance.Level = Manager.GameLevel.Easy;
        StartCoroutine(LoadConcentrationEasyScene());
    }

    public void LoadConcentrationHard()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Shooting;
        Manager.instance.Level = Manager.GameLevel.Hard;
        StartCoroutine(LoadConcentrationHardScene());
    }

    public void LoadSufferanceEasy()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Avoiding;
        Manager.instance.Level = Manager.GameLevel.Easy;
        StartCoroutine(LoadSufferanceEasyScene());
    }

    public void LoadSufferanceHard()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Avoiding;
        Manager.instance.Level = Manager.GameLevel.Hard;
        StartCoroutine(LoadSufferanceHardScene());
    }

    public void LoadGuardianEasy()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Guardian;
        Manager.instance.Level = Manager.GameLevel.Easy;
        StartCoroutine(LoadMainGameScene());
    }

    public void LoadGuardianHard()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Guardian;
        Manager.instance.Level = Manager.GameLevel.Hard;
        StartCoroutine(LoadMainGameScene());
    }

    public void LoadVagabondEasy()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Vagabond;
        Manager.instance.Level = Manager.GameLevel.Easy;
        StartCoroutine(LoadMainGameScene());
    }

    public void LoadVagabondHard()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Vagabond;
        Manager.instance.Level = Manager.GameLevel.Hard;
        StartCoroutine(LoadMainGameScene());
    }

    public void LoadStalkerEasy()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Stalker;
        Manager.instance.Level = Manager.GameLevel.Easy;
        StartCoroutine(LoadMainGameScene());
    }

    public void LoadStalkerHard()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Stalker;
        Manager.instance.Level = Manager.GameLevel.Hard;
        StartCoroutine(LoadMainGameScene());
    }

    public void LoadNetMakgora()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Network;
        Manager.instance.Level = Manager.GameLevel.Hard;
        StartCoroutine(LoadNetMakgoraScene());
    }

    IEnumerator LoadTutorialScene()
    {
        //yield return new WaitForSeconds(1f);
        asyncLoad = SceneManager.LoadSceneAsync("Tutorial");
        while (!asyncLoad.isDone)
        {
            //yield return new WaitForSeconds(1f);
            yield return null;
        }
        yield return null;
    }

    IEnumerator LoadConcentrationEasyScene()
    {
        yield return null;
    }

    IEnumerator LoadConcentrationHardScene()
    {
        yield return null;
    }

    IEnumerator LoadSufferanceEasyScene()
    {
        yield return null;
    }

    IEnumerator LoadSufferanceHardScene()
    {
        yield return null;
    }

    IEnumerator LoadMainGameScene()
    {
        //yield return new WaitForSeconds(1f);
        asyncLoad = SceneManager.LoadSceneAsync("MainGame");
        while (!asyncLoad.isDone)
        {
            //yield return new WaitForSeconds(1f);
            yield return null;
        }
        yield return null;
    }

    IEnumerator LoadNetMakgoraScene()
    {
        yield return null;
    }
}
