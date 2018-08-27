using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public GameObject target;           // 마우스 클릭 지점 프리팹입니다.
    public GameObject knife;            // 칼 프리팹입니다.
    public GameObject divineShield;
    public GameObject pageText;
    public List<GameObject> hearts;
    public AudioClip damagedSound;
    public AudioClip guardSound;
    public AudioClip killedSound;
    public AudioClip readySound;
    public GameObject blow;
    public GameObject zLocation;
    public GameObject weaponToSummon;
    public RectTransform purplePage;
    public Text purpleText;
    public GameObject speechBubble;
    public List<Tooltip> pausePanelButtons;
    public GameObject soundEffect;
    public Sprite[] soundEffectImage;
    

    private float speed;
    private float temporalSpeed;        // 시간 축을 따라 초당 움직이는 칸 수입니다.
    private float chargeSpeed;          // 마우스 클릭 시 Z좌표가 증가(감소)하는 속도입니다.
    private int health = 3;
    private GameObject targetObject;    // 현재 화면에 나타난 마우스 클릭 지점 오브젝트를 관리합니다.
    private GameObject myShield;
    private GameObject myText;
    private GameObject blowend;
    private Camera mainCamera;
    private bool hasReadySpoken = false;
    private bool isSFXPlaying = false;
    private float chargedZ;             // 투사체를 발사할 목적지 방향의 Z좌표(시간축 좌표)입니다.
    private float prepareWeaponTime;    // 투사체를 던지기 위해 마우스를 누르고 있던 시간 (충전 중이 아닐 때 -1, 충전이 시작되면 0부터 증가)
    private float invincibleTime;       // 피격 후 무적 판정이 되는, 남은 시간 
    private float temporalMoveCoolTime; // 시간 축을 따라 한 칸 이동하고 다음 한 칸을 이동하기까지 대기하는 시간입니다.
    private Rigidbody r;
    private Vector3 releasedMousePosition;
    private GameObject mySpeech;
    private Vector3 speechVector;
    private GameObject sound;
    private Vector3 soundVector;
    private Vector3 soundDirection;
    private float soundTime;
    private int soundNum;

    public int Health
    {
        get
        {
            return health;
        }
    }

    public bool IsInvincible
    {
        get
        {
            return invincibleTime > 0f;
        }
    }

    private void Awake()
    {
        r = GetComponent<Rigidbody>();
        chargedZ = 0f;
        prepareWeaponTime = -1f;
        myShield = null;
        blowend = null;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Start()
    {
        Manager.instance.PlayerObject = this.gameObject;
        string gameLevel = Manager.instance.GetCurrentGame()[1];
        if (gameLevel.Equals("Easy"))
        {
            chargeSpeed = Manager.instance.EasyChargeSpeed;
        }
        else if (gameLevel.Equals("Hard"))
        {
            chargeSpeed = Manager.instance.HardChargeSpeed;
        }
        speed = Manager.instance.MovingSpeed;
        temporalSpeed = Manager.instance.TemporalSpeed;
    }

    // Update is called once per frame
    void Update () {
        if (health <= 0 || Manager.instance.GetGameOver())
        {
            r.velocity = Vector3.zero;
            if (targetObject != null)
            {
                Destroy(targetObject);
                targetObject = null;
                purplePage.GetComponent<Image>().enabled = false;
                purpleText.enabled = false;
                weaponToSummon.GetComponent<MeshRenderer>().enabled = false;
            }
            if (myText != null)
            {
                Destroy(myText);
            }
            return;
        }

        // Esc키와 일시정지 관련 코드
        if (Input.GetKeyUp(KeyCode.Escape) && !Manager.instance.IsPaused)
        {
            Manager.instance.PauseButton();
        }
        else if (Input.GetKeyUp(KeyCode.Escape) && Manager.instance.IsPaused && Manager.instance.IsGameStart)
        {
            foreach (Tooltip t in pausePanelButtons)
            {
                t.DestroyTooltip();
            }
            Manager.instance.StartButton();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && Manager.instance.IsPaused && Manager.instance.IsGameStart &&
            pausePanelButtons.Count > 0)
        {
            pausePanelButtons[0].CreateTooltip();
        }

        if (Manager.instance.IsPaused)
        {
            if (targetObject != null)
            {
                Destroy(targetObject);
                targetObject = null;
                purplePage.GetComponent<Image>().enabled = false;
                purpleText.enabled = false;
                weaponToSummon.GetComponent<MeshRenderer>().enabled = false;
                chargedZ = 0f;
                prepareWeaponTime = -1f;
            }
            return;
        }

        #region 움직이는(move) 코드

        if (temporalMoveCoolTime > 0f)
        {
            temporalMoveCoolTime -= Time.deltaTime;
            /*
            // 연타할 때 페이지 간 이동 쿨타임이 초기화됩니다. 
            if ((((Input.GetKeyUp(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Q)) || 
                (Input.GetKeyUp(KeyCode.Q) && !Input.GetKey(KeyCode.LeftShift))) && !GetKeyPageUp()) ||
                (((Input.GetKeyUp(KeyCode.Space) && !Input.GetKey(KeyCode.E)) ||
                (Input.GetKeyUp(KeyCode.E) && !Input.GetKey(KeyCode.Space))) && !GetKeyPageDown()))
            {
                temporalMoveCoolTime = 0f;
            }
            */
        }

        // 키보드의 A, D, W, S, 좌Shift, Space 키로부터 입력을 받습니다.
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        if (!(SceneManager.GetActiveScene().name.Equals("Tutorial") && !GetComponent<TutorialManager>().CanMoveZ) &&
            !Manager.instance.GetGameOver())
        {
            if (GetKeyPageDown() && !GetKeyPageUp() && temporalMoveCoolTime <= 0f)
            {
                r.position = new Vector3(r.position.x, r.position.y, r.position.z - Boundary.OnePageToDeltaZ());
                temporalMoveCoolTime = 1f / temporalSpeed;
            }
            else if (!GetKeyPageDown() && GetKeyPageUp() && temporalMoveCoolTime <= 0f)
            {
                r.position = new Vector3(r.position.x, r.position.y, r.position.z + Boundary.OnePageToDeltaZ());
                temporalMoveCoolTime = 1f / temporalSpeed;
            }
            if (temporalMoveCoolTime > 0.5f / temporalSpeed)
            {
                moveHorizontal = 0f;
                moveVertical = 0f;
            }
        }

        if (SceneManager.GetActiveScene().name.Equals("Tutorial") && !GetComponent<TutorialManager>().CanMoveXY)
        {
            moveHorizontal = 0f;
            moveVertical = 0f;
        }

        zLocation.GetComponentInChildren<Text>().text = "" + Boundary.ZToPage(r.position.z);

        // 플레이어를 움직입니다.
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0f);
        r.velocity = movement.normalized * speed;

        // 플레이어가 화면 밖으로 나갈 수 없게 합니다.
        r.position = new Vector3
        (
            Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
            Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
            Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
        );

        #endregion

        #region 던지는(shoot) 코드

        if (!(SceneManager.GetActiveScene().name.Equals("Tutorial") && !GetComponent<TutorialManager>().CanShoot))
        {
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && prepareWeaponTime < 0f)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9) && hit.collider.gameObject.tag.Equals("Present"))
                {
                    prepareWeaponTime = 0f;
                }
            }
            else if (prepareWeaponTime >= 0f)
            {
                prepareWeaponTime += Time.deltaTime;
                if (targetObject != null)
                {
                    targetObject.GetComponentInChildren<ChargeClockUI>().PrepareTime = prepareWeaponTime;
                }
            }

            bool isMouseClickedAndInGameArea = false;
            if (!Input.GetMouseButton(1) && Input.GetMouseButton(0))
            {
                isMouseClickedAndInGameArea = Charging(-1f);
            }
            else if (!Input.GetMouseButton(0) && Input.GetMouseButton(1))
            {
                isMouseClickedAndInGameArea = Charging(1f);
            }
            else if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
            {
                isMouseClickedAndInGameArea = Charging(0f);
            }
            else if (prepareWeaponTime >= 0f && prepareWeaponTime < Manager.instance.PrepareChargeTime)
            {
                // 마우스를 누르고 있지 않지만 무기 소환이 시작되어 완료되지 않은 경우
                Charging(0f);
            }
            // 위의 조건들을 하나도 처리하지 않고 통과하는 경우는, 
            // 마우스를 누르고 있지 않고 (무기 소환을 시작하지 않은 경우 또는 무기 소환이 완료된 경우)뿐이다.
            // 위의 조건 중 하나를 처리했지만 isInGameArea가 false인 경우는,
            // 마우스가 게임 영역 밖(아래 UI)으로 나간 경우이다.

            if (!isMouseClickedAndInGameArea && prepareWeaponTime >= Manager.instance.PrepareChargeTime)
            {
                // 마우스를 누르고 있지 않고 무기 소환이 완료된 경우
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                GameObject k = Instantiate(knife, GetComponent<Transform>().position, Quaternion.identity);
                k.GetComponent<Knife>().Initialize(0, soundNum, new Vector3(
                    ray.origin.x + ray.direction.x *
                    (chargedZ + GetComponent<Transform>().position.z - ray.origin.z) / ray.direction.z,
                    ray.origin.y + ray.direction.y *
                    (chargedZ + GetComponent<Transform>().position.z - ray.origin.z) / ray.direction.z,
                    GetComponent<Transform>().position.z + Boundary.RoundZ(chargedZ)));
                Destroy(targetObject);
                targetObject = null;
                purplePage.GetComponent<Image>().enabled = false;
                purpleText.enabled = false;
                weaponToSummon.GetComponent<MeshRenderer>().enabled = false;
                chargedZ = 0f;
                prepareWeaponTime = -1f;
                soundNum = (soundNum + 1) % 3;
            }

            /*
            if (!hasMouseReleased && ((Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1)) || (Input.GetMouseButtonUp(1) && !Input.GetMouseButton(0))))
            {
                Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9) && hit.collider.gameObject.tag.Equals("Present"))
                {
                    hasMouseReleased = true;
                }
            }
            */
        }
        #endregion
    }

    void FixedUpdate()
    {
        if (invincibleTime > 0f)
        {
            invincibleTime -= Time.fixedDeltaTime;
        }
        if (invincibleTime < 0f)
        {
            invincibleTime = 0f;
        }
        if (invincibleTime <= 0f && myShield != null)
        {
            Destroy(myShield);
            myShield = null;
        }

        #region 말풍선 위치&방향 조절하는 코드
        speechVector = mainCamera.WorldToScreenPoint(GetComponent<Transform>().position);
        if(mySpeech != null)
        {
            if (speechVector.x < 250f)
            {
                speechVector.x += 120f;
                if (speechVector.y > 614f)
                {
                    speechVector.y -= 80f;
                    mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(180f, 180f, 0f));
                }
                else if (speechVector.y < 270f)
                {
                    speechVector.y += 80f;
                    mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                }
                else
                {
                    if (mySpeech.GetComponent<RectTransform>().rotation.Equals(Quaternion.Euler(new Vector3(0f, 180f, 0f))) ||
                        mySpeech.GetComponent<RectTransform>().rotation.Equals(Quaternion.Euler(new Vector3(0f, 0f, 0f))))
                    {
                        speechVector.y += 80f;
                        mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                    }
                    else
                    {
                        speechVector.y -= 80f;
                        mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(180f, 180f, 0f));
                    }
                }
            }
            else if (speechVector.x > 774f)
            {
                speechVector.x -= 120f;
                if (speechVector.y > 614f)
                {
                    speechVector.y -= 80f;
                    mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(180f, 0f, 0f));
                }
                if (speechVector.y < 270f)
                {
                    speechVector.y += 80f;
                    mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                }
                else
                {
                    if (mySpeech.GetComponent<RectTransform>().rotation.Equals(Quaternion.Euler(new Vector3(0f, 180f, 0f))) ||
                        mySpeech.GetComponent<RectTransform>().rotation.Equals(Quaternion.Euler(new Vector3(0f, 0f, 0f))))
                    {
                        speechVector.y += 80f;
                        mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                    }
                    else
                    {
                        speechVector.y -= 80f;
                        mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(180f, 0f, 0f));
                    }
                }
            }
            else
            {
                if (speechVector.y > 614f)
                {
                    speechVector.y -= 80f;
                    if (mySpeech.GetComponent<RectTransform>().rotation.Equals(Quaternion.Euler(new Vector3(0f, 0f, 0f))) ||
                        mySpeech.GetComponent<RectTransform>().rotation.Equals(Quaternion.Euler(new Vector3(180f, 0f, 0f))))
                    {
                        speechVector.x -= 120f;
                        mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(180f, 0f, 0f));
                    }
                    else
                    {
                        speechVector.x += 120f;
                        mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(180f, 180f, 0f));
                    }
                }
                else if (speechVector.y < 270f)
                {
                    speechVector.y += 80f;
                    if (mySpeech.GetComponent<RectTransform>().rotation.Equals(Quaternion.Euler(new Vector3(0f, 0f, 0f))) ||
                        mySpeech.GetComponent<RectTransform>().rotation.Equals(Quaternion.Euler(new Vector3(180f, 0f, 0f))))
                    {
                        speechVector.x -= 120f;
                        mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                    }
                    else
                    {
                        speechVector.x += 120f;
                        mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                    }
                }
                else
                {
                    if (mySpeech.GetComponent<RectTransform>().rotation.Equals(Quaternion.Euler(new Vector3(0f, 0f, 0f))))
                    {
                        speechVector.x -= 120f;
                        speechVector.y += 80f;
                    }
                    else if (mySpeech.GetComponent<RectTransform>().rotation.Equals(Quaternion.Euler(new Vector3(180f, 0f, 0f))))
                    {
                        speechVector.x -= 120f;
                        speechVector.y -= 80f;
                    }
                    else if (mySpeech.GetComponent<RectTransform>().rotation.Equals(Quaternion.Euler(new Vector3(0f, 180f, 0f))))
                    {
                        speechVector.x += 120f;
                        speechVector.y += 80f;
                    }
                    else
                    {
                        speechVector.x += 120f;
                        speechVector.y -= 80f;
                    }
                }
            }
            Transform child = mySpeech.transform.Find("SpeechText");
            child.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            mySpeech.GetComponent<RectTransform>().position = speechVector;
        }
        else
        {
            if (speechVector.x < 774f)
            {
                speechVector.x += 120f;
                if (speechVector.y < 614f)
                {
                    speechVector.y += 80f;
                }
                else
                {
                    speechVector.y -= 80f;
                }
            }
            else
            {
                speechVector.x -= 120f;
                if (speechVector.y < 614f)
                {
                    speechVector.y += 80f;
                }
                else
                {
                    speechVector.y -= 80f;
                }
            }
        }
        #endregion

        if (Manager.instance.GetGameOver()) return;

        if (Manager.instance.IsPaused)
        {
            if (myText != null) myText.GetComponent<Text>().enabled = false;
        }
        else
        {
            if (myText != null) myText.GetComponent<Text>().enabled = true;
            TextMover();

            if (prepareWeaponTime > 0f)
            {
                weaponToSummon.GetComponent<Transform>().localScale = new Vector3(
                    Mathf.Lerp(0f, 0.8f, prepareWeaponTime / Manager.instance.PrepareChargeTime),
                    Mathf.Lerp(0f, 0.8f, prepareWeaponTime / Manager.instance.PrepareChargeTime),
                    weaponToSummon.GetComponent<Transform>().localScale.z
                    );
                weaponToSummon.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    /// <summary>
    /// 플레이어가 있는 페이지를 텍스트로 표시하는 함수입니다.
    /// </summary>
    private void TextMover()
    {
        if (Manager.instance.GetCurrentGame()[1].Equals("Hard"))
        {
            return;
        }
        Vector3 v = mainCamera.WorldToScreenPoint(GetComponent<Transform>().position);
        v.y += 70f;
        if (myText != null)
        {
            myText.GetComponent<Transform>().position = v;
        }
        else
        {
            myText = Instantiate(pageText, v, Quaternion.identity, Manager.instance.Canvas.GetComponent<Transform>());
        }
        myText.GetComponent<Text>().text = Boundary.ZToPage(GetComponent<Transform>().position.z).ToString();

        myText.GetComponent<Text>().color = GetComponentInChildren<CharacterModel>().GetComponent<SpriteRenderer>().color;
    }

    public void Damaged()
    {
        if (Manager.instance.GetGameOver()) return;

        if (Health > 0 && invincibleTime <= 0f)
        {
            Debug.LogWarning("Player hit!");
            health--;
            MakeSoundEffect();
            StartCoroutine("SoundEffectMover");
            if (hearts.Count > Health)
            {
                hearts[Health].SetActive(false);
            }
            if (Health > 0)
            {
                invincibleTime = 3f;
                myShield = Instantiate(divineShield, GetComponent<Transform>());
                myShield.GetComponent<MeshRenderer>().enabled = true;
                StartCoroutine("DamagedSFX");
            }
        }
        else if (Health > 0 && invincibleTime > 0f)
        {
            if (!isSFXPlaying)
            {
                GetComponent<AudioSource>().clip = guardSound;
                GetComponent<AudioSource>().Play();
            }
        }

        if (Health <= 0 && GetComponentInChildren<CharacterModel>().gameObject.activeInHierarchy)
        {
            invincibleTime = 0f;
            GetComponentInChildren<CharacterModel>().gameObject.SetActive(false);
            weaponToSummon.SetActive(false);
            r.velocity = Vector3.zero;
            GetComponent<AudioSource>().clip = killedSound;
            GetComponent<AudioSource>().Play();
            blowend = Instantiate(blow, GetComponent<Transform>().position, Quaternion.identity);
            
            StartCoroutine("Blow");
            StartCoroutine("EndSpeech");
            Manager.instance.LoseGame();
        }
    }
    
    IEnumerator Blow()
    {
        yield return new WaitForSeconds(1.0f);
        if (blowend != null)
        {
            Destroy(blowend);
        }
        blowend = null;
    }

    IEnumerator EndSpeech()
    {
        if (mySpeech != null)
        {
            Destroy(mySpeech);
        }
        mySpeech = Instantiate(speechBubble, speechVector, Quaternion.identity, Manager.instance.Canvas.GetComponent<Transform>());
        mySpeech.GetComponentInChildren<Text>().text = "이제 그만...";
        if (mainCamera.WorldToScreenPoint(GetComponent<Transform>().position).x < 774f)
        {
            if (mainCamera.WorldToScreenPoint(GetComponent<Transform>().position).y < 614f)
            {
                mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
            }
            else
            {
                mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(180f, 180f, 0f));
            }
        }
        else
        {
            if (mainCamera.WorldToScreenPoint(GetComponent<Transform>().position).y < 614f)
            {
                mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }
            else
            {
                mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(180f, 0f, 0f));
            }
        }
        Transform child = mySpeech.transform.Find("SpeechText");
        child.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        mySpeech.GetComponent<RectTransform>().position = speechVector;
        yield return new WaitForSeconds(2.2f);
        Destroy(mySpeech);
        mySpeech = null;
    }

    public void SpeakReady()
    {
        if (!hasReadySpoken)
        {
            hasReadySpoken = true;

            string gameMode = Manager.instance.GetCurrentGame()[0];
            if (gameMode.Equals("Vagabond") || gameMode.Equals("Guardian") || gameMode.Equals("Stalker"))
            {
                StartCoroutine("ReadySpeech");
            }
        }
    }

    IEnumerator ReadySpeech()
    {
        if (mySpeech != null)
        {
            Destroy(mySpeech);
        }
        GetComponent<AudioSource>().clip = readySound;
        GetComponent<AudioSource>().Play();
        mySpeech = Instantiate(speechBubble, speechVector, Quaternion.identity, Manager.instance.Canvas.GetComponent<Transform>());
        mySpeech.GetComponentInChildren<Text>().text = "난 준비되었네.";
        if (mainCamera.WorldToScreenPoint(GetComponent<Transform>().position).x < 774f)
        {
            if (mainCamera.WorldToScreenPoint(GetComponent<Transform>().position).y < 614f)
            {
                mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
            }
            else
            {
                mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(180f, 180f, 0f));
            }
        }
        else
        {
            if (mainCamera.WorldToScreenPoint(GetComponent<Transform>().position).y < 614f)
            {
                mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }
            else
            {
                mySpeech.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(180f, 0f, 0f));
            }
        }
        Transform child = mySpeech.transform.Find("SpeechText");
        child.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        mySpeech.GetComponent<RectTransform>().position = speechVector;
        yield return new WaitForSeconds(1.5f);
        Destroy(mySpeech);
        mySpeech = null;
    }

    IEnumerator DamagedSFX()
    {
        GetComponent<AudioSource>().clip = damagedSound;
        GetComponent<AudioSource>().Play();
        isSFXPlaying = true;
        yield return new WaitForSeconds(0.4f);
        isSFXPlaying = false;
    }

    IEnumerator SoundEffectMover()
    {
        for(int i = 0; i <= 30; i++)
        {
            soundVector += soundDirection * 0.01f;
            sound.GetComponent<Transform>().position = soundVector;
            sound.GetComponent<SpriteRenderer>().color = ColorUtil.instance.AlphaColor(new Color(1f, 1f, 1f), 1f - Mathf.Pow((float)i / 30f, 2f));
            yield return null;
        }
        Destroy(sound);
        sound = null;
    }

    private void MakeSoundEffect()
    {
        soundTime = 0.5f;
        soundVector = GetComponent<Transform>().position;
        soundDirection = new Vector3(Random.value * 2f -1f, Random.value * 2f -1f, 0f).normalized;
        soundVector += soundDirection * 0.3f;
        sound = Instantiate(soundEffect, soundVector, Quaternion.identity);
        sound.GetComponent<SpriteRenderer>().sprite = soundEffectImage[(int)Mathf.Floor(Random.value * 3.99f)];
    }

    private bool GetKeyPageDown()
    {
        return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Q));
    }

    private bool GetKeyPageUp()
    {
        return (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E));
    }

    /// <summary>
    /// 무기를 충전하여 던질 곳을 정하고 시계 UI로 보여주는 함수입니다.
    /// chargeDirection이 -1f이면 앞 페이지로, 1f이면 뒤 페이지로 조준합니다. 0f이면 조준하는 페이지가 멈춥니다.
    /// 조준점은 마우스의 위치를 따릅니다.
    /// 반환값이 false이면 마우스가 영역을 벗어나서 charging이 되지 않는 상태라는 것을 의미합니다.
    /// </summary>
    /// <param name="chargeDirection"></param>
    private bool Charging(float chargeDirection)
    {
        return Charging(chargeDirection, Input.mousePosition);
    }

    /// <summary>
    /// 무기를 충전하여 던질 곳을 정하고 시계 UI로 보여주는 함수입니다.
    /// chargeDirection이 -1f이면 앞 페이지로, 1f이면 뒤 페이지로 조준합니다. 0f이면 조준하는 페이지가 멈춥니다.
    /// mousePosition으로 조준점을 직접 지정할 수 있습니다.
    /// 반환값이 false이면 마우스가 영역을 벗어나서 charging이 되지 않는 상태라는 것을 의미합니다.
    /// </summary>
    /// <param name="chargeDirection"></param>
    /// <param name="mousePosition"></param>
    private bool Charging(float chargeDirection, Vector3 mousePosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9) && hit.collider.gameObject.tag.Equals("Present"))
        {
            //Debug.DrawLine(ray.origin, hit.point, Color.yellow, 3f);
            if (targetObject == null)
            {
                targetObject = Instantiate(target, hit.point, Quaternion.identity);
                targetObject.GetComponentInChildren<ChargeClockUI>().SetRectTransform();
                targetObject.GetComponentInChildren<ChargeClockUI>().SetVisible();
                purplePage.GetComponent<Image>().enabled = true;
                purpleText.enabled = true;
            }
            else
            {
                targetObject.GetComponent<Transform>().SetPositionAndRotation(hit.point, Quaternion.identity);
            }

            if (targetObject != null)
            {
                int v = Boundary.IsValid(Boundary.RoundZ(chargedZ) + GetComponent<Transform>().position.z);
                if (v == 0)
                    chargedZ += Time.deltaTime * chargeSpeed * chargeDirection;
                else if (v < 0)
                    chargedZ = Boundary.zMin - GetComponent<Transform>().position.z;
                else if (v > 0)
                    chargedZ = Boundary.zMax - GetComponent<Transform>().position.z;

                targetObject.GetComponentInChildren<ChargeClockUI>().ChargedZ = Boundary.RoundZ(chargedZ);

                purplePage.anchoredPosition = new Vector2(Mathf.Lerp(-240f, 240f,
                    ((Boundary.ZToPage(GetComponent<Transform>().position.z + Boundary.RoundZ(chargedZ)) - Boundary.pageBase)
                    / (float)Boundary.pageNum)), purplePage.anchoredPosition.y);
                purpleText.text = Boundary.ZToPage(GetComponent<Transform>().position.z + Boundary.RoundZ(chargedZ)).ToString();
            }
            return true;
        }
        return false;
    }
    
}
