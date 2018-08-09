using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 모드입니다.
/// </summary>
enum GameMode { Tutorial, Guardian, Vagabond, Stalker, Avoiding, Shooting, Network}

/// <summary>
/// 게임 난이도입니다. 난이도가 구분되지 않는 모드는 Hard입니다.
/// </summary>
enum GameLevel { Easy, Hard }

public class Manager : MonoBehaviour {

    public static Manager instance;

    // TODO 씬 전환할 때 이거 설정하기
    private GameMode mode;
    private GameLevel level;

    private bool isGameOver;

    // TODO 아래 게임오브젝트들 설정하기
    private GameObject restartPanel;
    private Text restartText;
    private GameObject skipTutorialButton;

    [SerializeField]
    private AudioClip loseSound;
    [SerializeField]
    public AudioClip winSound;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        isGameOver = false;
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

    IEnumerator Lose()
    {
        instance.SetGameOver();
        yield return new WaitForSeconds(2.0f);
        GetComponent<AudioSource>().clip = loseSound;
        GetComponent<AudioSource>().Play();
        restartPanel.SetActive(true);
    }

    IEnumerator Win()
    {
        instance.SetGameOver();
        yield return new WaitForSeconds(2.0f);
        restartPanel.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.5f);
        restartText.text = "YOU WIN!";
        restartPanel.SetActive(true);
        GetComponent<AudioSource>().clip = winSound;
        GetComponent<AudioSource>().Play();
    }

    IEnumerator Graduate()
    {
        skipTutorialButton.SetActive(false);
        instance.SetGameOver();
        yield return new WaitForSeconds(2.0f);
        restartPanel.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.5f);
        restartPanel.SetActive(true);

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
}
