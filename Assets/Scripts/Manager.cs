﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class Manager : MonoBehaviour {

    public static Manager instance;
    
    /// <summary>
    /// 게임 모드입니다. None은 메인 메뉴에서 사용됩니다.
    /// </summary>
    public enum GameMode { None, Tutorial, Guardian, Vagabond, Stalker, Avoiding, Shooting, Network }

    /// <summary>
    /// 게임 난이도입니다. 난이도가 구분되지 않는 모드는 Hard입니다. None은 메인 메뉴에서 사용됩니다.
    /// </summary>
    public enum GameLevel { None, Hard, Easy }

    // TODO 씬 전환할 때 이거 설정하기
    private GameMode mode;
    private GameLevel level;
    private bool isGameOver;

    // TODO 아래 게임오브젝트들 설정하기
    public GameObject restartPanel;
    public Text restartText;
    public GameObject skipTutorialButton;

    [SerializeField]
    private AudioClip loseSound;
    [SerializeField]
    public AudioClip winSound;

    public GameMode Mode
    {
        get { return mode; }
        set { mode = value; }
    }

    public GameLevel Level
    {
        get { return level; }
        set { level = value; }
    }

    public GameObject RestartPanel
    {
        set { restartPanel = value; }
    }

    public Text RestartText
    {
        set { restartText = value; }
    }

    public GameObject SkipTutorialButton
    {
        set { skipTutorialButton = value; }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        instance.isGameOver = false;
        instance.Mode = GameMode.None;
        instance.Level = GameLevel.None;
    }

    public void LoseGame()
    {
        StartCoroutine("Lose");
    }

    public void WinGame()
    {
        StartCoroutine("Win");
    }

    public void GraduateTutorial()
    {
        StartCoroutine("Graduate");
    }

    public void RestartButton()
    {
        if (SceneManager.GetActiveScene().name.Equals("Menu"))
        {
            instance.Mode = GameMode.None;
            instance.Level = GameLevel.None;
        }
        instance.isGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else 
		Application.Quit();
#endif
    }

    /*
    // TODO 이름 바꾸기
    public void MainGameButton()
    {
        SceneManager.LoadScene("VagabondH");
    }
    */

    public void MenuButton()
    {
        instance.RestartPanel = null;
        instance.RestartText = null;
        instance.SkipTutorialButton = null;
        instance.Mode = GameMode.None;
        instance.Level = GameLevel.None;
        instance.isGameOver = false;
        SceneManager.LoadScene("Menu");
    }

    IEnumerator Lose()
    {
        instance.SetGameOver();
        yield return new WaitForSeconds(2.0f);
        GetComponent<AudioSource>().clip = loseSound;
        GetComponent<AudioSource>().Play();
        instance.restartPanel.SetActive(true);
    }

    IEnumerator Win()
    {
        instance.SetGameOver();
        yield return new WaitForSeconds(2.0f);
        instance.restartPanel.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.5f);
        instance.restartText.text = "YOU WIN!";
        instance.restartPanel.SetActive(true);
        GetComponent<AudioSource>().clip = winSound;
        GetComponent<AudioSource>().Play();
    }

    IEnumerator Graduate()
    {
        instance.skipTutorialButton.SetActive(false);
        instance.SetGameOver();
        yield return new WaitForSeconds(2.0f);
        instance.restartPanel.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.5f);
        instance.restartPanel.SetActive(true);

        GetComponent<AudioSource>().clip = winSound;
        GetComponent<AudioSource>().Play();
    }

    public void SetGameOver()
    {
        isGameOver = true;
    }

    public bool GetGameOver()
    {
        return isGameOver;
    }

    /// <summary>
    /// 현재 게임 모드와 난이도를 길이가 2인 string 배열로 반환합니다.
    /// 반환값의 0번째 원소는 모드 이름, 1번째 원소는 난이도 이름입니다.
    /// 메뉴에 있으면 0번째 원소가 "Menu"이고, 문제가 생기면 0번째 원소가 "?"입니다.
    /// 이 메서드를 사용하면 Manager에서 직접 Mode, Level을 참조하는 것보다 안전합니다.
    /// </summary>
    /// <returns></returns>
    public List<string> GetCurrentGame()
    {
        List<string> l = new List<string>();
        if (SceneManager.GetActiveScene().name.Equals("Menu"))
        {
            l.Add("Menu");
            l.Add("None");
            return l;
        }
        else if (SceneManager.GetActiveScene().name.Equals("Tutorial"))
        {
            l.Add("Tutorial");
            l.Add("Hard");
            return l;
        }
        else if (SceneManager.GetActiveScene().name.Equals("VagabondH"))
        {
            l.Add(instance.Mode.ToString());
            l.Add(instance.Level.ToString());
            return l;
        }
        else
        {
            l.Add("?");
            l.Add("None");
            return l;
        }
    }
}