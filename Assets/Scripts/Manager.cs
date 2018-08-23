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
    public enum GameMode { None, Tutorial, Guardian, Vagabond, Stalker, Avoiding, Shooting, Network, Sniping }

    /// <summary>
    /// 게임 난이도입니다. 난이도가 구분되지 않는 모드는 Hard입니다. None은 메인 메뉴에서 사용됩니다.
    /// </summary>
    public enum GameLevel { None, Hard, Easy }

    // TODO 씬 전환할 때 이거 설정하기
    private GameMode mode;
    private GameLevel level;
    private bool isGameOver;
    private bool isPaused;

    // TODO 아래 게임오브젝트들 설정하기
    private GameObject canvas;
    private GameObject startPanel;
    private GameObject pausePanel;
    private GameObject buttonPause;
    private GameObject winPanel;
    private GameObject losePanel;
    private GameObject skipTutorialButton;
    private GameObject player;
    private GameObject enemy;

    [SerializeField]
    private AudioClip loseSound;
    [SerializeField]
    private AudioClip winSound;

    [SerializeField]
    private float easyChargeSpeed;          // 쉬움 모드에서 한쪽 방향으로 조준 중에 초당 충전되는 Z좌표 거리
    [SerializeField]
    private float hardChargeSpeed;          // 어려움 모드에서 한쪽 방향으로 조준 중에 초당 충전되는 Z좌표 거리
    [SerializeField]
    private float prepareChargeDistance;    // 한쪽 방향으로 조준 중에 Z좌표 거리로 이 변수의 값만큼 충전될 시간 동안 무기를 소환함.

    [SerializeField]
    private float movingSpeed;
    [SerializeField]
    private float temporalSpeed;
    [SerializeField]
    private float knifeSpeed;

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

    public bool IsPaused
    {
        get { return isPaused; }
    }

    public GameObject Canvas
    {
        get { return canvas; }
        set { canvas = value; }
    }
    
    public GameObject StartPanel
    {
        set { startPanel = value; }
    }

    public GameObject WinPanel
    {
        set { winPanel = value; }
    }

    public GameObject LosePanel
    {
        set { losePanel = value; }
    }

    public GameObject PausePanel
    {
        set { pausePanel = value; }
    }

    public GameObject ButtonPause
    {
        set { buttonPause = value; }
    }

    public GameObject PlayerObject
    {
        get { return player; }
        set { player = value; }
    }

    public GameObject EnemyObject
    {
        get { return enemy; }
        set { enemy = value; }
    }

    public GameObject SkipTutorialButton
    {
        set { skipTutorialButton = value; }
    }

    public float EasyChargeSpeed
    {
        get { return easyChargeSpeed; }
    }

    public float HardChargeSpeed
    {
        get { return hardChargeSpeed; }
    }

    public float PrepareChargeTime
    {
        get {
            string gameLevel = instance.GetCurrentGame()[1];
            if (gameLevel.Equals("Easy") && instance.EasyChargeSpeed != 0f) {
                return prepareChargeDistance / instance.EasyChargeSpeed;
            }
            else if (gameLevel.Equals("Hard") && instance.HardChargeSpeed != 0f)
            {
                return prepareChargeDistance / instance.HardChargeSpeed;
            }
            else return prepareChargeDistance;
        }
    }

    public float MovingSpeed
    {
        get { return movingSpeed; }
    }

    public float TemporalSpeed
    {
        get { return temporalSpeed; }
    }

    public float KnifeSpeed
    {
        get { return knifeSpeed; }
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
        instance.isPaused = false;
        instance.Mode = GameMode.None;
        instance.Level = GameLevel.None;
    }

    public void WinGame()
    {
        StartCoroutine("Win");
    }

    public void LoseGame()
    {
        StartCoroutine("Lose");
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
        instance.isPaused = true;
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
        SceneManager.LoadScene("MainGame");
    }
    */

    public void MenuButton()
    {
        instance.WinPanel = null;
        instance.losePanel = null;
        instance.SkipTutorialButton = null;
        instance.Mode = GameMode.None;
        instance.Level = GameLevel.None;
        instance.isGameOver = false;
        instance.isPaused = true;
        SceneManager.LoadScene("Menu");
    }

    public void StartButton()
    {
        Time.timeScale = 1f;
        instance.startPanel.SetActive(false);
        StartCoroutine("Unpause");
    }

    /* TODO 
     * Invoke this method when 'Esc' key is pressed.
     */
    public void PauseButton()
    {
        instance.pausePanel.SetActive(true);
        instance.buttonPause.SetActive(false);
        Pause();
    }

    IEnumerator Unpause()
    {
        yield return null;
        instance.isPaused = false;
        instance.pausePanel.SetActive(false);
        instance.buttonPause.SetActive(true);

        string gameMode = Manager.instance.GetCurrentGame()[0];
        if (gameMode.Equals("Vagabond") || gameMode.Equals("Guardian") || gameMode.Equals("Stalker"))
        {
            EnemyObject.GetComponent<Enemy>().SpeakReady();
            yield return new WaitForSeconds(2.2f);
            PlayerObject.GetComponent<Player>().SpeakReady();
        }
    }

    IEnumerator Lose()
    {
        instance.SetGameOver();
        yield return new WaitForSeconds(3.0f);
        GetComponent<AudioSource>().clip = loseSound;
        GetComponent<AudioSource>().Play();
        instance.losePanel.SetActive(true);
    }

    IEnumerator Win()
    {
        instance.SetGameOver();
        yield return new WaitForSeconds(3.5f);
        instance.winPanel.SetActive(true);
        GetComponent<AudioSource>().clip = winSound;
        GetComponent<AudioSource>().Play();
    }

    IEnumerator Graduate()
    {
        instance.skipTutorialButton.SetActive(false);
        instance.SetGameOver();
        yield return new WaitForSeconds(3.0f);
        instance.winPanel.SetActive(true);

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
    
    public void Pause()
    {
        instance.isPaused = true;
        Time.timeScale = 0f;
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
            l.Add("Easy");
            return l;
        }
        else if (SceneManager.GetActiveScene().name.Equals("MainGame"))
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
