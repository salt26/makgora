using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public float speed;
    public float temporalSpeed;         // 시간 축을 따라 초당 움직이는 칸 수입니다.
    public GameObject target;           // 마우스 클릭 지점 프리팹입니다.
    public GameObject knife;            // 칼 프리팹입니다.
    public GameObject divineShield;
    public List<GameObject> hearts;
    public AudioClip damagedSound;
    public AudioClip guardSound;
    public AudioClip killedSound;
    public GameObject blow;
    public GameObject zLocation;
    public RectTransform purplePage;
    public Text purpleText;

    private int health = 3;
    private float chargeSpeed;          // 마우스 클릭 시 Z좌표가 증가(감소)하는 속도입니다.
    private GameObject targetObject;    // 현재 화면에 나타난 마우스 클릭 지점 오브젝트를 관리합니다.
    private GameObject myShield;
    private float chargedZ;             // 칼을 발사할 목적지 방향의 Z좌표(시간축 좌표)입니다.
    private float invincibleTime;       // 피격 후 무적 판정이 되는, 남은 시간 
    private float temporalMoveCoolTime; // 시간 축을 따라 한 칸 이동하고 다음 한 칸을 이동하기까지 대기하는 시간입니다.
    private Rigidbody r;
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

    private void Awake()
    {
        r = GetComponent<Rigidbody>();
        chargedZ = 0f;
        myShield = null;
        blowend = null;
    }

    private void Start()
    {
        string gameLevel = Manager.instance.GetCurrentGame()[1];
        if (gameLevel.Equals("Easy") || (Manager.instance.GetCurrentGame()[0].Equals("Tutorial")))
        {
            chargeSpeed = Manager.instance.EasyChargeSpeed;
        }
        else if (gameLevel.Equals("Hard"))
        {
            chargeSpeed = Manager.instance.HardChargeSpeed;
        }
    }

    // Update is called once per frame
    void Update () {
        if (health <= 0)
        {
            if (targetObject != null)
            {
                Destroy(targetObject);
                targetObject = null;
                purplePage.GetComponent<Image>().enabled = false;
                purpleText.enabled = false;
            }
            return;
        }

        #region 움직이는(move) 코드

        if (temporalMoveCoolTime > 0f)
        {
            temporalMoveCoolTime -= Time.deltaTime;
            if ((Input.GetKeyUp(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space)) ||
                (Input.GetKeyUp(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift)))
            {
                temporalMoveCoolTime = 0f;
            }
        }

        // 키보드의 A, D, W, S, 좌Shift, Space 키로부터 입력을 받습니다.
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        if (!(SceneManager.GetActiveScene().name.Equals("Tutorial") && GetComponent<TutorialManager>().Phase <= 0) &&
            !Manager.instance.GetGameOver())
        {
            if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space) && temporalMoveCoolTime <= 0f)
            {
                r.position = new Vector3(r.position.x, r.position.y, r.position.z - Boundary.OnePageToDeltaZ());
                temporalMoveCoolTime = 1f / temporalSpeed;
            }
            else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Space) && temporalMoveCoolTime <= 0f)
            {
                r.position = new Vector3(r.position.x, r.position.y, r.position.z + Boundary.OnePageToDeltaZ());
                temporalMoveCoolTime = 1f / temporalSpeed;
            }
            if (temporalMoveCoolTime > 0f)
            {
                moveHorizontal = 0f;
                moveVertical = 0f;
            }
        }

        zLocation.GetComponentInChildren<Text>().text = "" + Boundary.ZToPage(r.position.z);

        // 플레이어를 움직입니다.
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0f);
        if (!Manager.instance.GetGameOver())
        {
            r.velocity = movement.normalized * speed;
        }
        else
        {
            r.velocity = Vector3.zero;
        }

        // 플레이어가 화면 밖으로 나갈 수 없게 합니다.
        r.position = new Vector3
        (
            Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
            Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
            Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
        );

        #endregion

        if (Manager.instance.GetGameOver())
        {
            if (targetObject != null)
            {
                Destroy(targetObject);
                targetObject = null;
                purplePage.GetComponent<Image>().enabled = false;
                purpleText.enabled = false;
            }
            return;
        }
        if (Manager.instance.IsPaused) return;

        #region 던지는(shoot) 코드

        if (!(SceneManager.GetActiveScene().name.Equals("Tutorial") && GetComponent<TutorialManager>().Phase <= 1))
        {
            if (!Input.GetMouseButton(1) && Input.GetMouseButton(0))
            {
                Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9) && hit.collider.gameObject.tag.Equals("Present"))
                {
                    //Debug.DrawLine(ray.origin, hit.point, Color.yellow, 3f);
                    if (targetObject == null)
                    {
                        targetObject = Instantiate(target, hit.point, Quaternion.identity);
                        targetObject.GetComponentInChildren<ChargeUI>().SetRectTransform();
                        targetObject.GetComponentInChildren<ChargeUI>().SetVisible();
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
                            chargedZ -= Time.deltaTime * chargeSpeed;
                        else if (v < 0)
                            chargedZ = Boundary.zMin - GetComponent<Transform>().position.z;
                        else
                            chargedZ = Boundary.zMax - GetComponent<Transform>().position.z;
                        /*
                        targetObject.GetComponentInChildren<Text>().text = "과거로 ";
                        targetObject.GetComponentInChildren<Text>().text += (int)(Mathf.Abs(chargedZ)) + "." + (int)(Mathf.Abs(chargedZ) * 100) % 100;
                        */
                        targetObject.GetComponentInChildren<ChargeClockUI>().NewChargedZ = Boundary.RoundZ(chargedZ);
                        /* 
                            * Vector2.Distance(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y),
                            new Vector2(hit.point.x, hit.point.y));
                        */
                        purplePage.anchoredPosition = new Vector2(Mathf.Lerp(-240f, 240f,
                            ((Boundary.ZToPage(GetComponent<Transform>().position.z + Boundary.RoundZ(chargedZ)) - Boundary.pageBase)
                            / (float)Boundary.pageNum)), purplePage.anchoredPosition.y);
                        purpleText.text = Boundary.ZToPage(GetComponent<Transform>().position.z + Boundary.RoundZ(chargedZ)).ToString();
                    }
                }
            }
            else if (!Input.GetMouseButton(0) && Input.GetMouseButton(1))
            {
                Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9) && hit.collider.gameObject.tag.Equals("Present"))
                {
                    //Debug.DrawLine(ray.origin, hit.point, Color.yellow, 3f);
                    if (targetObject == null)
                    {
                        targetObject = Instantiate(target, hit.point, Quaternion.identity);
                        targetObject.GetComponentInChildren<ChargeUI>().SetRectTransform();
                        targetObject.GetComponentInChildren<ChargeUI>().SetVisible();
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
                            chargedZ += Time.deltaTime * chargeSpeed;
                        else if (v < 0)
                            chargedZ = Boundary.zMin - GetComponent<Transform>().position.z;
                        else
                            chargedZ = Boundary.zMax - GetComponent<Transform>().position.z;

                        /*
                        targetObject.GetComponentInChildren<Text>().text = "미래로 ";
                        targetObject.GetComponentInChildren<Text>().text += (int)(Mathf.Abs(chargedZ)) + "." + (int)(Mathf.Abs(chargedZ) * 100) % 100;
                        */
                        targetObject.GetComponentInChildren<ChargeClockUI>().NewChargedZ = Boundary.RoundZ(chargedZ);
                        /*
                            * Vector2.Distance(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y),
                            new Vector2(hit.point.x, hit.point.y));
                        */
                        purplePage.anchoredPosition = new Vector2(Mathf.Lerp(-240f, 240f,
                            ((Boundary.ZToPage(GetComponent<Transform>().position.z + Boundary.RoundZ(chargedZ)) - Boundary.pageBase)
                            / (float)Boundary.pageNum)), purplePage.anchoredPosition.y);
                        purpleText.text = Boundary.ZToPage(GetComponent<Transform>().position.z + Boundary.RoundZ(chargedZ)).ToString();
                    }
                }
            }
            else if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
            {
                Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9) && hit.collider.gameObject.tag.Equals("Present"))
                {
                    //Debug.DrawLine(ray.origin, hit.point, Color.yellow, 3f);
                    if (targetObject == null)
                    {
                        targetObject = Instantiate(target, hit.point, Quaternion.identity);
                        targetObject.GetComponentInChildren<ChargeUI>().SetRectTransform();
                        targetObject.GetComponentInChildren<ChargeUI>().SetVisible();
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
                        if (v < 0)
                            chargedZ = Boundary.zMin - GetComponent<Transform>().position.z;
                        else if (v > 0)
                            chargedZ = Boundary.zMax - GetComponent<Transform>().position.z;

                        targetObject.GetComponentInChildren<ChargeClockUI>().NewChargedZ = Boundary.RoundZ(chargedZ);
                        /*
                            * Vector2.Distance(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y),
                            new Vector2(hit.point.x, hit.point.y));
                        */
                        purplePage.anchoredPosition = new Vector2(Mathf.Lerp(-240f, 240f,
                            ((Boundary.ZToPage(GetComponent<Transform>().position.z + Boundary.RoundZ(chargedZ)) - Boundary.pageBase)
                            / (float)Boundary.pageNum)), purplePage.anchoredPosition.y);
                        purpleText.text = Boundary.ZToPage(GetComponent<Transform>().position.z + Boundary.RoundZ(chargedZ)).ToString();
                    }
                }
            }

            if ((Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1)) || (Input.GetMouseButtonUp(1) && !Input.GetMouseButton(0)))
            {
                Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9) && hit.collider.gameObject.tag.Equals("Present"))
                {
                    /*
                    chargedZ *= Vector2.Distance(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y),
                            new Vector2(hit.point.x, hit.point.y));
                    */
                    GameObject k = Instantiate(knife, GetComponent<Transform>().position, Quaternion.identity);
                    k.GetComponent<Knife>().Initialize(0, new Vector3(
                        ray.origin.x + ray.direction.x * (chargedZ + GetComponent<Transform>().position.z - ray.origin.z) / ray.direction.z,
                        ray.origin.y + ray.direction.y * (chargedZ + GetComponent<Transform>().position.z - ray.origin.z) / ray.direction.z,
                        GetComponent<Transform>().position.z + Boundary.RoundZ(chargedZ)));
                    Destroy(targetObject);
                    targetObject = null;
                    purplePage.GetComponent<Image>().enabled = false;
                    purpleText.enabled = false;
                    chargedZ = 0f;
                }
            }
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
    }

    public void Damaged()
    {
        if (Manager.instance.GetGameOver()) return;

        if (Health > 0 && invincibleTime <= 0f)
        {
            Debug.LogWarning("Player hit!");
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

    
}
