using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

    public Text tutorialText;
    public GameObject destination;
    public Image redHand;

    private int phase;  // 0: XY평면 상 이동, 1: 시간축 상 이동, 2: 멈춘 적 맞추기
    private bool isStarted;   // phase 시작 전에 false

    public int Phase
    {
        get
        {
            return phase;
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
            tutorialText.text = "2차원 벡터공간에서 캐릭터를\nW(상), S(하), A(좌), D(우)로 움직일 수 있습니다.\n" + 
                "\"파란색 공이 있는 곳으로 캐릭터를 움직이세요.\"";
            Instantiate(destination, new Vector3(0.2f, 0.56f, 0f), Quaternion.identity);
            redHand.enabled = false;
        }
        else if (phase == 1 && !isStarted)
        {
            isStarted = true;
            tutorialText.text = "왼쪽 Shift를 눌러 과거로 가거나,\n스페이스 바를 눌러 미래로 갈 수 있습니다.\n" +
                "본인이 있는 시간은 초록색 침으로,\n상대가 있는 시간은 빨간색 침으로 표시됩니다.\n" +
                "\"상대가 있는 시간대로 캐릭터를 움직이세요.\"";
            redHand.enabled = true;
            Transform enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
            enemy.SetPositionAndRotation(
                new Vector3(enemy.position.x, enemy.position.y, Boundary.RoundZ(enemy.position.z)), enemy.rotation);
        }
        else if (phase == 2 && !isStarted)
        {
            isStarted = true;
            tutorialText.text = "마우스 왼쪽을 눌러 과거로, 또는 마우스 오른쪽을 눌러 미래로 칼을 던질 수 있습니다.\n" +
                "마우스를 누르고 있으면 작은 시계가 나타납니다.\n이 시계의 파란색 침은 칼이 향할 시간을 가리킵니다.\n" +
                "마우스를 오래 누를수록 더 먼 과거(미래)로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                "마우스로 상대를 조준하고 파란색 침과 빨간색 침이 겹칠 때까지 눌렀다가 떼세요.\n" +
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
