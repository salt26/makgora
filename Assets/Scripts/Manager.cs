using System.Collections;
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
        if (SceneManager.GetActiveScene().name == "Menu")
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

    public string GetCurrentGame()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            return "Menu";
        }
        else if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            return "TutorialHard";
        }
        else if (SceneManager.GetActiveScene().name == "VagabondH")
        {
            string r = instance.Mode.ToString();
            r += instance.Level.ToString();
            return r;
        }
        else
        {
            return "?";
        }
    }
}
