using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Restart : MonoBehaviour {

    [SerializeField]
    private GameObject restartPanel;
    [SerializeField]
    private Text restartText;
    [SerializeField]
    private GameObject skipTutorialButton;

    private void Start()
    {
        Debug.LogWarning("It works on Restart!");
        Manager.instance.RestartPanel = restartPanel;
        if (restartText != null)
            Manager.instance.RestartText = restartText;
        if (skipTutorialButton != null)
            Manager.instance.SkipTutorialButton = skipTutorialButton;
    }

    public void RestartButton()
    {
        Manager.instance.RestartButton();
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

    public void MenuButton()
    {
        Manager.instance.MenuButton();
    }
}
