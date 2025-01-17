﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {
    
    public Text tDExplainText;
    public Text tDNextText;
    public Text shootingExplainText;
    public GameObject mentor;
    public BookUI book;
    public GameObject tutorialBubble;
    public GameObject tDExplainBubble;
    public GameObject tDExplainBubble2;
    public GameObject shootingExplainBubble;
    public GameObject arrow;
    public GameObject silence;
    public GameObject magic;
    public List<GameObject> WASD;
    public List<GameObject> leftShiftQSpaceE;
    public List<GameObject> mouseButtons;
    public List<GameObject> tDExplanations;
    public List<GameObject> shootingExplanations;
    public Color pressedColor;
    public Color magicBeforeColor;
    public Color magicAfterColor;
    public AudioClip silenceSound;
    public AudioClip magicSound;

    private int phase;  // 1: XY평면 상 이동, 2: 페이지 이동, 3: 상대 투사체 맞기, 4: 공격
    private int process;    // 0부터 시작, 각 페이즈에서의 진행 정도를 나타냅니다.
    private bool isPhaseStarted;    // 이것이 false가 되면 다음 프레임에 페이즈가 넘어갑니다.
    private bool isProcessReady;    // 이것이 false가 되면 다음 프레임에 true가 되면서 새 설명이 나타납니다.
    private bool isEnterAvailable;  // 이것이 true인 동안 Enter키를 눌러 다음 설명으로 넘어갈 수 있습니다.
    private bool isBackAvailable;   // 이것이 true인 동안 Backspace키를 눌러 이전 설명으로 넘어갈 수 있습니다.
    private bool stopAutoShooting;  // 이것이 true가 되면 자동 발사를 멈춥니다.
    private GameObject myBubble;    // 현재 떠 있는 뾰족 말풍선(설명)을 가지고 있습니다.
    private GameObject myMentor;
    private GameObject myArrow;     // 현재 떠 있는 화살표를 가지고 있습니다.
    private GameObject myArrow2;
    private GameObject mySilence;   // 현재 적에게 걸려 있는 침묵 이펙트를 가지고 있습니다.
    private GameObject myMagic;     // 현재 멘토가 사용하는 마법진 이펙트를 가지고 있습니다.
    private GameObject myMagic2;    // 현재 적에게 뜨는 마법진 이펙트를 가지고 있습니다.
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
            return State(1, 1) || State(5, 2);
        }
    }

    public bool CanMoveZ
    {
        get
        {
            return State(2, 4) || State(4, 2) || State(5, 2);
        }
    }

    public bool CanShoot
    {
        get
        {
            return State(4, 8) || State(5, 0) || State(5, 2);
        }
    }

	// Use this for initialization
	void Start () {
        phase = 1;
        process = 0;
        isPhaseStarted = false;
        isProcessReady = false;
        isEnterAvailable = false;
        isBackAvailable = false;
        stopAutoShooting = false;
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
        if (isBackAvailable && Input.GetKeyDown(KeyCode.Backspace))
        {
            isBackAvailable = false;
            PrevProcess();
            return;
        }

        #region 키보드 이미지 관련 코드

        if (WASD.Count > 0 && WASD[0].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.W))
            {
                WASD[0].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, WASD[0].GetComponent<Image>().color.a);
            }
            else
            {
                WASD[0].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, WASD[0].GetComponent<Image>().color.a);
            }
        }
        if (WASD.Count > 1 && WASD[1].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.A))
            {
                WASD[1].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, WASD[1].GetComponent<Image>().color.a);
            }
            else
            {
                WASD[1].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, WASD[1].GetComponent<Image>().color.a);
            }
        }
        if (WASD.Count > 2 && WASD[2].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.S))
            {
                WASD[2].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, WASD[2].GetComponent<Image>().color.a);
            }
            else
            {
                WASD[2].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, WASD[2].GetComponent<Image>().color.a);
            }
        }
        if (WASD.Count > 3 && WASD[3].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.D))
            {
                WASD[3].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, WASD[3].GetComponent<Image>().color.a);
            }
            else
            {
                WASD[3].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, WASD[3].GetComponent<Image>().color.a);
            }
        }

        if (leftShiftQSpaceE.Count > 0 && leftShiftQSpaceE[0].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                leftShiftQSpaceE[0].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, leftShiftQSpaceE[0].GetComponent<Image>().color.a);
            }
            else
            {
                leftShiftQSpaceE[0].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, leftShiftQSpaceE[0].GetComponent<Image>().color.a);
            }
        }
        if (leftShiftQSpaceE.Count > 1 && leftShiftQSpaceE[1].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                leftShiftQSpaceE[1].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, leftShiftQSpaceE[1].GetComponent<Image>().color.a);
            }
            else
            {
                leftShiftQSpaceE[1].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, leftShiftQSpaceE[1].GetComponent<Image>().color.a);
            }
        }
        if (leftShiftQSpaceE.Count > 2 && leftShiftQSpaceE[2].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                leftShiftQSpaceE[2].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, leftShiftQSpaceE[2].GetComponent<Image>().color.a);
            }
            else
            {
                leftShiftQSpaceE[2].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, leftShiftQSpaceE[2].GetComponent<Image>().color.a);
            }
        }
        if (leftShiftQSpaceE.Count > 3 && leftShiftQSpaceE[3].activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.E))
            {
                leftShiftQSpaceE[3].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, leftShiftQSpaceE[3].GetComponent<Image>().color.a);
            }
            else
            {
                leftShiftQSpaceE[3].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, leftShiftQSpaceE[3].GetComponent<Image>().color.a);
            }
        }

        if (mouseButtons.Count > 0 && mouseButtons[0].activeInHierarchy && CanShoot)
        {
            if (Input.GetMouseButton(0))
            {
                mouseButtons[0].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, mouseButtons[0].GetComponent<Image>().color.a);
            }
            else
            {
                mouseButtons[0].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, mouseButtons[0].GetComponent<Image>().color.a);
            }
        }
        if (mouseButtons.Count > 1 && mouseButtons[1].activeInHierarchy && CanShoot)
        {
            if (Input.GetMouseButton(1))
            {
                mouseButtons[1].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, mouseButtons[1].GetComponent<Image>().color.a);
            }
            else
            {
                mouseButtons[1].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, mouseButtons[1].GetComponent<Image>().color.a);
            }
        }
        #endregion

        #region Phase 1: 상하좌우 이동
        if (StateNotReady(1, 0) && !isPhaseStarted)
        {
            // phase 1, process 0
            isPhaseStarted = true;  // 새 페이즈가 시작될 때에 true로 설정합니다.

            foreach (GameObject g in WASD)
            {
                g.SetActive(false);
            }
            if (myMentor == null)
                myMentor = Instantiate(mentor, new Vector3(-1.3f, 0.76f, 0f), Quaternion.identity);
            if (myArrow != null) Destroy(myArrow);
            if (myArrow2 != null) Destroy(myArrow2);

            book.redPage.GetComponent<Image>().enabled = false;
            book.redText.GetComponent<Text>().enabled = false;
            book.greenPage.GetComponent<Image>().enabled = false;
            book.greenText.GetComponent<Text>().enabled = false;
            book.GetComponent<Image>().enabled = false;
            CreateBubble("『Mak'Gora』의 세계에 온\n" +
                "것을 환영하네, 젊은이여!\n" +
                "내가 그대를 강인한\n" +
                "전사의 길로 인도할 걸세.\n" +
                "<color=#666699>(Enter: 다음으로 넘어가기)</color>");
            isEnterAvailable = true;    // Enter를 눌러 넘어갑니다.
        }
        else if (StateNotReady(1, 1))
        {
            foreach (GameObject g in WASD)
            {
                g.SetActive(true);
            }
            myArrow = Instantiate(arrow, new Vector3(-1.3f, 1.36f, 0f), Quaternion.identity);
            myArrow2 = Instantiate(arrow);
            CreateBubble("자네는 컷 사이를 마음대로\n" +
                "넘어다닐 수 있다네.\n" +
                "<color=#EE1111>내가 있는 곳으로 와 보게나.</color>\n" +
                "W, A, S, D키를 누르면\n" +
                "움직일 수 있을 걸세.\n" +
                "<color=#666699>(Backspace: 이전 설명 보기)</color>");
            // 여기서는 Enter를 눌러 넘어갈 수 없습니다.
            isBackAvailable = true;
        }
        else if (StateNotReady(1, 2))
        {
            foreach (GameObject g in WASD)
            {
                g.SetActive(false);
            }
            Destroy(myArrow);
            Destroy(myArrow2);
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
        #endregion
        #region Phase 2: 페이지 이동
        else if (phase == 2 && !isPhaseStarted)
        {
            isPhaseStarted = true;  // 새 페이즈가 시작될 때에 true로 설정합니다.

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
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(2, 1))
        {
            CreateBubble("물론 사람들이 이 책을 펼치면\n" +
                "제자리로 돌아가 있어야겠지만...\n" +
                "걱정 말게. 이 책은 1년 넘게\n" +
                "아무도 읽지 않았으니.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(2, 2) && myMentor != null && !myMentor.GetComponent<TutorialMentor>().StartMoving)
        {
            CreateBubble("페이지를 이동할 테니\n" +
                "내가 하는 것을 잘 보게.");
            myMentor.GetComponent<TutorialMentor>().SetMoving(new Vector3(1.7f, 0.46f, -2.5f));
        }
        else if (StateNotReady(2, 3))
        {
            if (myArrow != null) Destroy(myArrow);
            foreach (GameObject g in leftShiftQSpaceE)
            {
                g.SetActive(false);
            }
            myArrow = Instantiate(arrow, new Vector3(0f, 1f, player.GetComponent<Transform>().position.z), Quaternion.identity);
            CreateBubble("갑자기 사라져서 당황했나?\n" +
                "내가 어디에 있는지는 뒤에 있는\n" +
                "책 그림을 보면 알 수 있다네.\n" +
                "자네가 있는 페이지는 <color=#24DD00>초록색</color>으로,\n" +
                "내가 있는 페이지는 <color=#007ADD>하늘색</color>으로 보이지.\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(2, 4))
        {
            Destroy(myArrow);
            foreach (GameObject g in leftShiftQSpaceE)
            {
                g.SetActive(true);
            }
            CreateBubble("<color=#EE1111>이제 내가 있는 페이지까지 와보게.</color>\n" +
                "왼쪽 Shift키 또는 Q키를 누르면\n" +
                "앞쪽 페이지로, Space키 또는 E키를\n" +
                "누르면 뒤쪽 페이지로 이동할 수 있다네.\n" +
                "가까이 오면 내 모습이 보일거야.\n" +
                "<color=#666699>(Backspace: 이전)</color>");
            // 여기서는 Enter를 눌러 넘어갈 수 없습니다.
            isBackAvailable = true;
        }
        else if (StateNotReady(2, 5))
        {
            foreach (GameObject g in leftShiftQSpaceE)
            {
                g.SetActive(false);
            }
            book.bluePage.GetComponent<Image>().enabled = false;
            book.blueText.GetComponent<Text>().enabled = false;
            tDExplanations[0].SetActive(false);
            CreateBubble("역시, 기대했던 대로야.\n" +
                "자네는 앞으로 3페이지,\n" +
                "뒤로 3페이지 이내에 있는\n" +
                "가까운 대상만 볼 수 있다네.\n" +
                "그래서 아까 나를 볼 수 없었지.\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(2, 6))
        {
            isProcessReady = true;
            if (myBubble != null)
            {
                Destroy(myBubble);
            }
            tDExplanations[0].SetActive(true);
            tDExplanations[1].SetActive(true);
            tDExplainBubble.SetActive(false);
            tDExplainBubble2.SetActive(false);
            StartCoroutine(PageFadeIn(tDExplanations[0]));   // TODO 화면 전환 애니메이션 구현
        }
        else if (StateNotReady(2, 7))
        {
            isProcessReady = true;
            tDExplanations[1].SetActive(true);
            tDExplanations[2].SetActive(false);
            tDExplainBubble.SetActive(true);
            tDExplainBubble2.SetActive(true);
            tDExplainText.text = "이 만화책이 어떻게\n" +
                "생겼는지 다른\n" +
                "시점에서 설명하지.";
            tDNextText.text = "<color=#666699>(Enter: 다음)</color>";
            tDExplainBubble.GetComponent<AudioSource>().Play();
            isEnterAvailable = true;
        }
        else if (StateNotReady(2, 8))
        {
            isProcessReady = true;
            tDExplanations[1].SetActive(false);
            tDExplanations[2].SetActive(true);
            tDExplanations[3].SetActive(false);
            tDExplainText.text = "사실 이곳은 3차원\n" +
                "세계다. 각 페이지는\n" +
                "같은 크기이고, 같은\n" +
                "간격만큼 떨어져 있다네.";
            tDNextText.text = "<color=#666699>(Enter: 다음)\n" +
                "(Backspace: 이전)</color>";
            tDExplainBubble.GetComponent<AudioSource>().Play();
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(2, 9))
        {
            isProcessReady = true;
            tDExplanations[2].SetActive(false);
            tDExplanations[3].SetActive(true);
            tDExplanations[4].SetActive(false);
            tDExplainText.text = "자네에게 보이는\n" +
                "화면에는 원근법이\n" +
                "적용되어 있어.";
            tDExplainBubble.GetComponent<AudioSource>().Play();
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(2, 10))
        {
            isProcessReady = true;
            tDExplanations[3].SetActive(false);
            tDExplanations[4].SetActive(true);
            tDExplanations[5].SetActive(false);
            tDExplainText.text = "자네가 있는 페이지는\n" +
                "전부 보이지만...";
            tDExplainBubble.GetComponent<AudioSource>().Play();
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(2, 11))
        {
            isProcessReady = true;
            tDExplanations[4].SetActive(false);
            tDExplanations[5].SetActive(true);
            tDExplanations[6].SetActive(false);
            tDExplainText.text = "앞 페이지에 있는 대상은\n" +
                "더 크게 보이거나, 화면\n" +
                "밖에 있을 수도 있네.";
            tDExplainBubble.GetComponent<AudioSource>().Play();
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(2, 12))
        {
            isProcessReady = true;
            tDExplanations[5].SetActive(false);
            tDExplanations[6].SetActive(true);
            tDExplainText.text = "뒤 페이지에 있는 대상은\n" +
                "당연히 작게 보이겠지.";
            tDExplainBubble.GetComponent<AudioSource>().Play();
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(2, 13))
        {
            isProcessReady = true;
            tDExplainBubble.SetActive(false);
            tDExplainBubble2.SetActive(false);
            StartCoroutine(PageFadeOut(tDExplanations[0]));   // TODO 화면 전환 애니메이션 구현
        }
        else if (StateNotReady(2, 14))
        {
            tDExplanations[6].SetActive(false);
            tDExplanations[0].SetActive(false);
            CreateBubble("설명이 다 되었나?\n" +
                "그럼 이제...\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(2, 15))
        {
            NextPhase();
        }
        #endregion
        #region Phase 3: 피격
        else if (Phase == 3 && !isPhaseStarted)
        {
            isPhaseStarted = true;
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
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(3, 2))
        {
            CreateBubble("방금 본 무기는 다른 페이지에서,\n" +
                "페이지들을 뚫고 날아온 것이라네.\n" +
                "누가 어디서 던졌는지\n" +
                "아직은 감이 오지 않는군.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(3, 3))
        {
            CreateBubble("아까 봤는지 모르겠지만,\n" +
                "적이 던진 무기는 자네와\n" +
                "같은 페이지에 있을 때\n" +
                "<color=#E71B50>빨간색</color>으로 보인다네.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(3, 4))
        {
            CreateBubble("<color=#E71B50>빨간색</color>으로 보이는 무기는\n" +
                "잘못하다가는 맞을 수 있어서\n" +
                "위험하지. 그러나 무기가 항상\n" +
                "빨간색을 띠지는 않는다네.");
            StartCoroutine(ShootAndMiss(new Vector3(-0.2f, 0.55f, 0f)));
        }
        else if (StateNotReady(3, 5))
        {
            CreateBubble("<color=#E71B50>빨간색</color>으로 보이는 무기는\n" +
                "잘못하다가는 맞을 수 있어서\n" +
                "위험하지. 그러나 무기가 항상\n" +
                "빨간색을 띠지는 않는다네.\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(3, 6))
        {
            CreateBubble("적 무기가 자네와 다른 페이지에\n" +
                   "있으면 <color=#387399>빨간색이 아닌 색</color>을 띠지.\n" +
                   "미리 보고 피하게.\n" +
                   "무기에는 자네와의 페이지\n" +
                   "차이가 표시되어 있을 걸세.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(3, 7))
        {
            CreateBubble("조심하게!");
            StartCoroutine(ShootAndMiss(new Vector3(0.3f, -0.3f, 0f)));
            StartCoroutine(ShootAndHit());
            StartCoroutine(ShootAndHit2());
        }
        else if (StateNotReady(3, 8))
        {
            isProcessReady = true;
            if (myBubble != null)
            {
                Destroy(myBubble);
            }
            // Do nothing (Automatically skipped)
        }
        else if (StateNotReady(3, 9))
        {
            CreateBubble("이런! 아프겠군.\n" +
                "어서 적을 처리하러 가야겠어.\n" +
                "하지만 그 전에 몇 가지\n" +
                "알아야 할 것들이 있다네.\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(3, 10))
        {
            if (myArrow != null) Destroy(myArrow);
            CreateBubble("방금 자네에게 생긴 보호막을 보았나?\n" +
                "적에게 공격받으면 3초 동안\n" +
                "<color=#DDD180>보호막</color>이 생기고, 이 동안에는\n" +
                "어떤 공격에도 피해를 받지 않는다네.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(3, 11))
        {
            if (myArrow != null) Destroy(myArrow);
            myArrow = Instantiate(arrow, new Vector3(-0.3f, -1.2f, -2.5f), Quaternion.identity);
            CreateBubble("이번엔 책 아래를 보게.\n" +
                "초록색, 빨간색 하트가 있을 거야.\n" +
                "<color=#00FF00>초록색</color>은 자네의 체력이고\n" +
                "<color=#FF0000>빨간색</color>은 적의 체력이라네.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(3, 12))
        {
            CreateBubble("체력은 3에서 시작하고,\n" +
                "자네는 아까 맞았으니 2가 되었군.\n" +
                "조심하게. 체력이 0이 되면\n" +
                "전투에서 패배한다고.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(3, 13))
        {
            Destroy(myArrow);
            NextPhase();
        }
        #endregion
        #region Phase 4: 공격 설명
        else if (Phase == 4 && !isPhaseStarted)
        {
            isPhaseStarted = true;
            isEnterAvailable = false;

            if (myBubble != null)
            {
                Destroy(myBubble);
            }
            enemy.GetComponent<Enemy>().SetModelVisibleInTutorial(true);
            StartCoroutine(ShootAndMiss(new Vector3(-0.4f, -0.1f, 0f)));
            StartCoroutine(ShootAndHit());
        }
        else if (StateNotReady(4, 1))
        {

        }
        else if (StateNotReady(4, 2))
        {
            // TODO 페이지 이동 키보드 이미지 띄우기
            foreach (GameObject g in leftShiftQSpaceE)
            {
                g.SetActive(true);
            }
            book.redPage.GetComponent<Image>().enabled = true;
            book.redText.GetComponent<Text>().enabled = true;
            CreateBubble("자네를 공격한 적의\n" +
                "위치를 알아냈다네!\n" +
                "책 그림에서 <color=#DD0015>빨간색</color>으로 표시된\n" +
                "페이지가 적이 있는 페이지일세.\n" +
                "<color=#EE1111>적이 있는 페이지로 가보자고.</color>");

            // 여기서는 Enter를 눌러 넘어갈 수 없습니다.
        }
        else if (StateNotReady(4, 3))
        {
            isEnterAvailable = false;
            foreach (GameObject g in leftShiftQSpaceE)
            {
                g.SetActive(false);
            }
            CreateBubble("여기 있었군!\n" +
                "얌전히 있거라...");
            StartCoroutine(SilenceEnemy());
            // 침묵은 무기 소환을 하지 못하는 상태 이상입니다.
            // 여기서는 Enter를 눌러 넘어갈 수 없습니다.
        }
        else if (StateNotReady(4, 4))
        {
            CreateBubble("이 놈이 무기를 소환하지\n" +
                "못하게 막았네.\n" +
                "자네의 망치로 저 놈을 공격하게나!\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(4, 5))
        {
            isProcessReady = true;
            isEnterAvailable = false;
            if (myBubble != null)
            {
                Destroy(myBubble);
            }
            enemy.GetComponent<Enemy>().SpeakTutorial(2);
            StartCoroutine(Wait(2f));
            // 시간이 지나면 자동으로 넘어갑니다.
        }
        else if (StateNotReady(4, 6))
        {
            CreateBubble("아차, 망치를 소환하는 법을\n" +
                "내가 안 알려줬던가?\n" +
                "중요한 걸 깜박하는 걸 보니\n" +
                "나도 이제 늙었나 보군...\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(4, 7))
        {
            CreateBubble("자네와 <color=#337755>같은 페이지</color>에 있는\n" +
                "적에게 무기를 던질 때에는\n" +
                "마우스의 양쪽 버튼을\n" +
                "모두 누르면 된다네.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(4, 8))
        {
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(true);
            }
            myArrow = Instantiate(arrow, new Vector3(0.39f, 1.57f, 2f), Quaternion.identity);
            CreateBubble("<color=#EE1111>마우스의 양쪽 버튼을 모두\n" +
                "누른 상태에서, 저 오크를\n" +
                "마우스로 정확히 조준하고\n" +
                "버튼을 떼 보게나.</color>");
            // 여기서는 Enter를 눌러 넘어갈 수 없습니다.
        }
        else if (StateNotReady(4, 9))
        {
            isProcessReady = true;
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(false);
            }
            if (myArrow != null)
            {
                Destroy(myArrow);
            }
            if (myBubble != null)
            {
                Destroy(myBubble);
            }
            StartCoroutine(Wait(3f));
        }
        else if (StateNotReady(4, 10))
        {
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(true);
            }
            myArrow = Instantiate(arrow, player.GetComponent<Transform>().position + new Vector3(-0.25f, 0.3f, 0f), Quaternion.identity);
            CreateBubble("어떤가? 아직 어렵나?\n" +
                "무기를 던지려면 먼저 소환해야 하네.\n" +
                "무기가 다 소환될 때까지는\n" +
                "조준만 할 수 있지.");
            stopAutoShooting = false;
            StartCoroutine(AutoShoot1());
            // 여기서는 Enter를 눌러 넘어갈 수 없습니다.
        }
        else if (StateNotReady(4, 11))
        {
            CreateBubble("어떤가? 아직 어렵나?\n" +
                "무기를 던지려면 먼저 소환해야 하네.\n" +
                "무기가 다 소환될 때까지는\n" +
                "조준만 할 수 있지.\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(4, 12))
        {
            CreateBubble("소환이 완료되기 전에\n" +
                "마우스 버튼을 모두 떼면\n" +
                "소환이 끝나자마자 무기가\n" +
                "조준점을 향해 날아갈 걸세.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(4, 13))
        {
            stopAutoShooting = true;
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(false);
            }
            if (myArrow != null)
            {
                Destroy(myArrow);
            }
            CreateBubble("물론 소환이 끝나도 바로\n" +
                "던지지 않고 계속 조준을 할\n" +
                "수 있다네. 조준을 오래 해도\n" +
                "무기의 속력은 같지만 말야.");
            StartCoroutine(Wait(2.1f));
            // 시간이 지나면 자동으로 넘어갑니다.
        }
        else if (StateNotReady(4, 14))
        {
            CreateBubble("물론 소환이 끝나도 바로\n" +
                "던지지 않고 계속 조준을 할\n" +
                "수 있다네. 조준을 오래 해도\n" +
                "무기의 속력은 같지만 말야.\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(4, 15))
        {
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(true);
            }
            CreateBubble("이렇게 말일세.\n" +
                "마우스 버튼 그림과\n" +
                "조준점에 놓인 시계를 잘 보게.");
            stopAutoShooting = false;
            StartCoroutine(AutoShoot2());
            // 여기서는 Enter를 눌러 넘어갈 수 없습니다.
        }
        else if (StateNotReady(4, 16))
        {
            CreateBubble("이렇게 말일세.\n" +
                "마우스 버튼 그림과\n" +
                "조준점에 놓인 시계를 잘 보게.\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(4, 17))
        {
            CreateBubble("무기 소환이 다 되면\n" +
                "시계 중앙의 원이 사라지고\n" +
                "시계 테두리가 검은색이 되지.\n" +
                "그러면 언제든 던질 수 있다고.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(4, 18))
        {
            stopAutoShooting = true;
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(false);
            }
            CreateBubble("아까 상대가 다른 페이지에서\n" +
                "무기를 던졌던 것을 기억하나?\n" +
                "자네도 다른 페이지로\n" +
                "무기를 던질 수 있다네.");
            StartCoroutine(Wait(2.7f));
            // 시간이 지나면 자동으로 넘어갑니다.
        }
        else if (StateNotReady(4, 19))
        {
            CreateBubble("아까 상대가 다른 페이지에서\n" +
                "무기를 던졌던 것을 기억하나?\n" +
                "자네도 다른 페이지로\n" +
                "무기를 던질 수 있다네.\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(4, 20))
        {
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(true);
            }
            CreateBubble("마우스 왼쪽 버튼을 누르고\n" +
                "있으면 앞 페이지를 조준하고,\n" +
                "마우스 오른쪽 버튼을 누르고\n" +
                "있으면 뒤 페이지를 조준하지.\n" +
                "오래 누를수록 먼 곳을 조준한다네.");
            stopAutoShooting = false;
            StartCoroutine(AutoShoot3());
            // 여기서는 Enter를 눌러 넘어갈 수 없습니다.
        }
        else if (StateNotReady(4, 21))
        {
            CreateBubble("마우스 왼쪽 버튼을 누르고\n" +
                "있으면 앞 페이지를 조준하고,\n" +
                "마우스 오른쪽 버튼을 누르고\n" +
                "있으면 뒤 페이지를 조준하지.\n" +
                "오래 누를수록 먼 곳을 조준한다네.\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(4, 22))
        {
            CreateBubble("시계의 <color=#B400B4>보라색 침</color>이 자네가\n" +
                "조준하는 페이지를 가리킨다네.\n" +
                "시계의 <color=#FF0022>빨간색 침</color>은 상대가\n" +
                "위치한 페이지를 가리키고.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(4, 23))
        {
            stopAutoShooting = true;
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(false);
            }
            CreateBubble("여기까지 잘 따라왔다면\n" +
                "적을 맞추기 위해\n" +
                "어떻게 조준해야 하는지\n" +
                "알 거라 믿는다만...");
            StartCoroutine(Wait(2.6f));
            // 시간이 지나면 자동으로 넘어갑니다.
        }
        else if (StateNotReady(4, 24))
        {
            CreateBubble("여기까지 잘 따라왔다면\n" +
                "적을 맞추기 위해\n" +
                "어떻게 조준해야 하는지\n" +
                "알 거라 믿는다만...\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(4, 25))
        {
            isProcessReady = true;
            if (myBubble != null)
            {
                Destroy(myBubble);
            }
            shootingExplanations[0].SetActive(true);
            shootingExplanations[1].SetActive(true);
            shootingExplainBubble.SetActive(false);
            StartCoroutine(PageFadeIn(shootingExplanations[0]));   // TODO 화면 전환 애니메이션 구현
        }
        else if (StateNotReady(4, 26))
        {
            isProcessReady = true;
            shootingExplanations[1].SetActive(true);
            shootingExplanations[2].SetActive(false);
            shootingExplainBubble.SetActive(true);
            shootingExplainText.text = "페이지들을 화면 오른쪽에서 본 모습이다.\n" +
                "<color=#666699>(Enter: 다음)</color>";
            shootingExplainBubble.GetComponent<AudioSource>().Play();
            isEnterAvailable = true;
        }
        else if (StateNotReady(4, 27))
        {
            isProcessReady = true;
            shootingExplanations[1].SetActive(false);
            shootingExplanations[2].SetActive(true);
            shootingExplanations[3].SetActive(false);
            shootingExplainText.text = "자네가 24페이지 초록색 점의\n" +
                "위치에 있다고 하자.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>";
            shootingExplainBubble.GetComponent<AudioSource>().Play();
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(4, 28))
        {
            isProcessReady = true;
            shootingExplanations[2].SetActive(false);
            shootingExplanations[3].SetActive(true);
            shootingExplanations[4].SetActive(false);
            shootingExplainText.text = "만약 자네가 26페이지의 보라색 점을\n" +
                "조준해서 무기를 던지면...\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>";
            shootingExplainBubble.GetComponent<AudioSource>().Play();
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(4, 29))
        {
            isProcessReady = true;
            shootingExplanations[3].SetActive(false);
            shootingExplanations[4].SetActive(true);
            shootingExplanations[5].SetActive(false);
            shootingExplainText.text = "무기는 26페이지에서 멈추지 않고\n" +
                "직선으로 쭉 날아간다네.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>";
            shootingExplainBubble.GetComponent<AudioSource>().Play();
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(4, 30))
        {
            isProcessReady = true;
            shootingExplanations[4].SetActive(false);
            shootingExplanations[5].SetActive(true);
            shootingExplainText.text = "운이 좋으면 다른 페이지에 있던\n" +
                "적이 맞을 수도 있겠지.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>";
            shootingExplainBubble.GetComponent<AudioSource>().Play();
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(4, 31))
        {
            isProcessReady = true;
            shootingExplainBubble.SetActive(false);
            StartCoroutine(PageFadeOut(shootingExplanations[0]));   // TODO 화면 전환 애니메이션 구현
        }
        else if (StateNotReady(4, 32))
        {
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(true);
            }
            shootingExplanations[5].SetActive(false);
            shootingExplanations[0].SetActive(false);
            CreateBubble("마지막으로 꿀팁 하나\n" +
                "알려주지. 자네가 던진 망치가\n" +
                "적이 있는 페이지를 지나면\n" +
                "그 지점에 균열이 발생한다네.");
            stopAutoShooting = false;
            StartCoroutine(AutoShoot4());
            // 여기서는 Enter를 눌러 넘어갈 수 없습니다.
        }
        else if (StateNotReady(4, 33))
        {
            CreateBubble("마지막으로 꿀팁 하나\n" +
                "알려주지. 자네가 던진 망치가\n" +
                "적이 있는 페이지를 지나면\n" +
                "그 지점에 균열이 발생한다네.\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(4, 34))
        {
            CreateBubble("망치가 원하는 곳으로 잘\n" +
                "날아갔는지 확인하고 싶을 때\n" +
                "균열의 위치를 보면 된다고.\n" +
                "<color=#666699>(Enter: 다음 / Backspace: 이전)</color>");
            isEnterAvailable = true;
            isBackAvailable = true;
        }
        else if (StateNotReady(4, 35))
        {
            stopAutoShooting = true;
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(false);
            }
            CreateBubble("과연 내 도움 없이\n" +
                "무기를 다룰 수 있을까...");
            StartCoroutine(Wait(3.1f));
            // 시간이 지나면 자동으로 넘어갑니다.
        }
        else if (StateNotReady(4, 36))
        {
            NextPhase();
        }
        #endregion
        #region Phase 5: 직접 공격
        else if (Phase == 5 && !isPhaseStarted)
        {
            isPhaseStarted = true;
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(true);
            }
            myArrow = Instantiate(arrow, new Vector3(-0.8f, 0.6f, Boundary.RoundZ(3f)), Quaternion.identity);
            CreateBubble("<color=#EE1111>드디어 적을 공격할 시간이네.</color>\n" +
                "적이 지금 자네보다 뒤 페이지에\n" +
                "있으니 마우스 오른쪽 버튼으로\n" +
                "적을 조준하고 시계의 두 침이\n" +
                "겹칠 때 마우스를 떼면 될 걸세.");
            // 여기서는 Enter로 넘어갈 수 없습니다.
        }
        else if (StateNotReady(5, 1))
        {
            isProcessReady = true;
            if (myArrow != null)
            {
                Destroy(myArrow);
            }
            if (myBubble != null)
            {
                Destroy(myBubble);
            }
            foreach (GameObject g in mouseButtons)
            {
                g.SetActive(false);
            }
            StartCoroutine(Wait(3f));
        }
        else if (StateNotReady(5, 2))
        {
            CreateBubble("잘했군. 적이 도망갔네.\n" +
                "<color=#EE1111>적을 찾아 쓰러뜨리게.</color>\n" +
                "이제 자네는 자유롭게 컷을\n" +
                "넘나들고, 페이지를 가로지르며\n" +
                "무기를 던질 수 있다네.");
            // 여기서는 Enter로 넘어갈 수 없습니다.
        }
        else if (StateNotReady(5, 3))
        {
            isProcessReady = true;
            if (myBubble != null)
            {
                Destroy(myBubble);
            }
            if (mySilence != null)
            {
                Destroy(mySilence);
            }
            StartCoroutine(Wait(2.5f));
        }
        else if (StateNotReady(5, 4))
        {
            CreateBubble("수고했다.\n" +
                "이제 내가 가르칠 것은 없네.\n" +
                "막고라를 하면서 성장하고,\n" +
                "강한 오크가 되거라.\n" +
                "언젠가 다시 볼 일이 있겠지...\n" +
                "<color=#666699>(Enter: 다음)</color>");
            isEnterAvailable = true;
        }
        else if (StateNotReady(5, 5))
        {
            isProcessReady = true;
            if (myBubble != null)
            {
                Destroy(myBubble);
            }
            Manager.instance.GraduateTutorial();
        }
        #endregion

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

        if (State(4, 2) && isProcessReady &&
            Boundary.ZToPage(GetComponent<Transform>().position.z) > 
            Boundary.ZToPage(myMentor.GetComponent<Transform>().position.z) &&
            myMentor.GetComponent<TutorialMentor>().EndMoving)
        {
            // 멘토가 내 페이지를 따라서 페이지 이동을 함
            myMentor.GetComponent<TutorialMentor>().SetMoving(new Vector3(1.7f, 0.46f, Boundary.RoundZ(GetComponent<Transform>().position.z)));
        }

        if (State(4, 2) && isProcessReady &&
            GetComponent<Transform>().position.z >= enemy.position.z)
        {
            // 적이 있는 페이지와 같은 페이지에 도달했을 때 3페이즈로 넘어감
            myMentor.GetComponent<TutorialMentor>().SetMoving(new Vector3(1.7f, 0.46f, Boundary.RoundZ(enemy.position.z)));
            NextProcess();
        }

    }
    
    public void NextPhase()
    {
        Debug.Log("NextPhase");
        phase++;
        process = 0;
        Manager.instance.Canvas.GetComponentInParent<InGameUI>().UpdatePhase(phase);
        isPhaseStarted = false;
    }

    public void NextProcess()
    {
        Debug.Log("NextProcess");
        process++;
        isProcessReady = false;
        isBackAvailable = false;
    }
    
    public void PrevProcess()
    {
        Debug.Log("PrevProcess");
        process--;
        isProcessReady = false;
        isEnterAvailable = false;
        if (process == 0) isPhaseStarted = false;
    }

    IEnumerator ShootAndMiss(Vector3 error)
    {
        GameObject k = Instantiate(enemy.GetComponent<Enemy>().knife, enemy.position, Quaternion.identity);
        k.GetComponent<MeshRenderer>().enabled = false;
        k.GetComponent<Knife>().Initialize(1, player.GetComponent<Transform>().position + error);
        yield return null;
        k.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(2f);
        k.GetComponent<AudioSource>().spatialBlend=0.1f;
        k.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1f);
        NextProcess();
    }

    IEnumerator ShootAndHit()
    {
        yield return new WaitForSeconds(0.8f);
        GameObject k = Instantiate(enemy.GetComponent<Enemy>().knife, enemy.position, Quaternion.identity);
        k.GetComponent<MeshRenderer>().enabled = false;
        k.GetComponent<Knife>().Initialize(1, player.GetComponent<Transform>().position);
        yield return null;
        k.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(4.5f);
        NextProcess();
    }

    IEnumerator ShootAndHit2()
    {
        yield return new WaitForSeconds(1.6f);
        GameObject k = Instantiate(enemy.GetComponent<Enemy>().knife, enemy.position, Quaternion.identity);
        k.GetComponent<MeshRenderer>().enabled = false;
        k.GetComponent<Knife>().Initialize(1, player.GetComponent<Transform>().position);
        yield return null;
        k.GetComponent<MeshRenderer>().enabled = true;
    }
    
    IEnumerator SilenceEnemy()
    {
        // TODO 마법진 소리 재생
        yield return new WaitForSeconds(1f);
        GetComponent<AudioSource>().clip = magicSound;
        GetComponent<AudioSource>().Play();
        myMagic = Instantiate(magic, myMentor.GetComponent<Transform>());
        myMagic2 = Instantiate(magic, enemy);
        int frame = 120;
        for (int i = 0; i < frame; i++)
        {
            myMagic.GetComponent<SpriteRenderer>().color = Color.Lerp(magicBeforeColor, magicAfterColor, i / (float)frame);
            myMagic.GetComponent<Transform>().localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.5f, 1.5f, 1.5f), i / (float)frame);
            myMagic2.GetComponent<SpriteRenderer>().color = Color.Lerp(magicBeforeColor, magicAfterColor, i / (float)frame);
            myMagic2.GetComponent<Transform>().localScale = Vector3.Lerp(Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f), i / (float)frame);
            myMagic2.GetComponent<Transform>().localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0f, 0f, 180f), i / (float)frame);
            yield return null;
        }
        frame = 30;
        for (int i = 0; i < frame; i++)
        {
            myMagic2.GetComponent<Transform>().localScale = Vector3.Lerp(new Vector3(0.5f, 0.5f, 0.5f), Vector3.zero, i / (float)frame);
            myMagic2.GetComponent<Transform>().localRotation = Quaternion.Slerp(Quaternion.Euler(0f, 0f, 180f), Quaternion.identity, i / (float)frame);
            yield return null;
        }
        Destroy(myMagic);
        Destroy(myMagic2);
        // TODO 침묵 소리 재생
        GetComponent<AudioSource>().clip = silenceSound;
        GetComponent<AudioSource>().Play();
        mySilence = Instantiate(silence, enemy);
        yield return new WaitForSeconds(0.7f);
        enemy.GetComponent<Enemy>().SpeakTutorial(1);
        yield return new WaitForSeconds(0.3f);
        if (myBubble != null)
        {
            Destroy(myBubble);
        }
        yield return new WaitForSeconds(2.1f);
        NextProcess();
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        NextProcess();
    }

    IEnumerator PageFadeIn(GameObject panel)
    {
        int frame = 60;
        for (int j = 0; j < frame; j++)
        {
            float alpha = Mathf.Lerp(0f, 1f, j / (float)frame);
            Color c = Color.Lerp(ColorUtil.instance.pastColor, Color.white, j / (float)frame);
            Vector3 scale = Vector3.Lerp(new Vector3(1.2f, 1.2f, 1f), new Vector3(1f, 1f, 1f), j / (float)frame);
            Vector3 scale2 = Vector3.Lerp(new Vector3(1.1f, 1.1f, 1f), new Vector3(1f, 1f, 1f), j / (float)frame);
            panel.GetComponent<Image>().color = ColorUtil.instance.AlphaColor(c, alpha);
            panel.GetComponent<Transform>().localScale = scale;
            foreach (Image i in panel.GetComponentsInChildren<Image>())
            {
                i.color = ColorUtil.instance.AlphaColor(Color.white, alpha);
                i.GetComponent<Transform>().localScale = scale2;
            }
            yield return null;
        }
        NextProcess();
    }

    IEnumerator PageFadeOut(GameObject panel)
    {
        int frame = 60;
        for (int j = 0; j < frame; j++)
        {
            float alpha = Mathf.Lerp(1f, 0f, j / (float)frame);
            Color c = Color.Lerp(Color.white, ColorUtil.instance.pastColor, j / (float)frame);
            Vector3 scale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(1.2f, 1.2f, 1f), j / (float)frame);
            Vector3 scale2 = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(1.1f, 1.1f, 1f), j / (float)frame);
            panel.GetComponent<Image>().color = ColorUtil.instance.AlphaColor(c, alpha);
            panel.GetComponent<Transform>().localScale = scale;
            foreach (Image i in panel.GetComponentsInChildren<Image>())
            {
                i.color = ColorUtil.instance.AlphaColor(Color.white, alpha);
                i.GetComponent<Transform>().localScale = scale2;
            }
            yield return null;
        }
        NextProcess();
    }

    IEnumerator AutoShoot1()
    {
        AutoLeft(false);
        AutoRight(false);
        yield return new WaitForSeconds(1f);
        // TODO 마우스 버튼 보이기
        player.SetAutoShootInTutorial(new Vector3(-0.7f, -0.6f, 2f));
        AutoLeft(true);
        AutoRight(true);
        yield return new WaitForSeconds(0.4f);
        AutoLeft(false);
        AutoRight(false);
        yield return new WaitForSeconds(1.6f);
        player.SetAutoShootInTutorial(new Vector3(-0.4f, 1.2f, 2f));
        AutoLeft(true);
        AutoRight(true);
        yield return new WaitForSeconds(0.2f);
        AutoLeft(false);
        AutoRight(false);
        yield return new WaitForSeconds(1.8f);
        NextProcess();
        for (int i = 0; i < 12; i++)
        {
            if (stopAutoShooting) break;
            player.SetAutoShootInTutorial(new Vector3(1.3f, 1.3f, 2f));
            AutoLeft(true);
            AutoRight(true);
            yield return new WaitForSeconds(0.5f);
            AutoLeft(false);
            AutoRight(false);
            yield return new WaitForSeconds(1.5f);
            if (stopAutoShooting) break;
            player.SetAutoShootInTutorial(new Vector3(-0.7f, -0.6f, 2f));
            AutoLeft(true);
            AutoRight(true);
            yield return new WaitForSeconds(0.4f);
            AutoLeft(false);
            AutoRight(false);
            yield return new WaitForSeconds(1.6f);
            if (stopAutoShooting) break;
            player.SetAutoShootInTutorial(new Vector3(-0.4f, 1.2f, 2f));
            AutoLeft(true);
            AutoRight(true);
            yield return new WaitForSeconds(0.2f);
            AutoLeft(false);
            AutoRight(false);
            yield return new WaitForSeconds(1.8f);
        }
        stopAutoShooting = false;
    }

    IEnumerator AutoShoot2()
    {
        AutoLeft(false);
        AutoRight(false);
        yield return new WaitForSeconds(1f);
        player.SetAutoShootInTutorial(new Vector3(-0.4f, 1.2f, 2f));
        AutoLeft(true);
        AutoRight(true);
        yield return new WaitForSeconds(Random.Range(1f, 1.7f));
        AutoLeft(false);
        AutoRight(false);
        yield return new WaitForSeconds(1f);
        NextProcess();
        for (int i = 0; i < 20; i++)
        {
            if (stopAutoShooting) break;
            player.SetAutoShootInTutorial(new Vector3(-0.3f, 1.3f, 2f));
            AutoLeft(true);
            AutoRight(true);
            yield return new WaitForSeconds(0.2f);
            AutoLeft(false);
            AutoRight(false);
            yield return new WaitForSeconds(1.8f);
            if (stopAutoShooting) break;
            player.SetAutoShootInTutorial(new Vector3(-0.4f, 1.2f, 2f));
            AutoLeft(true);
            AutoRight(true);
            yield return new WaitForSeconds(Random.Range(1f, 1.7f));
            AutoLeft(false);
            AutoRight(false);
            yield return new WaitForSeconds(1f);
        }
        stopAutoShooting = false;
    }

    IEnumerator AutoShoot3()
    {
        AutoLeft(false);
        AutoRight(false);
        yield return new WaitForSeconds(1f);
        player.SetAutoShootInTutorial(new Vector3(-0.4f, 1f, 2f));
        AutoLeft(true);
        AutoRight(false);
        yield return new WaitForSeconds(1.5f);
        AutoLeft(false);
        yield return new WaitForSeconds(1f);
        player.SetAutoShootInTutorial(new Vector3(-0.4f, 1f, 2f));
        AutoRight(true);
        yield return new WaitForSeconds(1.5f);
        AutoRight(false);
        yield return new WaitForSeconds(1f);
        NextProcess();
        for (int i = 0; i < 15; i++)
        {
            if (stopAutoShooting) break;
            player.SetAutoShootInTutorial(new Vector3(0f, 0.8f, 2f));
            AutoRight(true);
            yield return new WaitForSeconds(0.8f);
            AutoLeft(true);
            yield return new WaitForSeconds(0.7f);
            AutoRight(false);
            if (stopAutoShooting)
            {
                AutoLeft(false);
                yield return new WaitForSeconds(1f);
                break;
            }
            yield return new WaitForSeconds(1.3f);
            AutoLeft(false);
            yield return new WaitForSeconds(1f);
            if (stopAutoShooting) break;
            player.SetAutoShootInTutorial(new Vector3(-0.4f, 1f, 2f));
            AutoLeft(true);
            yield return new WaitForSeconds(1.5f);
            AutoLeft(false);
            yield return new WaitForSeconds(1f);
            if (stopAutoShooting) break;
            player.SetAutoShootInTutorial(new Vector3(-0.4f, 1f, 2f));
            AutoRight(true);
            yield return new WaitForSeconds(1.5f);
            AutoRight(false);
            yield return new WaitForSeconds(1f);
        }
        stopAutoShooting = false;
    }

    IEnumerator AutoShoot4()
    {
        AutoLeft(false);
        AutoRight(false);
        yield return new WaitForSeconds(1f);
        player.SetAutoShootInTutorial(new Vector3(-1.2f, -0.5f, 3f));
        AutoRight(true);
        yield return new WaitForSeconds(1f);
        AutoLeft(true);
        yield return new WaitForSeconds(0.5f);
        AutoLeft(false);
        AutoRight(false);
        yield return new WaitForSeconds(1f);
        player.SetAutoShootInTutorial(new Vector3(-1.2f, -0.5f, 3f));
        AutoRight(true);
        yield return new WaitForSeconds(1.5f);
        AutoRight(false);
        yield return new WaitForSeconds(1f);
        NextProcess();
        for (int i = 0; i < 20; i++)
        {
            if (stopAutoShooting) break;
            player.SetAutoShootInTutorial(new Vector3(-1.2f, -0.5f, 3f));
            AutoRight(true);
            yield return new WaitForSeconds(0.5f);
            AutoLeft(true);
            yield return new WaitForSeconds(1f);
            AutoLeft(false);
            AutoRight(false);
            yield return new WaitForSeconds(1f);
            if (stopAutoShooting) break;
            player.SetAutoShootInTutorial(new Vector3(-1.2f, -0.5f, 3f));
            AutoRight(true);
            yield return new WaitForSeconds(1f);
            AutoLeft(true);
            yield return new WaitForSeconds(0.5f);
            AutoLeft(false);
            AutoRight(false);
            yield return new WaitForSeconds(1f);
            if (stopAutoShooting) break;
            player.SetAutoShootInTutorial(new Vector3(-1.2f, -0.5f, 3f));
            AutoRight(true);
            yield return new WaitForSeconds(1.5f);
            AutoRight(false);
            yield return new WaitForSeconds(1f);
        }
        stopAutoShooting = false;
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
        if (Phase >= 2)
        {
            myBubble.GetComponent<RectTransform>().anchoredPosition = new Vector2(220f, -140f);
        }
        myBubble.GetComponentInChildren<Text>().text = explanation;
        isProcessReady = true;
    }

    private void AutoLeft(bool isClicked)
    {
        player.AutoLeftInTutorial = isClicked;
        if (mouseButtons.Count > 0 && mouseButtons[0] != null)
        {
            if (isClicked)
            {
                mouseButtons[0].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, mouseButtons[0].GetComponent<Image>().color.a);
            }
            else
            {
                mouseButtons[0].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, mouseButtons[0].GetComponent<Image>().color.a);
            }
        }
    }

    private void AutoRight(bool isClicked)
    {
        player.AutoRightInTutorial = isClicked;
        if (mouseButtons.Count > 1 && mouseButtons[1] != null)
        {
            if (isClicked)
            {
                mouseButtons[1].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(pressedColor, mouseButtons[1].GetComponent<Image>().color.a);
            }
            else
            {
                mouseButtons[1].GetComponent<Image>().color = ColorUtil.instance.AlphaColor(Color.white, mouseButtons[1].GetComponent<Image>().color.a);
            }
        }
    }
}
