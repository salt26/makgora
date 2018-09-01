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

    private void Awake()
    {
        asyncLoad = null;
    }

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

    public void LoadSnipingHard()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Sniping;
        Manager.instance.Level = Manager.GameLevel.Hard;
        StartCoroutine(LoadMainGameScene());
    }

    public void LoadBossHard()
    {
        loadingPanel.SetActive(true);
        Manager.instance.Mode = Manager.GameMode.Boss;
        Manager.instance.Level = Manager.GameLevel.Hard;
        StartCoroutine(LoadBossHardScene());
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

    IEnumerator LoadTutorialScene()
    {
        Manager.instance.Unpause();
        yield return new WaitForSeconds(1f);
        asyncLoad = SceneManager.LoadSceneAsync("Tutorial");
        while (!asyncLoad.isDone)
        {
            //yield return new WaitForSeconds(1f);
            yield return null;
        }
        yield return null;
    }

    IEnumerator LoadBossHardScene()
    {
        Manager.instance.Unpause();
        yield return new WaitForSeconds(1f);
        asyncLoad = SceneManager.LoadSceneAsync("Boss");
        while (!asyncLoad.isDone)
        {
            //yield return new WaitForSeconds(1f);
            yield return null;
        }
        yield return null;
    }

    IEnumerator LoadMainGameScene()
    {
        Manager.instance.Unpause();
        yield return new WaitForSeconds(1f);
        asyncLoad = SceneManager.LoadSceneAsync("MainGame");
        while (!asyncLoad.isDone)
        {
            //yield return new WaitForSeconds(1f);
            yield return null;
        }
        yield return null;
    }
}
