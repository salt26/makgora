using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {

    [SerializeField]
    private GameObject startPanel;
    [SerializeField]
    private GameObject snipingStartPanel;
    [SerializeField]
    private GameObject bossStartPanel;
    [SerializeField]
    private GameObject winPanel;
    [SerializeField]
    private GameObject losePanel;
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject pauseButton;
    [SerializeField]
    private Text modeText;
    [SerializeField]
    private GameObject modeImage;
    [SerializeField]
    private Sprite missionImage;
    [SerializeField]
    private GameObject sortingObject;   // 새로 생성될 ui 오브젝트들이 패널 뒤에 배치되도록 하는 오브젝트입니다.
    [SerializeField]
    private GameObject blindPanel;

    private void Start()
    {
        Manager.instance.Canvas = sortingObject;
        if (startPanel != null)
        {
            Manager.instance.StartPanel = startPanel;
            Manager.instance.Pause();
        }
        if(snipingStartPanel != null)
        {
            Manager.instance.SnipingStartPanel = snipingStartPanel;
            Manager.instance.Pause();
        }
        if (bossStartPanel != null)
        {
            Manager.instance.BossStartPanel = bossStartPanel;
            Manager.instance.Pause();
        }
        Manager.instance.WinPanel = winPanel;
        if (losePanel != null)
        {
            Manager.instance.LosePanel = losePanel;
        }
        Manager.instance.PausePanel = pausePanel;
        Manager.instance.ButtonPause = pauseButton;
        Manager.instance.BlindPanel = blindPanel;

        string gameMode = Manager.instance.GetCurrentGame()[0];
        string gameLevel = Manager.instance.GetCurrentGame()[1];
        if (gameMode.Equals("Tutorial"))
        {
            modeText.text = "튜토리얼 - 1/5";
            // TODO 현재 페이즈에 따라 다른 텍스트 표시
        }
        else if (gameMode.Equals("Vagabond") && gameLevel.Equals("Easy"))
        {
            modeText.text = "방랑자(쉬움)";
        }
        else if (gameMode.Equals("Guardian") && gameLevel.Equals("Easy"))
        {
            modeText.text = "수호자(쉬움)";
        }
        else if (gameMode.Equals("Stalker") && gameLevel.Equals("Easy"))
        {
            modeText.text = "추적자(쉬움)";
        }
        else if (gameMode.Equals("Deceiver") && gameLevel.Equals("Easy"))
        {
            modeText.text = "기만자(쉬움)";
        }
        else if (gameMode.Equals("Vagabond") && gameLevel.Equals("Hard"))
        {
            modeText.text = "방랑자(어려움)";
        }
        else if (gameMode.Equals("Guardian") && gameLevel.Equals("Hard"))
        {
            modeText.text = "수호자(어려움)";
        }
        else if (gameMode.Equals("Stalker") && gameLevel.Equals("Hard"))
        {
            modeText.text = "추적자(어려움)";
        }
        else if (gameMode.Equals("Deceiver") && gameLevel.Equals("Hard"))
        {
            modeText.text = "기만자(어려움)";
        }
        else if (gameMode.Equals("Sniping") && gameLevel.Equals("Hard"))
        {
            modeText.text = "저격 미션";
            modeImage.GetComponent<Image>().sprite = missionImage;
            startPanel.SetActive(false);
            snipingStartPanel.SetActive(true);
        }
        else if (gameMode.Equals("Boss") && gameLevel.Equals("Hard"))
        {
            modeText.text = "보스 미션";
            modeImage.GetComponent<Image>().sprite = missionImage;
            startPanel.SetActive(false);
            bossStartPanel.SetActive(true);
        }
        // TODO 모드 추가 시 추가바람
    }

    public void QuitButton()
    {
        Manager.instance.QuitButton();
    }

    public void RestartButton()
    {
        Manager.instance.RestartButton();
    }
    
    public void MenuButton()
    {
        Manager.instance.MenuButton();
    }

    public void StartButton()
    {
        Manager.instance.StartButton();
    }

    public void PauseButton()
    {
        Manager.instance.PauseButton();
    }

    public void UpdatePhase(int phase)
    {
        modeText.text = "튜토리얼 - " + phase + "/5";
    }
}
