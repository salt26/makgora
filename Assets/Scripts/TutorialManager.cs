using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

    public Text tutorialText;
    public GameObject mentor;
    public BookUI book;
    public GameObject tutorialBubble;
    public List<GameObject> WASD;

    private int phase;  // 1: XY평면 상 이동, 2: 페이지 이동, 3: 상대 투사체 맞기, 4: 공격
    private int process;    // 0부터 시작, 각 페이즈에서의 진행 정도를 나타냅니다.
    private bool isPhaseStarted;    // 이것이 false가 되면 다음 프레임에 페이즈가 넘어갑니다.
    private bool isProcessReady;    // 이것이 false가 되면 다음 프레임에 true가 되면서 새 설명이 나타납니다.
    private bool isEnterAvailable;  // 이것이 true인 동안 Enter키를 눌러 다음 설명으로 넘어갈 수 있습니다.
    private GameObject myBubble;    // 현재 떠 있는 뾰족 말풍선(설명)을 가지고 있습니다.
    private GameObject myMentor;
    private Transform enemy;
    private Player player;

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
            return State(2, 4);
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
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.SetPageVisibleInTutorial(false);
        enemy.GetComponent<Enemy>().SetModelVisibleInTutorial(false);
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
            isPhaseStarted = true;  // 새 페이즈가 시작될 때에 true로 설정합니다.
            /*
            tutorialText.text = "당신은 만화 속의 주인공입니다!\n" +
                "컷을 넘나들며 주인공을\nW(상), S(하), A(좌), D(우)로 움직일 수 있습니다.\n" + 
                "\"파란색 공이 있는 곳으로 주인공을 움직이세요.\"";
                */
            myMentor = Instantiate(mentor, new Vector3(-1.5f, 0.76f, 0f), Quaternion.identity);

            book.redPage.GetComponent<Image>().enabled = false;
            book.redText.GetComponent<Text>().enabled = false;
            book.greenPage.GetComponent<Image>().enabled = false;
            book.greenText.GetComponent<Text>().enabled = false;
            book.GetComponent<Image>().enabled = false;
            CreateBubble("『Mak'Gora』의 세계에 온\n" +
                "것을 환영하네, 젊은이여!\n" +
                "내가 그대를 강인한\n" +
                "전사의 길로 인도할 것이다.\n" +
                "<color=#666699>(Enter키를 입력하면 넘어갑니다.)</color>");
            isEnterAvailable = true;    // Enter를 눌러 넘어갑니다.
        }
        else if (StateNotReady(1, 1))
        {
            foreach (GameObject g in WASD)
            {
                g.SetActive(true);
            }
            CreateBubble("자네는 컷 사이를 마음대로\n" +
                "넘어다닐 수 있다네.\n" +
                "<color=#EE1111>내가 있는 곳으로 와 보게나.</color>\n" +
                "W, A, S, D키를 누르면\n" +
                "움직일 수 있을 걸세.");
            // 여기서는 Enter를 눌러 넘어갈 수 없습니다.
        }
        else if (StateNotReady(1, 2))
        {
            foreach (GameObject g in WASD)
            {
                g.SetActive(false);
            }
            CreateBubble("잘했네.\n" +
                "이제 다음 단계로 넘어가지.");
            myMentor.GetComponent<TutorialMentor>().SetMoving(new Vector3(1.7f, 0.46f, 0f));
            // 여기서는 Enter를 눌러 넘어갈 수 없고, 멘토가 움직임을 멈춘 후 자동으로 넘어갑니다.
        }
        else if (StateNotReady(1, 3))
        {
            myMentor.GetComponent<TutorialMentor>().SetStop();
            NextPhase();
        }
        else if (phase == 2 && !isPhaseStarted)
        {
            isPhaseStarted = true;  // 새 페이즈가 시작될 때에 true로 설정합니다.
            /*
            tutorialText.text = "주인공은 페이지를 넘나들 수도 있습니다!\n" +
                "왼쪽 Shift를 눌러 이전 페이지로 가거나,\n스페이스 바를 눌러 다음 페이지로 갈 수 있습니다.\n" +
                "본인이 있는 페이지는 화면 뒤에 초록색 선으로,\n상대가 있는 페이지는 빨간색 선으로 표시됩니다.\n" +
                "\"상대가 있는 페이지로 캐릭터를 움직이세요.\"";
                */
            /*
            redPage.enabled = true;
            redText.enabled = true;
            */
            enemy.SetPositionAndRotation(
                new Vector3(enemy.position.x, enemy.position.y, Boundary.RoundZ(enemy.position.z)), enemy.rotation);
            myMentor.GetComponent<TutorialMentor>().SetPageVisible();
            book.greenPage.GetComponent<Image>().enabled = true;
            book.greenText.GetComponent<Text>().enabled = true;
            book.bluePage.GetComponent<Image>().enabled = true;
            book.blueText.GetComponent<Text>().enabled = true;
            book.GetComponent<Image>().enabled = true;
            player.SetPageVisibleInTutorial(true);
            book.mentor = myMentor.GetComponent<Transform>();
            CreateBubble("눈치챘을수도 있겠지만,\n" +
                "우리는 만화책 속 주인공이라네.\n" +
                "다른 페이지를 오가는 것도 가능하지!\n" +
                "<color=#666699>(Enter키 입력)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(2, 1))
        {
            CreateBubble("물론 사람들이 이 책을 펼치면\n" +
                "제자리로 돌아가 있어야겠지만...\n" +
                "걱정 말게. 이 책은 1년 넘게\n" +
                "아무도 읽지 않았으니.\n" +
                "<color=#666699>(Enter키 입력)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(2, 2) && myMentor != null && !myMentor.GetComponent<TutorialMentor>().StartMoving)
        {
            CreateBubble("페이지를 이동할 테니\n" +
                "내가 하는 것을 잘 보게.");
            myMentor.GetComponent<TutorialMentor>().SetMoving(new Vector3(1.7f, 0.46f, -2.5f));
        }
        else if (StateNotReady(2, 3))
        {
            CreateBubble("갑자기 사라져서 당황했나?\n" +
                "내가 어디에 있는지는 뒤에 있는\n" +
                "책 그림을 보면 알 수 있다네.\n" +
                "자네가 있는 페이지는 <color=#24DD00>초록색</color>으로,\n" +
                "내가 있는 페이지는 <color=#007ADD>하늘색</color>으로 보이지.\n" +
                "<color=#666699>(Enter키 입력)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(2, 4))
        {
            CreateBubble("<color=#EE1111>이제 내가 있는 페이지까지 와보게.</color>\n" +
                "왼쪽 Shift키 또는 Q키를 누르면\n" +
                "앞쪽 페이지로, Space키 또는 E키를\n" +
                "누르면 뒤쪽 페이지로 이동할 수 있다네.\n" +
                "가까이 오면 내 모습이 보일거야.");
            // 여기서는 Enter를 눌러 넘어갈 수 없습니다.
            // TODO 키보드 이미지 보여주기
        }
        else if (StateNotReady(2, 5))
        {
            CreateBubble("역시, 기대했던 대로야.\n" +
                "그럼 이제...\n" +
                "<color=#666699>(Enter키 입력)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(2, 6))
        {
            NextPhase();
        }
        else if (Phase == 3 && !isPhaseStarted)
        {
            isPhaseStarted = true;
            /*
            book.redPage.GetComponent<Image>().enabled = true;
            book.redText.GetComponent<Text>().enabled = true;
            */
            if (myBubble != null)
            {
                Destroy(myBubble);
            }
            enemy.GetComponent<Enemy>().SetModelVisibleInTutorial(true);
            StartCoroutine(ShootAndMiss(new Vector3(0.3f, 0.1f, 0f)));
        }
        else if (StateNotReady(3, 1))
        {
            CreateBubble("앗!\n" +
                "어디 다치진 않았나?\n" +
                "아무래도 어딘가 적이 있는 것 같군.\n" +
                "<color=#666699>(Enter키 입력)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(3, 2))
        {
            CreateBubble("방금 본 무기는 다른 페이지에서\n" +
                "페이지들을 뚫고 날아온 것이라네.\n" +
                "어디서 던졌는지\n" +
                "아직은 알 길이 없군.\n" +
                "<color=#666699>(Enter키 입력)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(3, 3))
        {
            CreateBubble("아까 봤는지 모르겠지만,\n" +
                "상대가 던진 무기는 자네와 같은\n" +
                "페이지에 있을 때 <color=#E71B50>빨간색</color>으로\n" +
                "보인다네. 맞으면 위험하다는 뜻이지.\n" +
                "<color=#666699>(Enter키 입력)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(3, 4))
        {
            CreateBubble("자네와의 페이지 상 거리가 멀수록\n" +
                   "더 희미하게 보이고, 자네와 다른\n" +
                   "페이지에서는 빨간색이 아닌 색을 띠지.\n" +
                   "무기에 자네와의 페이지\n" +
                   "차이가 표시되어 있을 걸세.\n" +
                   "<color=#666699>(Enter키 입력)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(3, 5))
        {
            CreateBubble("조심하게!");
            StartCoroutine(ShootAndMiss(new Vector3(0.3f, -0.3f, 0f)));
            StartCoroutine(ShootAndHit());
        }
        else if (StateNotReady(3, 6))
        {
            isProcessReady = true;
            if (myBubble != null)
            {
                Destroy(myBubble);
            }
            // Do nothing (Automatically skipped)
        }
        else if (StateNotReady(3, 7))
        {
            CreateBubble("이런! 결국에는 맞았군.\n" +
                "적을 어서 처리해야겠어.\n" +
                "하지만 그 전에 몇 가지\n" +
                "알아야 할 것들이 있다네.\n" +
                "<color=#666699>(Enter키 입력)</color>");
            isEnterAvailable = true;
        }
        /*
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

        }
        */

        if (State(1, 2) && myMentor != null && myMentor.GetComponent<TutorialMentor>().EndMoving)
        {
            // 멘토가 정해진 위치로 이동을 완료했을 때 다음 과정으로 넘어감
            NextProcess();
        }
        if (State(2, 2) && myMentor != null && myMentor.GetComponent<TutorialMentor>().EndMoving)
        {
            // 멘토가 정해진 위치로 이동을 완료했을 때 다음 과정으로 넘어감
            NextProcess();
        }

        if (State(2, 4) && isProcessReady && 
            GetComponent<Transform>().position.z <= myMentor.GetComponent<Transform>().position.z)
        {
            // 멘토가 있는 페이지와 같은 페이지에 도달했을 때 3페이즈로 넘어감
            NextProcess();
        }

    }
    
    public void NextPhase()
    {
        Debug.Log("NextPhase");
        phase++;
        process = 0;
        isPhaseStarted = false;
    }

    public void NextProcess()
    {
        Debug.Log("NextProcess");
        process++;
        isProcessReady = false;
    }

    IEnumerator ShootAndMiss(Vector3 error)
    {
        GameObject k = Instantiate(enemy.GetComponent<Enemy>().knife, enemy.position, Quaternion.identity);
        k.GetComponent<MeshRenderer>().enabled = false;
        k.GetComponent<Knife>().Initialize(1, 0, player.GetComponent<Transform>().position + error);
        yield return null;
        k.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(4f);
        NextProcess();
    }

    IEnumerator ShootAndHit()
    {
        yield return new WaitForSeconds(0.8f);
        GameObject k = Instantiate(enemy.GetComponent<Enemy>().knife, enemy.position, Quaternion.identity);
        k.GetComponent<MeshRenderer>().enabled = false;
        k.GetComponent<Knife>().Initialize(1, 0, player.GetComponent<Transform>().position);
        yield return null;
        k.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(4f);
        NextProcess();
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
