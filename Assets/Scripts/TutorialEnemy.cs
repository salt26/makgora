using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEnemy : MonoBehaviour
{
    
    public GameObject divineShield;
    public GameObject graduatePanel;
    public GameObject skipButton;
    public List<GameObject> hearts;
    public AudioClip damagedSound;
    public AudioClip guardSound;
    public AudioClip killedSound;
    public AudioClip winSound;
    public GameObject blow;

    private int health = 3;
    private GameObject myShield;
    private Vector3 start;
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
            t.SetPositionAndRotation(Vector3.Lerp(dest, start, invincibleTime / 3f), Quaternion.identity);
            //r.velocity = (dest - start) / 3f;
            //r.position = Vector3.Lerp(dest, start, invincibleTime / 3f);
        }
        if (invincibleTime < 0f)
        {
            invincibleTime = 0f;
        }
        if (invincibleTime <= 0f && myShield != null)
        {
            t.SetPositionAndRotation(dest, Quaternion.identity);
            //r.velocity = Vector3.zero;
            //r.MovePosition(dest);
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
            if (Health > 0)
            {
                start = GetComponent<Transform>().position;
                if (Health == 2)
                {
                    dest = new Vector3(-1f, 0.05f, 2.89f);
                    GameObject.FindGameObjectWithTag("Player").GetComponent<TutorialManager>().tutorialText.text =
                        "마우스 왼쪽을 눌러 과거로, 또는 마우스 오른쪽을 눌러 미래로 칼을 던질 수 있습니다.\n" +
                        "마우스를 누르고 있으면 작은 시계가 나타납니다.\n이 시계의 초록색 침은 칼이 향할 시간을 가리킵니다.\n" +
                        "마우스를 오래 누를수록 더 먼 과거(미래)로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                        "\"움직이지 않는 상대를 향해 칼을 던져서 2번 더 맞추세요.\"";
                }
                else if (Health == 1)
                {
                    dest = new Vector3(0.1f, 0.3f, -3.5f);
                    GameObject.FindGameObjectWithTag("Player").GetComponent<TutorialManager>().tutorialText.text =
                        "마우스 왼쪽을 눌러 과거로, 또는 마우스 오른쪽을 눌러 미래로 칼을 던질 수 있습니다.\n" +
                        "마우스를 누르고 있으면 작은 시계가 나타납니다.\n이 시계의 초록색 침은 칼이 향할 시간을 가리킵니다.\n" +
                        "마우스를 오래 누를수록 더 먼 과거(미래)로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
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

            StartCoroutine("Graduate");

        }
    }

    IEnumerator Graduate()
    {
        skipButton.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        if (blowend != null)
        {
            Destroy(blowend);
        }
        blowend = null;
        yield return new WaitForSeconds(1.0f);
        graduatePanel.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.5f);
        graduatePanel.SetActive(true);

        GetComponent<AudioSource>().clip = winSound;
        GetComponent<AudioSource>().Play();
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
