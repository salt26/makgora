using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public float speed;
    public float chargeSpeed;
    public GameObject knife;
    public GameObject divineShield;
    public List<GameObject> hearts;
    public AudioClip damagedSound;
    public AudioClip guardSound;
    public AudioClip killedSound;
    public GameObject blow;
    public delegate void Damaged();
    public Damaged damaged;

    protected int health = 3;
    protected GameObject myShield;
    protected Vector3 exactTarget;
    protected Vector3 startPosition;
    protected Vector3 destPosition;
    protected bool isArrived = true;
    protected bool isCharging = false;
    protected float chargedZ;
    protected float approxZ;              // 플레이어 캐릭터 근처의, 칼을 발사할 지점의 Z좌표
    protected float invincibleTime;       // 피격 후 무적 판정이 되는, 남은 시간 
    protected Rigidbody r;
    protected Transform t;
    protected GameObject blowend;
    protected Player player;
    protected delegate void WhileInvincible();
    protected delegate void Vanish();
    protected delegate void Move();
    protected delegate void Shoot();
    protected WhileInvincible whileInvincible;
    protected Vanish vanish;
    protected Move move;
    protected Shoot shoot;

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
            move += MoveVagabond;
            damaged += DamagedVS;
        }
        else if (gameMode.Equals("Stalker"))
        {
            whileInvincible += WINormal;
            vanish += VanishNormal;
            move += MoveStalker;
            damaged += DamagedVS;
        }

        if (gameLevel.Equals("Hard") && !gameMode.Equals("Tutorial"))
        {
            shoot += ShootHard;
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

        move();
        
        if (Manager.instance.GetGameOver()) return;

        shoot();
    }

    /// <summary>
    /// 무적인 동안 매 프레임마다 실행될 함수입니다.
    /// 방랑자, 추적자 인공지능에 사용됩니다.
    /// </summary>
    protected void WINormal()
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
    }

    /// <summary>
    /// 무적인 동안 매 프레임마다 실행될 함수입니다.
    /// 수호자 인공지능과 튜토리얼에 사용됩니다.
    /// </summary>
    private void WIMove()
    {
        if (invincibleTime > 0f)
        {
            invincibleTime -= Time.fixedDeltaTime;
            t.SetPositionAndRotation(Vector3.Lerp(destPosition, startPosition, invincibleTime / 3f), Quaternion.identity);
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

    /// <summary>
    /// 시간대가 다를 때 투명해지도록 하는 함수입니다.
    /// </summary>
    protected void VanishNormal()
    {
        if (1 - Mathf.Abs(GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position.z - t.position.z) < 0)
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = false;
            }
        }
        else
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = true;
                foreach (Material m in mr.materials)
                {
                    /*
                    m.color = new Color(m.color.r, m.color.g, m.color.b,
                        Mathf.Pow(Mathf.Max(0, 1 - Mathf.Abs(
                            GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position.z - t.position.z)), 2));
                    */
                    m.color = new Color(m.color.r, m.color.g, m.color.b, Mathf.Max(0, 1 - Mathf.Abs(
                            GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position.z - t.position.z)));
                }
            }
        }
    }

    /// <summary>
    /// 이동 함수입니다.
    /// 방랑자 인공지능에 사용됩니다.
    /// </summary>
    protected void MoveVagabond()
    {
        if (!isArrived && Vector3.Distance(t.position, destPosition) < 0.01f)
        {
            // 목적지에 도착했습니다.
            isArrived = true;
        }
        else if (!isArrived && Mathf.Abs(t.position.z - destPosition.z) > 0.01f)
        {
            // 목적지를 향해 시간 축을 따라 이동합니다.
            Vector3 movement = destPosition - t.position;
            movement.x = 0f;
            movement.y = 0f;
            r.velocity = movement.normalized * speed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
            );
        }
        else if (!isArrived)
        {
            // 목적지를 향해 XY평면을 따라 이동합니다.
            Vector3 movement = destPosition - t.position;
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
            float z = t.position.z + GaussianRandom();
            z = Mathf.Clamp(z, Boundary.zMin, Boundary.zMax);
            destPosition = new Vector3
            (
                Random.Range(Boundary.xMin, Boundary.xMax),
                Random.Range(Boundary.yMin, Boundary.yMax),
                z
            );
            isArrived = false;
        }
    }

    /// <summary>
    /// 이동 함수입니다.
    /// 추적자 인공지능에 사용됩니다.
    /// </summary>
    protected void MoveStalker()
    {
        if (!isArrived && Vector3.Distance(t.position, destPosition) < 0.01f)
        {
            // 목적지에 도착했습니다.
            isArrived = true;
        }
        else if (!isArrived && Mathf.Abs(t.position.z - destPosition.z) > 0.01f)
        {
            // 목적지를 향해 시간 축을 따라 이동합니다.
            Vector3 movement = destPosition - t.position;
            movement.x = 0f;
            movement.y = 0f;
            r.velocity = movement.normalized * speed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
            );
        }
        else if (!isArrived)
        {
            // 목적지를 향해 XY평면을 따라 이동합니다.
            Vector3 movement = destPosition - t.position;
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
            float x = playerPosition.x + 0.3f * GaussianRandom();
            float y = playerPosition.y + 0.3f * GaussianRandom();
            float z = playerPosition.z + 0.5f * GaussianRandom();
            x = Mathf.Clamp(x, Boundary.xMin, Boundary.xMax);
            y = Mathf.Clamp(y, Boundary.yMin, Boundary.yMax);
            z = Mathf.Clamp(z, Boundary.zMin, Boundary.zMax);
            destPosition = new Vector3
            (
                x,
                y,
                z
            );
            isArrived = false;
        }
    }

    protected void MoveNever()
    {

    }

    /// <summary>
    /// 칼을 던지는 함수입니다. 기본은 어려움 인공지능입니다.
    /// </summary>
    protected void ShootHard()
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
            chargedZ += Time.fixedDeltaTime * chargeSpeed
                        * Vector2.Distance(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y),
                        new Vector2(exactTarget.x, exactTarget.y));
        }
        else if (isCharging && chargedZ >= Mathf.Abs(approxZ))
        {
            GameObject k = Instantiate(knife, GetComponent<Transform>().position, Quaternion.identity);
            k.GetComponent<Knife>().Initialize(1, exactTarget + new Vector3(GaussianRandom() * 0.3f, GaussianRandom() * 0.3f, approxZ));
            isCharging = false;
        }
    }

    protected void ShootNever()
    {

    }

    /// <summary>
    /// 공격받았을 때 실행될 함수입니다.
    /// 방랑자, 추적자 인공지능에 사용됩니다.
    /// </summary>
    public void DamagedVS()
    {
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
                invincibleTime = 3f;
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
                    destPosition = new Vector3(-1f, 0.05f, 2.89f);
                    GameObject.FindGameObjectWithTag("Player").GetComponent<TutorialManager>().tutorialText.text =
                        "마우스 왼쪽을 눌러 과거로, 또는 마우스 오른쪽을 눌러 미래로 칼을 던질 수 있습니다.\n" +
                        "마우스를 누르고 있으면 작은 시계가 나타납니다.\n이 시계의 초록색 침은 칼이 향할 시간을 가리킵니다.\n" +
                        "마우스를 오래 누를수록 더 먼 과거(미래)로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                        "마우스로 상대를 조준하고 초록색 침과 파란색 침이 겹칠 때까지 눌렀다가 떼세요.\n" +
                        "\"움직이지 않는 상대를 향해 칼을 던져서 2번 더 맞추세요.\"";
                }
                else if (Health == 1)
                {
                    destPosition = new Vector3(0.1f, 0.3f, -3.5f);
                    GameObject.FindGameObjectWithTag("Player").GetComponent<TutorialManager>().tutorialText.text =
                        "마우스 왼쪽을 눌러 과거로, 또는 마우스 오른쪽을 눌러 미래로 칼을 던질 수 있습니다.\n" +
                        "마우스를 누르고 있으면 작은 시계가 나타납니다.\n이 시계의 초록색 침은 칼이 향할 시간을 가리킵니다.\n" +
                        "마우스를 오래 누를수록 더 먼 과거(미래)로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                        "마우스로 상대를 조준하고 초록색 침과 파란색 침이 겹칠 때까지 눌렀다가 떼세요.\n" +
                        "\"움직이지 않는 상대를 향해 칼을 던져서 1번 더 맞추세요.\"";
                }
                invincibleTime = 3f;
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

    

    /// <summary>
    /// 표준정규분포를 따르는 랜덤한 값을 생성합니다.
    /// </summary>
    /// <returns></returns>
    protected float GaussianRandom()
    {
        float u1 = 1.0f - Random.Range(0f, 1f); // uniform(0,1] random doubles
        float u2 = 1.0f - Random.Range(0f, 1f);
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                     Mathf.Sin(2.0f * Mathf.PI * u2); // random normal(0,1)
        return randStdNormal;
    }
}
