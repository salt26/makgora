using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {

    [SerializeField]
    private GameObject startPanel;
    [SerializeField]
    private GameObject winPanel;
    [SerializeField]
    private GameObject losePanel;
    [SerializeField]
    private GameObject skipTutorialButton;
    [SerializeField]
    private Text modeText;

    private void Start()
    {
        Manager.instance.Canvas = this.gameObject;
        Manager.instance.WinPanel = winPanel;
        if (losePanel != null)
        {
            Manager.instance.LosePanel = losePanel;
        }
        if (skipTutorialButton != null)
            Manager.instance.SkipTutorialButton = skipTutorialButton;
        if (startPanel != null)
        {
            Manager.instance.StartPanel = startPanel;
            Manager.instance.Pause();
        }

        string gameMode = Manager.instance.GetCurrentGame()[0];
        string gameLevel = Manager.instance.GetCurrentGame()[1];
        if (gameMode.Equals("Tutorial"))
        {
            modeText.text = "튜토리얼";
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
        // TODO 모드 추가 시 추가바람
    }

    public void QuitButton()
    {
        Manager.instance.QuitButton();
    }
    
    /*
    public void MainGameButton()
    {
        Manager.instance.MainGameButton();
    }
    */

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
}
