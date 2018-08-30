﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public GameObject knife;
    public GameObject divineShield;
    public GameObject pageText;
    public List<GameObject> hearts;
    public AudioClip damagedSound;
    public AudioClip guardSound;
    public AudioClip killedSound;
    public AudioClip readySound;
    public AudioClip tutorialSound1;
    public AudioClip tutorialSound2;
    public GameObject blow;
    public GameObject weaponToSummon;
    public delegate void Damaged();
    public Damaged damaged;
    public GameObject speechBubble;

    private float speed;
    private float temporalSpeed;         // 시간 축을 따라 초당 움직이는 칸 수입니다.
    private float chargeSpeed = 0f;
    private int health = 3;
    private GameObject myShield;
    private GameObject myText;
    private GameObject blowend;
    private Vector3 exactTarget;
    private Vector3 startPosition;
    private Vector3 destPosition;
    private bool isArrived = true;
    private bool isCharging = false;
    private bool hasReadySpoken = false;
    private bool isSFXPlaying = false;
    private bool isModelVisible = true;
    private float chargedZ;
    private float approxZ;                  // 플레이어 캐릭터 근처의, 투사체를 발사할 지점의 Z좌표
    private float prepareWeaponTime;        // 투사체를 던지기 위해 마우스를 누르고 있던 시간 (충전 중이 아닐 때 -1, 충전이 시작되면 0부터 증가)
    private float invincibleTime;           // 피격 후 무적 판정이 되는, 남은 시간 
    private float maxInvincibleTime = 3f;
    private float temporalMoveCoolTime;     // 시간 축을 따라 한 칸 이동하고 다음 한 칸을 이동하기까지 대기하는 시간입니다.
    private float easyMoveCoolTime = 0f;    // 쉬움 난이도에서 목적지에 도착하고 다시 움직이기까지 대기하는 시간입니다.
    private Rigidbody r;
    private Transform t;
    private Player player;
    private Camera mainCamera;
    private delegate void WhileInvincible();
    private delegate void Vanish();
    private delegate void Move();
    private delegate void Shoot();
    private WhileInvincible whileInvincible;
    private Vanish vanish;
    private Move move;
    private Shoot shoot;
    private GameObject mySpeech;
    private Vector3 speechVector;

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
    
    void Awake () {
        r = GetComponent<Rigidbody>(); 
        t = GetComponent<Transform>();
        chargedZ = 0f;
        myShield = null;
        blowend = null;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Start()
    {
        Manager.instance.EnemyObject = this.gameObject;
        string gameMode = Manager.instance.GetCurrentGame()[0];
        string gameLevel = Manager.instance.GetCurrentGame()[1];
        if (gameMode.Equals("Tutorial"))
        {
            whileInvincible += WIMove;
            vanish += VanishNormal;
            move += MoveNever;
            shoot += ShootNever;
            damaged += DamagedTutorial;
        }
        else if (gameMode.Equals("Vagabond"))
        {
            whileInvincible += WINormal;
            vanish += VanishNormal;
            move += MoveVagabondHard;
            damaged += DamagedVS;
        }
        else if (gameMode.Equals("Stalker"))
        {
            whileInvincible += WINormal;
            vanish += VanishNormal;
            move += MoveStalkerHard;
            damaged += DamagedVS;
        }
        else if (gameMode.Equals("Guardian"))
        {
            whileInvincible += WIMove;
            vanish += VanishNormal;
            move += MoveNever;
            damaged += DamagedGuardian;
            MoveGuardian();
        }

        if (gameLevel.Equals("Hard") && gameMode.Equals("Guardian"))
        {
            shoot += ShootInsane;
            chargeSpeed = Manager.instance.HardChargeSpeed;
        }
        else if (gameLevel.Equals("Hard"))
        {
            shoot += ShootHard;
            chargeSpeed = Manager.instance.HardChargeSpeed;
        }
        else if (gameLevel.Equals("Easy") && !gameMode.Equals("Tutorial"))
        {
            shoot += ShootEasy;
            chargeSpeed = Manager.instance.EasyChargeSpeed;
            if (gameMode.Equals("Vagabond"))
            {
                move -= MoveVagabondHard;
                move += MoveVagabondEasy;
            }
            else if (gameMode.Equals("Stalker"))
            {
                move -= MoveStalkerHard;
                move += MoveStalkerEasy;
            }
        }
        speed = Manager.instance.MovingSpeed;
        temporalSpeed = Manager.instance.TemporalSpeed;
    }

    void FixedUpdate ()
    {
        #region 말풍선 위치&방향 조절하는 코드
        speechVector = mainCamera.WorldToScreenPoint(GetComponent<Transform>().position);
        if (mySpeech != null)
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

        whileInvincible();

        if (health <= 0 || Manager.instance.GetGameOver())
        {
            r.velocity = Vector3.zero;
            if (myText != null)
            {
                Destroy(myText);
            }
            return;
        }

        // 플레이어 캐릭터와의 Z좌표(시간축 좌표) 차이에 따라 투명도를 적용합니다.
        if (vanish.GetInvocationList().Length > 0) {
            vanish();
        }

        move();

        shoot();
    }

    #region 무적인 동안(WhileInvincible) 실행될 함수들

    /// <summary>
    /// 무적인 동안 매 프레임마다 실행될 함수입니다.
    /// 방랑자, 추적자 인공지능에 사용됩니다.
    /// </summary>
    private void WINormal()
    {
        if (Manager.instance.IsPaused) return;
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
    }

    /// <summary>
    /// 무적인 동안 매 프레임마다 실행될 함수입니다.
    /// 수호자 인공지능과 튜토리얼에 사용됩니다.
    /// </summary>
    private void WIMove()
    {
        if (Manager.instance.IsPaused) return;
        if (invincibleTime > 0f)
        {
            invincibleTime -= Time.fixedDeltaTime;
            Vector3 pos = Vector3.Lerp(destPosition, startPosition, invincibleTime / maxInvincibleTime);
            pos.z = Boundary.RoundZ(pos.z);
            t.SetPositionAndRotation(pos, Quaternion.identity);
        }
        if (invincibleTime < 0f)
        {
            invincibleTime = 0f;
        }
        if (invincibleTime <= 0f && myShield != null)
        {
            t.SetPositionAndRotation(destPosition, Quaternion.identity);
            Destroy(myShield);
            myShield = null;
        }
    }

    #endregion

    #region 투명해지는(Vanish) 함수들

    /// <summary>
    /// 시간대가 다를 때 투명해지도록 하는 함수입니다.
    /// </summary>
    private void VanishNormal()
    {
        float alpha = Mathf.Max(0f, 1f - Mathf.Pow(Mathf.Abs(player.GetComponent<Transform>().position.z - t.position.z) / Boundary.sight, 2));

        if (Manager.instance.IsPaused && myText != null)
        {
            myText.GetComponent<Text>().enabled = false;
        }
        else if (!Manager.instance.IsPaused)
        {
            if (1 - (Mathf.Abs(player.GetComponent<Transform>().position.z - t.position.z) / Boundary.sight) < 0 || 
                !isModelVisible)
            {
                foreach (SpriteRenderer mr in GetComponentsInChildren<SpriteRenderer>())
                {
                    mr.enabled = false;
                }
                weaponToSummon.GetComponent<MeshRenderer>().enabled = false;

                if (myShield != null) myShield.GetComponent<MeshRenderer>().enabled = false;
                if (myText != null) myText.GetComponent<Text>().enabled = false;
            }
            else if (Mathf.Abs(player.GetComponent<Transform>().position.z - t.position.z) < Boundary.OnePageToDeltaZ() * Boundary.approach)
            {
                foreach (SpriteRenderer mr in GetComponentsInChildren<SpriteRenderer>())
                {
                    mr.enabled = true;
                }
                //Material m = GetComponentInChildren<CharacterModel>().GetComponent<MeshRenderer>().material;
                SpriteRenderer m = GetComponentInChildren<CharacterModel>().GetComponent<SpriteRenderer>();
                m.color = ColorUtil.instance.AlphaColor(ColorUtil.instance.presentEnemyColor, alpha);
                
                weaponToSummon.GetComponent<MeshRenderer>().material.color =
                    ColorUtil.instance.AlphaColor(weaponToSummon.GetComponent<MeshRenderer>().material.color, alpha);

                if (myShield != null)
                {
                    myShield.GetComponent<MeshRenderer>().material.color = ColorUtil.instance.AlphaColor(
                        myShield.GetComponent<MeshRenderer>().material.color, alpha);
                    myShield.GetComponent<MeshRenderer>().enabled = true;
                }

                if (myText != null && !Manager.instance.IsPaused) myText.GetComponent<Text>().enabled = true;
                TextMover();
            }
            else if (t.position.z < player.GetComponent<Transform>().position.z)
            {
                foreach (SpriteRenderer mr in GetComponentsInChildren<SpriteRenderer>())
                {
                    mr.enabled = true;
                }
                //Material m = GetComponentInChildren<CharacterModel>().GetComponent<MeshRenderer>().material;
                SpriteRenderer m = GetComponentInChildren<CharacterModel>().GetComponent<SpriteRenderer>();
                m.color =
                    ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.pastColor, ColorUtil.instance.pastPastColor,
                    Mathf.Abs(player.GetComponent<Transform>().position.z - t.position.z) - Boundary.OnePageToDeltaZ() * Boundary.approach), alpha);

                weaponToSummon.GetComponent<MeshRenderer>().material.color =
                        ColorUtil.instance.AlphaColor(weaponToSummon.GetComponent<MeshRenderer>().material.color, alpha);

                if (myShield != null)
                {
                    myShield.GetComponent<MeshRenderer>().material.color = ColorUtil.instance.AlphaColor(
                        myShield.GetComponent<MeshRenderer>().material.color, alpha);
                    myShield.GetComponent<MeshRenderer>().enabled = true;
                }

                if (myText != null && !Manager.instance.IsPaused) myText.GetComponent<Text>().enabled = true;
                TextMover();
            }
            else
            {
                foreach (SpriteRenderer mr in GetComponentsInChildren<SpriteRenderer>())
                {
                    mr.enabled = true;
                }
                //Material m = GetComponentInChildren<CharacterModel>().GetComponent<MeshRenderer>().material;
                SpriteRenderer m = GetComponentInChildren<CharacterModel>().GetComponent<SpriteRenderer>();
                m.color =
                    ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.futureColor, ColorUtil.instance.futureFutureColor,
                    Mathf.Abs(player.GetComponent<Transform>().position.z - t.position.z) - Boundary.OnePageToDeltaZ() * Boundary.approach), alpha);
                
                weaponToSummon.GetComponent<MeshRenderer>().material.color =
                    ColorUtil.instance.AlphaColor(weaponToSummon.GetComponent<MeshRenderer>().material.color, alpha);

                if (myShield != null)
                {
                    myShield.GetComponent<MeshRenderer>().material.color = ColorUtil.instance.AlphaColor(
                        myShield.GetComponent<MeshRenderer>().material.color, alpha);
                    myShield.GetComponent<MeshRenderer>().enabled = true;
                }

                if (myText != null && !Manager.instance.IsPaused) myText.GetComponent<Text>().enabled = true;
                TextMover();
            }


            if (!(1 - (Mathf.Abs(player.GetComponent<Transform>().position.z - t.position.z) / Boundary.sight) < 0) &&
                prepareWeaponTime > 0f)
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
    /// 상대가 있는 페이지를 텍스트로 표시하는 함수입니다.
    /// delegate로 호출되지 않습니다.
    /// </summary>
    private void TextMover()
    {
        Vector3 v = mainCamera.WorldToScreenPoint(t.position);
        v.y += 70f;
        if (myText != null)
        {
            myText.GetComponent<Transform>().position = v;
        }
        else
        {
            myText = Instantiate(pageText, v, Quaternion.identity, Manager.instance.Canvas.GetComponent<Transform>());
        }
        myText.GetComponent<Text>().text = Boundary.ZToPage(t.position.z).ToString();

        myText.GetComponent<Text>().color = GetComponentInChildren<CharacterModel>().GetComponent<SpriteRenderer>().color;
    }

    #endregion

    #region 이동(Move) 함수들

    /// <summary>
    /// 이동 함수입니다.
    /// 방랑자(어려움) 인공지능에 사용됩니다.
    /// </summary>
    private void MoveVagabondHard()
    {
        if (!isArrived &&
            Vector3.Distance(t.position, destPosition) < Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지에 도착했습니다.
            isArrived = true;
        }
        else if (!isArrived && temporalMoveCoolTime > 0f)
        {
            temporalMoveCoolTime -= Time.fixedDeltaTime;
        }
        else if (!isArrived &&
            Mathf.Abs(t.position.z - destPosition.z) > Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지를 향해 시간 축을 따라 이동합니다.
            r.velocity = Vector3.zero;
            float deltaZ;
            if (t.position.z > destPosition.z) deltaZ = -Boundary.OnePageToDeltaZ();
            else deltaZ = Boundary.OnePageToDeltaZ();
            temporalMoveCoolTime = 1f / temporalSpeed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z + deltaZ, Boundary.zMin, Boundary.zMax)
            );
        }
        else if (!isArrived)
        {
            // 목적지를 향해 XY평면을 따라 이동합니다.
            Vector3 movement = destPosition - t.position;
            movement.z = 0f;
            r.velocity = movement.normalized * speed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
            );
        }

        if (isArrived)
        {
            // 새로 가려는 목적지를 정합니다.
            float z = t.position.z;
            if (Mathf.Abs(Boundary.zMax) - Mathf.Abs(z) < 0.5f)
                z = Random.Range(Boundary.zMin, Boundary.zMax);
            else
                z += GaussianRandom() * 1.2f;
            z = Mathf.Clamp(z, Boundary.zMin, Boundary.zMax);
            destPosition = new Vector3
            (
                Random.Range(Boundary.xMin, Boundary.xMax),
                Random.Range(Boundary.yMin, Boundary.yMax),
                Boundary.RoundZ(z)
            );
            isArrived = false;
        }
    }

    /// <summary>
    /// 이동 함수입니다.
    /// 방랑자(쉬움) 인공지능에 사용됩니다.
    /// </summary>
    private void MoveVagabondEasy()
    {
        if (easyMoveCoolTime > 0f)
        {
            r.velocity = Vector3.zero;
            easyMoveCoolTime -= Time.fixedDeltaTime;
        }

        if (!isArrived &&
            Vector3.Distance(t.position, destPosition) < Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지에 도착했습니다.
            easyMoveCoolTime = Random.Range(0f, 2f);
            isArrived = true;
        }
        else if (!isArrived && temporalMoveCoolTime > 0f)
        {
            temporalMoveCoolTime -= Time.fixedDeltaTime;
        }
        else if (!isArrived &&
            Mathf.Abs(t.position.z - destPosition.z) > Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지를 향해 시간 축을 따라 이동합니다.
            r.velocity = Vector3.zero;
            float deltaZ;
            if (t.position.z > destPosition.z) deltaZ = -Boundary.OnePageToDeltaZ();
            else deltaZ = Boundary.OnePageToDeltaZ();
            temporalMoveCoolTime = 1f / temporalSpeed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z + deltaZ, Boundary.zMin, Boundary.zMax)
            );
        }
        else if (!isArrived)
        {
            // 목적지를 향해 XY평면을 따라 이동합니다.
            Vector3 movement = destPosition - t.position;
            movement.z = 0f;
            r.velocity = movement.normalized * speed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
            );
        }

        if (isArrived && easyMoveCoolTime <= 0f)
        {
            // 새로 가려는 목적지를 정합니다.
            float z = t.position.z;
            if (Mathf.Abs(Boundary.zMax) - Mathf.Abs(z) < 0.5f)
                z = Random.Range(Boundary.zMin, Boundary.zMax);
            else
                z += GaussianRandom() * 1.2f;
            z = Mathf.Clamp(z, Boundary.zMin, Boundary.zMax);
            destPosition = new Vector3
            (
                Random.Range(Boundary.xMin, Boundary.xMax),
                Random.Range(Boundary.yMin, Boundary.yMax),
                Boundary.RoundZ(z)
            );
            isArrived = false;
        }
    }

    /// <summary>
    /// 이동 함수입니다.
    /// 추적자(어려움) 인공지능에 사용됩니다.
    /// </summary>
    private void MoveStalkerHard()
    {
        if (!isArrived &&
            Vector3.Distance(t.position, destPosition) < Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지에 도착했습니다.
            isArrived = true;
        }
        else if (!isArrived && temporalMoveCoolTime > 0f)
        {
            temporalMoveCoolTime -= Time.fixedDeltaTime;
        }
        else if (!isArrived &&
            Mathf.Abs(t.position.z - destPosition.z) > Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지를 향해 시간 축을 따라 이동합니다.
            r.velocity = Vector3.zero;
            float deltaZ;
            if (t.position.z > destPosition.z) deltaZ = -Boundary.OnePageToDeltaZ();
            else deltaZ = Boundary.OnePageToDeltaZ();
            temporalMoveCoolTime = 1f / temporalSpeed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z + deltaZ, Boundary.zMin, Boundary.zMax)
            );
        }
        else if (!isArrived)
        {
            // 목적지를 향해 XY평면을 따라 이동합니다.
            Vector3 movement = destPosition - t.position;
            movement.z = 0f;
            r.velocity = movement.normalized * speed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
            );
        }

        if (isArrived)
        {
            // 새로 가려는 목적지를 정합니다.
            Vector3 playerPosition = player.GetComponent<Transform>().position;
            float x = playerPosition.x + 0.8f * GaussianRandom();
            float y = playerPosition.y + 0.6f * GaussianRandom();
            float z = playerPosition.z + 0.5f * GaussianRandom();
            x = Mathf.Clamp(x, Boundary.xMin, Boundary.xMax);
            y = Mathf.Clamp(y, Boundary.yMin, Boundary.yMax);
            z = Mathf.Clamp(Boundary.RoundZ(z), Boundary.zMin, Boundary.zMax);
            destPosition = new Vector3
            (
                x,
                y,
                z
            );
            isArrived = false;
        }
    }

    /// <summary>
    /// 이동 함수입니다.
    /// 추적자(쉬움) 인공지능에 사용됩니다.
    /// </summary>
    private void MoveStalkerEasy()
    {
        if (easyMoveCoolTime > 0f)
        {
            r.velocity = Vector3.zero;
            easyMoveCoolTime -= Time.fixedDeltaTime;
        }

        if (!isArrived &&
            Vector3.Distance(t.position, destPosition) < Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지에 도착했습니다.
            easyMoveCoolTime = Random.Range(0f, 2f);
            isArrived = true;
        }
        else if (!isArrived && temporalMoveCoolTime > 0f)
        {
            temporalMoveCoolTime -= Time.fixedDeltaTime;
        }
        else if (!isArrived &&
            Mathf.Abs(t.position.z - destPosition.z) > Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지를 향해 시간 축을 따라 이동합니다.
            r.velocity = Vector3.zero;
            float deltaZ;
            if (t.position.z > destPosition.z) deltaZ = -Boundary.OnePageToDeltaZ();
            else deltaZ = Boundary.OnePageToDeltaZ();
            temporalMoveCoolTime = 1f / temporalSpeed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z + deltaZ, Boundary.zMin, Boundary.zMax)
            );
        }
        else if (!isArrived)
        {
            // 목적지를 향해 XY평면을 따라 이동합니다.
            Vector3 movement = destPosition - t.position;
            movement.z = 0f;
            r.velocity = movement.normalized * speed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
            );
        }

        if (isArrived && easyMoveCoolTime <= 0f)
        {
            // 새로 가려는 목적지를 정합니다.
            Vector3 playerPosition = player.GetComponent<Transform>().position;
            float x = playerPosition.x + 0.8f * GaussianRandom();
            float y = playerPosition.y + 0.6f * GaussianRandom();
            float z = playerPosition.z + 0.5f * GaussianRandom();
            x = Mathf.Clamp(x, Boundary.xMin, Boundary.xMax);
            y = Mathf.Clamp(y, Boundary.yMin, Boundary.yMax);
            z = Mathf.Clamp(Boundary.RoundZ(z), Boundary.zMin, Boundary.zMax);
            destPosition = new Vector3
            (
                x,
                y,
                z
            );
            isArrived = false;
        }
    }

    /// <summary>
    /// 수호자 인공지능의 이동 함수이지만, move의 이벤트로 사용되지 않는 함수입니다.
    /// </summary>
    private void MoveGuardian()
    {
        startPosition = GetComponent<Transform>().position;
        destPosition = new Vector3(Random.Range(Boundary.xMin, Boundary.xMax),
            Random.Range(Boundary.yMin, Boundary.yMax), Boundary.RoundZ(Random.Range(Boundary.zMin, Boundary.zMax)));
        invincibleTime = maxInvincibleTime;
        myShield = Instantiate(divineShield, GetComponent<Transform>());
    }

    /// <summary>
    /// 이동하지 않는 함수입니다.
    /// 튜토리얼과 수호자 인공지능에 사용됩니다.
    /// </summary>
    private void MoveNever()
    {

    }

    #endregion

    #region 던지는(Shoot) 함수들

    /// <summary>
    /// 투사체를 던지는 함수입니다. 
    /// 쉬움 인공지능에 사용됩니다.
    /// </summary>
    private void ShootEasy()
    {
        if (!isCharging && !IsInvincible)
        {
            chargedZ = 0f;
            prepareWeaponTime = 0f;
            isCharging = true;
            exactTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
            approxZ = GaussianRandom() * 1.2f;
        }
        else if (isCharging && (chargedZ < Mathf.Abs(approxZ) || prepareWeaponTime < Manager.instance.PrepareChargeTime))
        {
            chargedZ += Time.fixedDeltaTime * chargeSpeed;
            prepareWeaponTime += Time.fixedDeltaTime;
        }
        else if (isCharging && chargedZ >= Mathf.Abs(approxZ) && prepareWeaponTime >= Manager.instance.PrepareChargeTime)
        {
            GameObject k = Instantiate(knife, GetComponent<Transform>().position, Quaternion.identity);
            k.GetComponent<Knife>().Initialize(1, 0, exactTarget + new Vector3(GaussianRandom() * 0.5f, GaussianRandom() * 0.5f, Boundary.RoundZ(approxZ)));
            isCharging = false;
            weaponToSummon.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// 투사체를 던지는 함수입니다. 
    /// 어려움 인공지능에 사용됩니다.
    /// </summary>
    private void ShootHard()
    {
        if (!isCharging)
        {
            chargedZ = 0f;
            prepareWeaponTime = 0f;
            isCharging = true;
            exactTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
            approxZ = GaussianRandom() * 0.9f;
        }
        else if (isCharging && (chargedZ < Mathf.Abs(approxZ) || prepareWeaponTime < Manager.instance.PrepareChargeTime))
        {
            chargedZ += Time.fixedDeltaTime * chargeSpeed;
            prepareWeaponTime += Time.fixedDeltaTime;
        }
        else if (isCharging && chargedZ >= Mathf.Abs(approxZ) && prepareWeaponTime >= Manager.instance.PrepareChargeTime)
        {
            GameObject k = Instantiate(knife, GetComponent<Transform>().position, Quaternion.identity);
            k.GetComponent<Knife>().Initialize(1, 0, exactTarget + new Vector3(GaussianRandom() * 0.5f, GaussianRandom() * 0.5f, Boundary.RoundZ(approxZ)));
            isCharging = false;
            weaponToSummon.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// 투사체를 던지는 함수입니다.
    /// 수호자(어려움) 인공지능에 사용됩니다.
    /// </summary>
    private void ShootInsane()
    {
        if (!isCharging)
        {
            chargedZ = 0f;
            prepareWeaponTime = 0f;
            isCharging = true;
            exactTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
            approxZ = GaussianRandom() * 0.5f;
        }
        else if (isCharging && (chargedZ < Mathf.Abs(approxZ) || prepareWeaponTime < Manager.instance.PrepareChargeTime))
        {
            chargedZ += Time.fixedDeltaTime * chargeSpeed;
            prepareWeaponTime += Time.fixedDeltaTime;
        }
        else if (isCharging && chargedZ >= Mathf.Abs(approxZ) && prepareWeaponTime >= Manager.instance.PrepareChargeTime)
        {
            GameObject k = Instantiate(knife, GetComponent<Transform>().position, Quaternion.identity);
            k.GetComponent<Knife>().Initialize(1, 0, exactTarget + new Vector3(GaussianRandom() * 0.8f, GaussianRandom() * 0.8f, Boundary.RoundZ(approxZ)));
            isCharging = false;
            weaponToSummon.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// 투사체를 던지지 않는 함수입니다.
    /// 튜토리얼에 사용됩니다.
    /// </summary>
    private void ShootNever()
    {

    }

    #endregion

    #region 공격받을 때(Damaged) 실행될 함수들

    /// <summary>
    /// 공격받았을 때 실행될 함수입니다.
    /// 방랑자, 추적자 인공지능에 사용됩니다.
    /// </summary>
    public void DamagedVS()
    {
        if (Manager.instance.GetGameOver()) return;

        if (Health > 0 && invincibleTime <= 0f)
        {
            Debug.LogWarning("Enemy hit!");
            health--;
            if (hearts.Count > Health)
            {
                hearts[Health].SetActive(false);
            }
            if (Health > 0)
            {
                invincibleTime = maxInvincibleTime;
                myShield = Instantiate(divineShield, GetComponent<Transform>());
                StartCoroutine("DamagedSFX");
            }
        }
        else if (Health > 0 && invincibleTime > 0f)
        {
            Debug.Log("Enemy guarded!");
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
            Manager.instance.WinGame();
        }
    }

    /// <summary>
    /// 공격받았을 때 실행될 함수입니다.
    /// 수호자 인공지능에 사용됩니다.
    /// </summary>
    public void DamagedGuardian()
    {
        if (Manager.instance.GetGameOver()) return;

        if (Health > 0 && invincibleTime <= 0f)
        {
            Debug.LogWarning("Enemy hit!");
            health--;
            if (hearts.Count > Health)
            {
                hearts[Health].SetActive(false);
            }
            if (Health > 0)
            {
                startPosition = GetComponent<Transform>().position;
                destPosition = new Vector3(Random.Range(Boundary.xMin, Boundary.xMax),
                    Random.Range(Boundary.yMin, Boundary.yMax),
                    Boundary.RoundZ(Random.Range(Boundary.zMin, Boundary.zMax)));
                invincibleTime = maxInvincibleTime;
                myShield = Instantiate(divineShield, GetComponent<Transform>());
                StartCoroutine("DamagedSFX");
            }
        }
        else if (Health > 0 && invincibleTime > 0f)
        {
            Debug.Log("Enemy guarded!");
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
            Manager.instance.WinGame();

        }
    }

    /// <summary>
    /// 공격받았을 때 실행될 함수입니다.
    /// 튜토리얼에 사용됩니다.
    /// </summary>
    public void DamagedTutorial()
    {
        if (Manager.instance.GetGameOver()) return;

        if (Health > 0 && invincibleTime <= 0f)
        {
            Debug.LogWarning("Enemy hit!");
            health--;
            if (hearts.Count > Health)
            {
                hearts[Health].SetActive(false);
            }
            if (Health > 0)
            {
                startPosition = GetComponent<Transform>().position;
                if (Health == 2)
                {
                    destPosition = new Vector3(-1f, 0.05f, Boundary.RoundZ(4f));
                    GameObject.FindGameObjectWithTag("Player").GetComponent<TutorialManager>().tutorialText.text =
                        "주인공은 페이지를 뚫고 다른 페이지로 칼을 던질 수 있습니다.\n" +
                        "마우스 왼쪽을 눌렀다 떼면 앞 페이지로, 오른쪽을 눌렀다 떼면 뒤 페이지로 칼이 날아갑니다.\n" +
                        "마우스를 누르고 있으면 조준점에 작은 시계가 나타납니다.\n" +
                        "이 시계의 보라색 침은 칼이 향할 페이지를, 빨간색 침은 상대가 있는 페이지를 가리킵니다.\n" +
                        "마우스를 오래 누를수록 더 먼 페이지로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                        "<color=#ff00bf>마우스로 상대를 조준하고, 보라색 침과 빨간색 침이 겹칠 때까지 눌렀다가 떼세요.</color>\n" +
                        "\"움직이지 않는 상대를 향해 칼을 던져서 2번 더 맞추세요.\"";
                }
                else if (Health == 1)
                {
                    destPosition = new Vector3(0.1f, 0.3f, Boundary.RoundZ(-3.5f));
                    GameObject.FindGameObjectWithTag("Player").GetComponent<TutorialManager>().tutorialText.text =
                        "주인공은 페이지를 뚫고 다른 페이지로 칼을 던질 수 있습니다.\n" +
                        "마우스 왼쪽을 눌렀다 떼면 앞 페이지로, 오른쪽을 눌렀다 떼면 뒤 페이지로 칼이 날아갑니다.\n" +
                        "마우스를 누르고 있으면 조준점에 작은 시계가 나타납니다.\n" +
                        "이 시계의 보라색 침은 칼이 향할 페이지를, 빨간색 침은 상대가 있는 페이지를 가리킵니다.\n" +
                        "마우스를 오래 누를수록 더 먼 페이지로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                        "<color=#ff00bf>마우스로 상대를 조준하고, 보라색 침과 빨간색 침이 겹칠 때까지 눌렀다가 떼세요.</color>\n" +
                        "\"움직이지 않는 상대를 향해 칼을 던져서 1번 더 맞추세요.\"";
                }
                invincibleTime = maxInvincibleTime;
                myShield = Instantiate(divineShield, GetComponent<Transform>());
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
            Manager.instance.GraduateTutorial();

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

    #endregion

    #region 대사 재생 함수들

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
        mySpeech.GetComponentInChildren<Text>().text = "이 전투를 기다려왔다!";
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
    
    IEnumerator EndSpeech()
    {
        if (mySpeech != null)
        {
            Destroy(mySpeech);
        }
        mySpeech = Instantiate(speechBubble, speechVector, Quaternion.identity, Manager.instance.Canvas.GetComponent<Transform>());
        mySpeech.GetComponentInChildren<Text>().text = "이 세계에 파멸을...";
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
        yield return new WaitForSeconds(3.0f);
        Destroy(mySpeech);
        mySpeech = null;
    }

    /// <summary>
    /// 튜토리얼에 사용될 대사를 재생합니다.
    /// num은 1 이상 4 이하여야 합니다.
    /// </summary>
    /// <param name="num"></param>
    public void SpeakTutorial(int num)
    {
        if (num < 1 || num > 4) return;
        StartCoroutine("TutorialSpeech" + num.ToString());
    }

    IEnumerator TutorialSpeech1()
    {
        if (mySpeech != null)
        {
            Destroy(mySpeech);
        }
        GetComponent<AudioSource>().clip = tutorialSound1;  // TODO
        GetComponent<AudioSource>().Play();
        mySpeech = Instantiate(speechBubble, speechVector, Quaternion.identity, Manager.instance.Canvas.GetComponent<Transform>());
        mySpeech.GetComponentInChildren<Text>().text = "으읍, 이게 뭐냐!";
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
        yield return new WaitForSeconds(0.9f);
        GetComponent<AudioSource>().clip = tutorialSound2;  // TODO
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1.4f);
        Destroy(mySpeech);
        mySpeech = null;
    }

    IEnumerator TutorialSpeech2()
    {
        yield return null;
    }

    IEnumerator TutorialSpeech3()
    {
        yield return null;
    }

    IEnumerator TutorialSpeech4()
    {
        yield return null;
    }

    #endregion

    IEnumerator DamagedSFX()
    {
        GetComponent<AudioSource>().clip = damagedSound;
        GetComponent<AudioSource>().Play();
        isSFXPlaying = true;
        yield return new WaitForSeconds(0.5f);
        isSFXPlaying = false;
    }

    public void SetModelVisibleInTutorial(bool b)
    {
        isModelVisible = b;
    }

    /// <summary>
    /// 표준정규분포를 따르는 랜덤한 값을 생성합니다.
    /// </summary>
    /// <returns></returns>
    private float GaussianRandom()
    {
        float u1 = 1.0f - Random.Range(0f, 1f); // uniform(0,1] random doubles
        float u2 = 1.0f - Random.Range(0f, 1f);
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                     Mathf.Sin(2.0f * Mathf.PI * u2); // random normal(0,1)
        return randStdNormal;
    }
}
