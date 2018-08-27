using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

    public Text tutorialText;
    public GameObject destination;
    public Image redHand;
    public Text redText;

    private int phase;  // 1: XY평면 상 이동, 2: 페이지 이동, 3: 상대 투사체 맞기, 4: 공격
    private int process;    // 0부터 시작, 각 페이즈에서의 진행 정도를 나타냄.
    private bool isStarted;   // phase 시작 전에 false

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
            return Phase == 1;
        }
    }

    public bool CanMoveZ
    {
        get
        {
            return Phase == 2;
        }
    }

	// Use this for initialization
	void Start () {
        phase = 0;
        isStarted = false;
	}
	
	void FixedUpdate () {
		if (phase == 0 && !isStarted)
        {
            isStarted = true;
            tutorialText.text = "당신은 만화 속의 주인공입니다!\n" +
                "컷을 넘나들며 주인공을\nW(상), S(하), A(좌), D(우)로 움직일 수 있습니다.\n" + 
                "\"파란색 공이 있는 곳으로 주인공을 움직이세요.\"";
            Instantiate(destination, new Vector3(0.2f, 0.56f, 0f), Quaternion.identity);
            redHand.enabled = false;
            redText.enabled = false;
        }
        else if (phase == 1 && !isStarted)
        {
            isStarted = true;
            tutorialText.text = "주인공은 페이지를 넘나들 수도 있습니다!\n" +
                "왼쪽 Shift를 눌러 이전 페이지로 가거나,\n스페이스 바를 눌러 다음 페이지로 갈 수 있습니다.\n" +
                "본인이 있는 페이지는 화면 뒤에 초록색 선으로,\n상대가 있는 페이지는 빨간색 선으로 표시됩니다.\n" +
                "\"상대가 있는 페이지로 캐릭터를 움직이세요.\"";
            redHand.enabled = true;
            redText.enabled = true;
            Transform enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
            enemy.SetPositionAndRotation(
                new Vector3(enemy.position.x, enemy.position.y, Boundary.RoundZ(enemy.position.z)), enemy.rotation);
        }
        else if (phase == 2 && !isStarted)
        {
            isStarted = true;
            tutorialText.text = "주인공은 페이지를 뚫고 다른 페이지로 칼을 던질 수 있습니다.\n" +
                "마우스 왼쪽을 눌렀다 떼면 앞 페이지로, 오른쪽을 눌렀다 떼면 뒤 페이지로 칼이 날아갑니다.\n" +
                "마우스를 누르고 있으면 조준점에 작은 시계가 나타납니다.\n" +
                "이 시계의 보라색 침은 칼이 향할 페이지를, 빨간색 침은 상대가 있는 페이지를 가리킵니다.\n" +
                "마우스를 오래 누를수록 더 먼 페이지로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                "<color=#ff00bf>마우스로 상대를 조준하고, 보라색 침과 빨간색 침이 겹칠 때까지 눌렀다가 떼세요.</color>\n" +
                "\"움직이지 않는 상대를 향해 칼을 던져서 3번 맞추세요.\"";

            Transform enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
        }

        if (phase == 1 && isStarted && 
            GetComponent<Transform>().position.z >= GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>().position.z)
        {
            NextPhase();
        }
	}

    public void NextPhase()
    {
        phase++;
        isStarted = false;
    }
}
