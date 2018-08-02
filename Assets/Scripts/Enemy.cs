using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public float speed;
    public float chargeSpeed;
    public GameObject knife;
    public GameObject divineShield;
    public GameObject restartPanel;
    public Text restartText;
    public List<GameObject> hearts;
    public AudioClip damagedSound;
    public AudioClip guardSound;
    public AudioClip killedSound;
    public AudioClip winSound;
    public GameObject blow;

    protected int health = 3;
    protected GameObject myShield;
    protected Vector3 exactTarget;
    protected Vector3 dest;
    protected bool isArrived = true;
    protected bool isCharging = false;
    protected float chargedZ;
    protected float approxZ;              // 플레이어 캐릭터 근처의, 칼을 발사할 지점의 Z좌표
    protected float invincibleTime;       // 피격 후 무적 판정이 되는, 남은 시간 
    protected Rigidbody r;
    protected Transform t;
    protected GameObject blowend;

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
    }
	
	void FixedUpdate () {
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

        if (health <= 0) return;
        /*
        // 플레이어 캐릭터와의 Z좌표(시간축 좌표) 차이에 따라 투명도를 적용합니다.
        foreach (SkinnedMeshRenderer mr in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach (Material m in mr.materials)
            {
                m.color = new Color(m.color.r, m.color.g, m.color.b,
                    Mathf.Max(0, Mathf.Pow(Mathf.Abs(
                        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position.z - t.position.z) - 1, 2)));
            }
        }
        */
        if (!isArrived && Vector3.Distance(t.position, dest) < 0.01f)
        {
            // 목적지에 도착했습니다.
            isArrived = true;
        }
        else if (!isArrived)
        {
            // 목적지를 향해 이동합니다.
            Vector3 movement = dest - t.position;
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
            dest = new Vector3
            (
                Random.Range(Boundary.xMin, Boundary.xMax), 
                Random.Range(Boundary.yMin, Boundary.yMax),
                z
            );
            isArrived = false;
        }
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

    public virtual void Damaged()
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

            StartCoroutine("Restart");

        }
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(1.0f);
        if (blowend != null)
        {
            Destroy(blowend);
        }
        blowend = null;
        yield return new WaitForSeconds(1.0f);
        restartPanel.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.5f);
        restartText.text = "YOU WIN!";
        restartPanel.SetActive(true);
        GetComponent<AudioSource>().clip = winSound;
        GetComponent<AudioSource>().Play();
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
