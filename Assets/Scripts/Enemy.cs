using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public float speed;
    public float temporalSpeed;         // 시간 축을 따라 초당 움직이는 칸 수입니다.
    public GameObject knife;
    public GameObject divineShield;
    public List<GameObject> hearts;
    public AudioClip damagedSound;
    public AudioClip guardSound;
    public AudioClip killedSound;
    public GameObject blow;
    public delegate void Damaged();
    public Damaged damaged;

    private int health = 3;
    private float chargeSpeed = 0f;
    private GameObject myShield;
    private Vector3 exactTarget;
    private Vector3 startPosition;
    private Vector3 destPosition;
    private bool isArrived = true;
    private bool isCharging = false;
    private float chargedZ;
    private float approxZ;              // 플레이어 캐릭터 근처의, 칼을 발사할 지점의 Z좌표
    private float invincibleTime;       // 피격 후 무적 판정이 되는, 남은 시간 
    private float maxInvincibleTime = 3f;
    private float temporalMoveCoolTime; // 시간 축을 따라 한 칸 이동하고 다음 한 칸을 이동하기까지 대기하는 시간입니다.
    private float easyMoveCoolTime = 0f;// 쉬움 난이도에서 목적지에 도착하고 다시 움직이기까지 대기하는 시간입니다.
    private Rigidbody r;
    private Transform t;
    private GameObject blowend;
    private Player player;
    private delegate void WhileInvincible();
    private delegate void Vanish();
    private delegate void Move();
    private delegate void Shoot();
    private WhileInvincible whileInvincible;
    private Vanish vanish;
    private Move move;
    private Shoot shoot;

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
    }

    void Start()
    {
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
        else if (gameLevel.Equals("Hard") && !gameMode.Equals("Tutorial"))
        {
            shoot += ShootHard;
            chargeSpeed = Manager.instance.HardChargeSpeed;
        }
        else if (gameLevel.Equals("Easy"))
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
    }

    void FixedUpdate ()
    {
        whileInvincible();

        if (health <= 0) return;

        // 플레이어 캐릭터와의 Z좌표(시간축 좌표) 차이에 따라 투명도를 적용합니다.
        if (vanish.GetInvocationList().Length > 0) {
            vanish();
        }

        if (Manager.instance.GetGameOver()) return;

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
        float alpha = Mathf.Max(0f, 1f - Mathf.Pow(Mathf.Abs(player.GetComponent<Transform>().position.z - t.position.z), 2));

        if (1 - Mathf.Abs(GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position.z - t.position.z) < 0)
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = false;
            }
        }
        else if (Mathf.Abs(player.GetComponent<Transform>().position.z - t.position.z) < Boundary.OnePageToDeltaZ() * 0.8f)
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = true;
            }
            Material m = GetComponentInChildren<CharacterModel>().GetComponent<MeshRenderer>().material;
            m.color = ColorUtil.instance.AlphaColor(ColorUtil.instance.presentEnemyColor, alpha);
        }
        else if (t.position.z < player.GetComponent<Transform>().position.z)
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = true;
            }
            Material m = GetComponentInChildren<CharacterModel>().GetComponent<MeshRenderer>().material;
            m.color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.pastColor, ColorUtil.instance.pastPastColor,
                Mathf.Abs(player.GetComponent<Transform>().position.z - t.position.z) - Boundary.OnePageToDeltaZ() * 0.8f), alpha);
        }
        else
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = true;
            }
            Material m = GetComponentInChildren<CharacterModel>().GetComponent<MeshRenderer>().material;
            m.color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.futureColor, ColorUtil.instance.futureFutureColor,
                Mathf.Abs(player.GetComponent<Transform>().position.z - t.position.z) - Boundary.OnePageToDeltaZ() * 0.8f), alpha);
        }
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
            easyMoveCoolTime = Random.Range(0f, 1f);
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
            easyMoveCoolTime = Random.Range(0f, 1f);
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
        if (!isCharging)
        {
            chargedZ = 0f;
            isCharging = true;
            exactTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
            approxZ = GaussianRandom() * 1f;
        }
        else if (isCharging && chargedZ < Mathf.Abs(approxZ))
        {
            chargedZ += Time.fixedDeltaTime * chargeSpeed;
            /*
                        * Vector2.Distance(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y),
                        new Vector2(exactTarget.x, exactTarget.y));
            */
        }
        else if (isCharging && chargedZ >= Mathf.Abs(approxZ))
        {
            GameObject k = Instantiate(knife, GetComponent<Transform>().position, Quaternion.identity);
            k.GetComponent<Knife>().Initialize(1, exactTarget + new Vector3(GaussianRandom() * 0.5f, GaussianRandom() * 0.5f, Boundary.RoundZ(approxZ)));
            isCharging = false;
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
            isCharging = true;
            exactTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
            approxZ = GaussianRandom() * 0.5f;
        }
        else if (isCharging && chargedZ < Mathf.Abs(approxZ))
        {
            chargedZ += Time.fixedDeltaTime * chargeSpeed;
            /*
                        * Vector2.Distance(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y),
                        new Vector2(exactTarget.x, exactTarget.y));
            */
        }
        else if (isCharging && chargedZ >= Mathf.Abs(approxZ))
        {
            GameObject k = Instantiate(knife, GetComponent<Transform>().position, Quaternion.identity);
            k.GetComponent<Knife>().Initialize(1, exactTarget + new Vector3(GaussianRandom() * 0.5f, GaussianRandom() * 0.5f, Boundary.RoundZ(approxZ)));
            isCharging = false;
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
            isCharging = true;
            exactTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
            approxZ = GaussianRandom() * 0.2f;
        }
        else if (isCharging && chargedZ < Mathf.Abs(approxZ))
        {
            chargedZ += Time.fixedDeltaTime * chargeSpeed;
            /*
                        * Vector2.Distance(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y),
                        new Vector2(exactTarget.x, exactTarget.y));
            */
        }
        else if (isCharging && chargedZ >= Mathf.Abs(approxZ))
        {
            GameObject k = Instantiate(knife, GetComponent<Transform>().position, Quaternion.identity);
            k.GetComponent<Knife>().Initialize(1, exactTarget + new Vector3(GaussianRandom() * 0.8f, GaussianRandom() * 0.8f, Boundary.RoundZ(approxZ)));
            isCharging = false;
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
                GetComponent<AudioSource>().clip = damagedSound;
                GetComponent<AudioSource>().Play();
            }
        }
        else if (Health > 0 && invincibleTime > 0f)
        {
            GetComponent<AudioSource>().clip = guardSound;
            GetComponent<AudioSource>().Play();
        }

        if (Health <= 0 && GetComponentInChildren<CharacterModel>().gameObject.activeInHierarchy)
        {
            invincibleTime = 0f;
            GetComponentInChildren<CharacterModel>().gameObject.SetActive(false);
            r.velocity = Vector3.zero;
            GetComponent<AudioSource>().clip = killedSound;
            GetComponent<AudioSource>().Play();

            blowend = Instantiate(blow, GetComponent<Transform>().position, Quaternion.identity);

            StartCoroutine("Blow");
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
                GetComponent<AudioSource>().clip = damagedSound;
                GetComponent<AudioSource>().Play();
            }
        }
        else if (Health > 0 && invincibleTime > 0f)
        {
            GetComponent<AudioSource>().clip = guardSound;
            GetComponent<AudioSource>().Play();
        }

        if (Health <= 0 && GetComponentInChildren<CharacterModel>().gameObject.activeInHierarchy)
        {
            invincibleTime = 0f;
            GetComponentInChildren<CharacterModel>().gameObject.SetActive(false);
            r.velocity = Vector3.zero;
            GetComponent<AudioSource>().clip = killedSound;
            GetComponent<AudioSource>().Play();

            blowend = Instantiate(blow, GetComponent<Transform>().position, Quaternion.identity);

            StartCoroutine("Blow");
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
                        "마우스 왼쪽을 눌러 과거로, 또는 마우스 오른쪽을 눌러 미래로 칼을 던질 수 있습니다.\n" +
                        "마우스를 누르고 있으면 작은 시계가 나타납니다.\n이 시계의 파란색 침은 칼이 향할 시간을 가리킵니다.\n" +
                        "마우스를 오래 누를수록 더 먼 과거(미래)로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                        "마우스로 상대를 조준하고 파란색 침과 빨간색 침이 겹칠 때까지 눌렀다가 떼세요.\n" +
                        "\"움직이지 않는 상대를 향해 칼을 던져서 2번 더 맞추세요.\"";
                }
                else if (Health == 1)
                {
                    destPosition = new Vector3(0.1f, 0.3f, Boundary.RoundZ(-3.5f));
                    GameObject.FindGameObjectWithTag("Player").GetComponent<TutorialManager>().tutorialText.text =
                        "마우스 왼쪽을 눌러 과거로, 또는 마우스 오른쪽을 눌러 미래로 칼을 던질 수 있습니다.\n" +
                        "마우스를 누르고 있으면 작은 시계가 나타납니다.\n이 시계의 파란색 침은 칼이 향할 시간을 가리킵니다.\n" +
                        "마우스를 오래 누를수록 더 먼 과거(미래)로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                        "마우스로 상대를 조준하고 파란색 침과 빨간색 침이 겹칠 때까지 눌렀다가 떼세요.\n" +
                        "\"움직이지 않는 상대를 향해 칼을 던져서 1번 더 맞추세요.\"";
                }
                invincibleTime = maxInvincibleTime;
                myShield = Instantiate(divineShield, GetComponent<Transform>());
                GetComponent<AudioSource>().clip = damagedSound;
                GetComponent<AudioSource>().Play();
            }
        }
        else if (Health > 0 && invincibleTime > 0f)
        {
            GetComponent<AudioSource>().clip = guardSound;
            GetComponent<AudioSource>().Play();
        }

        if (Health <= 0 && GetComponentInChildren<CharacterModel>().gameObject.activeInHierarchy)
        {
            invincibleTime = 0f;
            GetComponentInChildren<CharacterModel>().gameObject.SetActive(false);
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
