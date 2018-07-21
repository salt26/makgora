using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEnemy : MonoBehaviour
{
    
    public GameObject divineShield;
    public GameObject graduatePanel;
    public List<GameObject> hearts;
    public AudioClip killedSound;
    public GameObject blow;

    private int health = 3;
    private GameObject myShield;
    private Vector3 exactTarget;
    private Vector3 dest;
    private bool isArrived = true;
    private bool isCharging = false;
    private float chargedZ;
    private float approxZ;              // 플레이어 캐릭터 근처의, 칼을 발사할 지점의 Z좌표
    private float invincibleTime;       // 피격 후 무적 판정이 되는, 남은 시간 
    private Rigidbody r;
    private Transform t;
    private GameObject blowend;

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

    void Awake()
    {
        r = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
        chargedZ = 0f;
        myShield = null;
        blowend = null;
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

        if (health <= 0) return;
    }

    public void Damaged()
    {
        if (Health > 0 && invincibleTime <= 0f)
        {
            Debug.LogWarning("Enemy hit!");
            health--;
            if (hearts.Count > Health)
            {
                hearts[Health].SetActive(false);
            }
            if (Health > 0f)
            {
                invincibleTime = 3f;
                myShield = Instantiate(divineShield, GetComponent<Transform>());
                GetComponent<AudioSource>().Play();
            }
        }
        if (Health <= 0 && GetComponentInChildren<CharacterModel>().gameObject.activeInHierarchy)
        {
            invincibleTime = 0f;
            GetComponentInChildren<CharacterModel>().gameObject.SetActive(false);
            r.velocity = Vector3.zero;
            GetComponent<AudioSource>().clip = killedSound;
            GetComponent<AudioSource>().Play();

            blowend = Instantiate(blow, GetComponent<Transform>().position, Quaternion.identity);

            StartCoroutine("Graduate");

        }
    }

    IEnumerator Graduate()
    {
        yield return new WaitForSeconds(1.0f);
        if (blowend != null)
        {
            Destroy(blowend);
        }
        blowend = null;
        yield return new WaitForSeconds(1.0f);
        graduatePanel.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.5f);
        graduatePanel.SetActive(true);
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
