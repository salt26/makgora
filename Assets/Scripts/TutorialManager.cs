using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

    public Text tutorialText;
    public GameObject destination;
    public Image redPage;
    public Text redText;
    public Image greenPage;
    public Text greenBookText;
    public Image book;
    public GameObject tutorialBubble;
    public List<GameObject> WASD;

    private int phase;  // 1: XY평면 상 이동, 2: 페이지 이동, 3: 상대 투사체 맞기, 4: 공격
    private int process;    // 0부터 시작, 각 페이즈에서의 진행 정도를 나타냅니다.
    private bool isPhaseStarted;    // 이것이 false가 되면 다음 프레임에 페이즈가 넘어갑니다.
    private bool isProcessReady;    // 이것이 false가 되면 다음 프레임에 true가 되면서 새 설명이 나타납니다.
    private bool isEnterAvailable;  // 이것이 true인 동안 Enter키를 눌러 다음 설명으로 넘어갈 수 있습니다.
    private GameObject myBubble;    // 현재 떠 있는 뾰족 말풍선(설명)을 가지고 있습니다.

    public int Phase
    {
        get
        {
            return phase;
        }
    }

    public bool CanMoveXY
    {
        get
        {
            return State(1, 1);
        }
    }

    public bool CanMoveZ
    {
        get
        {
            return (Phase == 2 || Phase == 3);
        }
    }

    public bool CanShoot
    {
        get
        {
            return Phase == 3;
        }
    }

	// Use this for initialization
	void Start () {
        phase = 1;
        process = 0;
        isPhaseStarted = false;
        isProcessReady = false;
	}
	
	void FixedUpdate () {
        if (isEnterAvailable && Input.GetKeyDown(KeyCode.Return))
        {
            isEnterAvailable = false;
            NextProcess();
            return;
        }

        if (WASD.Count > 0 && WASD[0].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.W))
            {
                WASD[0].GetComponent<Image>().color = new Color(0.9f, 0.1f, 0.1f);
            }
            else
            {
                WASD[0].GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
        }
        if (WASD.Count > 1 && WASD[1].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.A))
            {
                WASD[1].GetComponent<Image>().color = new Color(0.9f, 0.1f, 0.1f);
            }
            else
            {
                WASD[1].GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
        }
        if (WASD.Count > 2 && WASD[2].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.S))
            {
                WASD[2].GetComponent<Image>().color = new Color(0.9f, 0.1f, 0.1f);
            }
            else
            {
                WASD[2].GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
        }
        if (WASD.Count > 3 && WASD[3].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.D))
            {
                WASD[3].GetComponent<Image>().color = new Color(0.9f, 0.1f, 0.1f);
            }
            else
            {
                WASD[3].GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
        }

        if (StateNotReady(1, 0) && !isPhaseStarted)
        {
            // phase 1, process 0
            isPhaseStarted = true;
            /*
            tutorialText.text = "당신은 만화 속의 주인공입니다!\n" +
                "컷을 넘나들며 주인공을\nW(상), S(하), A(좌), D(우)로 움직일 수 있습니다.\n" + 
                "\"파란색 공이 있는 곳으로 주인공을 움직이세요.\"";
                */
            Instantiate(destination, new Vector3(-1.5f, 0.56f, 0f), Quaternion.identity);
            redPage.enabled = false;
            redText.enabled = false;
            greenPage.enabled = false;
            greenBookText.enabled = false;
            book.enabled = false;
            CreateBubble("여기서 보니 정말 반갑구나,\n" +
                "내 아들아!\n" +
                "내가 그대를 강인한 전사의 길로 인도할 것이다.\n" +
                "<color=#666699>(Enter키를 누르면 넘어갑니다.)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(1, 1))
        {
            foreach (GameObject g in WASD)
            {
                g.SetActive(true);
            }
            CreateBubble("우선 간단하게 몸풀기부터 하지.\n" +
                "<color=#EE1111>내가 있는 곳으로 와 보게나.</color>\n" +
                "W, A, S, D키를 누르면 움직일 수 있다네.");
        }
        else if (phase == 2 && !isPhaseStarted)
        {
            isPhaseStarted = true;
            tutorialText.text = "주인공은 페이지를 넘나들 수도 있습니다!\n" +
                "왼쪽 Shift를 눌러 이전 페이지로 가거나,\n스페이스 바를 눌러 다음 페이지로 갈 수 있습니다.\n" +
                "본인이 있는 페이지는 화면 뒤에 초록색 선으로,\n상대가 있는 페이지는 빨간색 선으로 표시됩니다.\n" +
                "\"상대가 있는 페이지로 캐릭터를 움직이세요.\"";
            redPage.enabled = true;
            redText.enabled = true;
            greenPage.enabled = true;
            greenBookText.enabled = true;
            book.enabled = true;
            foreach (GameObject g in WASD)
            {
                g.SetActive(false);
            }
            Transform enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
            enemy.SetPositionAndRotation(
                new Vector3(enemy.position.x, enemy.position.y, Boundary.RoundZ(enemy.position.z)), enemy.rotation);
        }
        else if (phase == 3 && !isPhaseStarted)
        {
            isPhaseStarted = true;
            tutorialText.text = "주인공은 페이지를 뚫고 다른 페이지로 칼을 던질 수 있습니다.\n" +
                "마우스 왼쪽을 눌렀다 떼면 앞 페이지로, 오른쪽을 눌렀다 떼면 뒤 페이지로 칼이 날아갑니다.\n" +
                "마우스를 누르고 있으면 조준점에 작은 시계가 나타납니다.\n" +
                "이 시계의 보라색 침은 칼이 향할 페이지를, 빨간색 침은 상대가 있는 페이지를 가리킵니다.\n" +
                "마우스를 오래 누를수록 더 먼 페이지로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                "<color=#ff00bf>마우스로 상대를 조준하고, 보라색 침과 빨간색 침이 겹칠 때까지 눌렀다가 떼세요.</color>\n" +
                "\"움직이지 않는 상대를 향해 칼을 던져서 3번 맞추세요.\"";

            Transform enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
        }

        if (phase == 2 && isPhaseStarted && 
            GetComponent<Transform>().position.z >= GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>().position.z)
        {
            NextPhase();
        }

    }
    
    public void NextPhase()
    {
        phase++;
        process = 0;
        isPhaseStarted = false;
    }

    public void NextProcess()
    {
        process++;
        isProcessReady = false;
    }

    private bool State(int phase, int process)
    {
        return (phase == this.phase && process == this.process);
    }

    private bool StateNotReady(int phase, int process)
    {
        return (State(phase, process) && !isProcessReady);
    }

    private void CreateBubble(string explanation)
    {
        if (myBubble != null)
        {
            Destroy(myBubble);
        }
        myBubble = Instantiate(tutorialBubble, Manager.instance.Canvas.GetComponent<Transform>());
        myBubble.GetComponentInChildren<Text>().text = explanation;
        isProcessReady = true;
    }
}
