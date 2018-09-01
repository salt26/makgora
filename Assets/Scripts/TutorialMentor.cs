using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMentor : MonoBehaviour {

    public GameObject pageText;

    private Vector3 destPosition;
    private float temporalMoveCoolTime = 0f;
    private bool startMoving = false, endMoving = false;
    private bool isArrived = false; // 1페이즈에서 플레이어가 멘토에게 닿으면 true가 됩니다.
    private bool isPageVisible = false; // 2페이즈가 시작될 때 true가 됩니다.
    private Player player;
    private GameObject myText;
    private Camera mainCamera;

    public bool StartMoving
    {
        get
        {
            return startMoving;
        }
    }

    public bool EndMoving
    {
        get
        {
            return endMoving;
        }
    }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        Vanish();

        if (Manager.instance.IsPaused) return;

        if (startMoving && !endMoving &&
            Vector3.Distance(GetComponent<Transform>().position, destPosition) < Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지에 도착했습니다.
            endMoving = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else if (startMoving && !endMoving && temporalMoveCoolTime > 0f)
        {
            temporalMoveCoolTime -= Time.fixedDeltaTime;
        }
        else if (startMoving && !endMoving &&
            Mathf.Abs(GetComponent<Transform>().position.z - destPosition.z) > Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지를 향해 시간 축을 따라 이동합니다.
            Rigidbody r = GetComponent<Rigidbody>();
            r.velocity = Vector3.zero;
            float deltaZ;
            if (GetComponent<Transform>().position.z > destPosition.z) deltaZ = -Boundary.OnePageToDeltaZ();
            else deltaZ = Boundary.OnePageToDeltaZ();
            temporalMoveCoolTime = 1f / Manager.instance.TemporalSpeed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z + deltaZ, Boundary.zMin, Boundary.zMax)
            );
        }
        else if (startMoving && !endMoving)
        {
            // 목적지를 향해 XY평면을 따라 이동합니다.
            Rigidbody r = GetComponent<Rigidbody>();
            Vector3 movement = destPosition - GetComponent<Transform>().position;
            movement.z = 0f;
            r.velocity = movement.normalized * Manager.instance.MovingSpeed * 1.5f;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
            );
        }
    }

    /// <summary>
    /// 시간대가 다를 때 투명해지도록 하는 함수입니다.
    /// </summary>
    private void Vanish()
    {
        float alpha = Mathf.Max(0f, 1f - Mathf.Pow(Mathf.Abs(player.GetComponent<Transform>().position.z - GetComponent<Transform>().position.z) / Boundary.sight, 2));

        if (Manager.instance.IsPaused && myText != null)
        {
            myText.GetComponent<Text>().enabled = false;
        }
        else if (!Manager.instance.IsPaused)
        {
            if (1 - (Mathf.Abs(player.GetComponent<Transform>().position.z - GetComponent<Transform>().position.z) / Boundary.sight) < 0)
            {
                foreach (SpriteRenderer mr in GetComponentsInChildren<SpriteRenderer>())
                {
                    mr.enabled = false;
                }
                
                if (myText != null) myText.GetComponent<Text>().enabled = false;
            }
            else if (Mathf.Abs(player.GetComponent<Transform>().position.z - GetComponent<Transform>().position.z) < Boundary.OnePageToDeltaZ() * Boundary.approach)
            {
                foreach (SpriteRenderer mr in GetComponentsInChildren<SpriteRenderer>())
                {
                    mr.enabled = true;
                }
                //Material m = GetComponentInChildren<CharacterModel>().GetComponent<MeshRenderer>().material;
                SpriteRenderer m = GetComponent<SpriteRenderer>();
                m.color = ColorUtil.instance.AlphaColor(ColorUtil.instance.presentMentorColor, alpha);

                if (myText != null && !Manager.instance.IsPaused) myText.GetComponent<Text>().enabled = true;
                TextMover();
            }
            else if (GetComponent<Transform>().position.z < player.GetComponent<Transform>().position.z)
            {
                foreach (SpriteRenderer mr in GetComponentsInChildren<SpriteRenderer>())
                {
                    mr.enabled = true;
                }
                //Material m = GetComponentInChildren<CharacterModel>().GetComponent<MeshRenderer>().material;
                SpriteRenderer m = GetComponent<SpriteRenderer>();
                m.color =
                    ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.pastColor, ColorUtil.instance.pastPastColor,
                    Mathf.Abs(player.GetComponent<Transform>().position.z - GetComponent<Transform>().position.z) - Boundary.OnePageToDeltaZ() * Boundary.approach), alpha);

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
                SpriteRenderer m = GetComponent<SpriteRenderer>();
                m.color =
                    ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.futureColor, ColorUtil.instance.futureFutureColor,
                    Mathf.Abs(player.GetComponent<Transform>().position.z - GetComponent<Transform>().position.z) - Boundary.OnePageToDeltaZ() * Boundary.approach), alpha);

                if (myText != null && !Manager.instance.IsPaused) myText.GetComponent<Text>().enabled = true;
                TextMover();
            }
        }
    }

    private void TextMover()
    {
        if (!isPageVisible) return;
        Vector3 v = mainCamera.WorldToScreenPoint(GetComponent<Transform>().position);
        v.y += 100f;
        if (myText != null)
        {
            myText.GetComponent<Transform>().position = v;
        }
        else
        {
            myText = Instantiate(pageText, v, Quaternion.identity, Manager.instance.Canvas.GetComponent<Transform>());
        }
        myText.GetComponent<Text>().text = Boundary.ZToPage(GetComponent<Transform>().position.z).ToString();

        myText.GetComponent<Text>().color = GetComponent<SpriteRenderer>().color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && other.GetComponent<TutorialManager>().Phase == 1 && !isArrived)
        {
            isArrived = true;
            other.GetComponent<TutorialManager>().NextProcess();
            //Destroy(gameObject);
        }
    }

    public void SetMoving(Vector3 dest)
    {
        destPosition = dest;
        startMoving = true;
        endMoving = false;
    }

    public void SetStop()
    {
        if (startMoving && endMoving)
        {
            startMoving = false;
            endMoving = false;
        }
    }

    public void SetPageVisible()
    {
        isPageVisible = true;
    }
}
