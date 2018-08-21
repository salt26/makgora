using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Knife : MonoBehaviour {

    private int owner = -1;     // 칼을 던진 플레이어가 누구인지 (0: 휴먼, 1: 컴퓨터)
    private Vector3 start;      // 출발지입니다.
    private Vector3 dest;       // 목적지입니다. 날아갈 방향을 결정합니다.

    private Transform t;
    private Transform player;
    private Transform enemy;
    private GameObject text;
    private Camera mainCamera;

    private bool isCracked;

    private float speed;

    public GameObject flare;
    public GameObject knifeText;

    // 칼이 생성될 때 자동으로, 한 번만 호출됩니다.
    private void Awake()
    {
        t = GetComponent<Transform>();  // 이 칼의 위치를 갖고 있습니다.
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        start = t.position;
        isCracked = false;
        speed = Manager.instance.KnifeSpeed;

        if (Mathf.Abs(player.position.z - t.position.z) > 1f)
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = false;
            }
        }
        else
        {
            TextMover();
        }
    }

    // 매 프레임마다 자동으로 호출됩니다.
    void FixedUpdate () {
        Vector3 direction = new Vector3();
        float ownZ = 0f;
        float otherZ = 0f;
		if (owner != -1)
        {
            direction = (dest - start).normalized * speed;  // 속력은 항상 speed만큼입니다.
            t.SetPositionAndRotation(t.position + direction * Time.fixedDeltaTime,
                Quaternion.Euler(Quaternion.LookRotation(direction).eulerAngles/* + new Vector3(-90f, 0f, 90f)*/));
        }

        float alpha = 1f;   // 대상을 지나 사라질 때 투명화됨
        if (owner == 0)
        {
            ownZ = player.position.z;
            otherZ = enemy.position.z;
        }
        else if (owner == 1)
        {
            ownZ = enemy.position.z;
            otherZ = player.position.z;
        }

        if (Mathf.Abs(player.position.z - t.position.z) > 1f)
        {
            alpha = 0f;
            if (text != null) text.GetComponent<Text>().enabled = false;
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = false;
            }
        }
        else
        {
            if (text != null) text.GetComponent<Text>().enabled = true;
            TextMover();
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = true;
            }
            
            if (direction.z != 0f && (otherZ - t.position.z) * direction.z < 0)
            {
                // 상대방 위치의 반대 방향으로 총알을 쏘면 자신과의 Z좌표(시간축 좌표) 차이에 따라 투명도를 적용합니다.
                if ((ownZ - otherZ) * (ownZ - t.position.z) < 0)
                {
                    alpha = Mathf.Pow(Mathf.Abs(ownZ - t.position.z) - 1, 2);
                }
                // 그 외의 경우 대상 캐릭터와의 Z좌표 차이에 따라 투명도를 적용합니다.
                else
                {
                    alpha = Mathf.Pow(Mathf.Abs(otherZ - t.position.z) - 1, 2);
                }

            }
        }

        if (owner == 0)
        {
            if (!isCracked && Mathf.Abs(otherZ - t.position.z) < 0.02f)
            {
                Instantiate(flare, t.position, Quaternion.identity);
                isCracked = true;
            }
        }

        if (Mathf.Abs(player.position.z - t.position.z) < Boundary.OnePageToDeltaZ() * 0.8f)
        {
            if (owner == 0)
                GetComponent<MeshRenderer>().material.color = ColorUtil.instance.AlphaColor(ColorUtil.instance.presentPlayerColor, alpha);
            else
                GetComponent<MeshRenderer>().material.color = ColorUtil.instance.AlphaColor(ColorUtil.instance.presentEnemyColor, alpha);
        }
        else if (t.position.z < player.position.z)
        {
            GetComponent<MeshRenderer>().material.color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.pastColor, ColorUtil.instance.pastPastColor, 
                Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * 0.8f), alpha);
        }
        else
        {
            GetComponent<MeshRenderer>().material.color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.futureColor, ColorUtil.instance.futureFutureColor,
                Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * 0.8f), alpha);
        }

        if (Mathf.Abs(t.position.z) > Boundary.zMax + 1f || Mathf.Abs(t.position.x) > Boundary.xMax + 1f || Mathf.Abs(t.position.y) > Boundary.yMax + 1f)
        {
            Destroy(text);
            Destroy(gameObject);
        }

    }
    
    /// <summary>
    /// 칼을 날리기 위해 외부에서 호출해줘야 하는 함수입니다.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="destination"></param>
    public void Initialize(int owner, Vector3 destination)
    {
        dest = destination;
        if (owner == 0 || owner == 1)
            this.owner = owner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && owner == 1 && other.GetComponent<Player>().Health > 0)
        {
            other.GetComponent<Player>().Damaged();
            Destroy(text);
            Destroy(gameObject);
        }
        else if (other.tag.Equals("Enemy") && owner == 0 && other.GetComponent<Enemy>().Health > 0)
        {
            if (!isCracked)
            {
                Instantiate(flare, t.position, Quaternion.identity);
                isCracked = true;
            }
            other.GetComponent<Enemy>().damaged();
            Destroy(text);
            Destroy(gameObject);
        }
    }

    private void TextMover()
    {
        Vector3 v = mainCamera.WorldToScreenPoint(t.position);
        v.y += 12f;
        if (text != null)
        {
            text.GetComponent<Transform>().position = v;
        }
        else
        {
            text = Instantiate(knifeText, v, Quaternion.identity, Manager.instance.Canvas.GetComponent<Transform>());
        }
        int pageDiff = Boundary.ZToPage(t.position.z) - Boundary.ZToPage(player.position.z);
        if (pageDiff > 0)
        {
            text.GetComponent<Text>().text = "+" + pageDiff.ToString();
        }
        else
        {
            text.GetComponent<Text>().text = pageDiff.ToString();
        }

        if (Mathf.Abs(player.position.z - t.position.z) < Boundary.OnePageToDeltaZ() * 0.8f)
        {
            if (owner == 0)
                text.GetComponent<Text>().color = ColorUtil.instance.AlphaColor(ColorUtil.instance.presentPlayerColor, 1f);
            else
                text.GetComponent<Text>().color = ColorUtil.instance.AlphaColor(ColorUtil.instance.presentEnemyColor, 1f);
        }
        else if (t.position.z < player.position.z)
        {
            text.GetComponent<Text>().color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.pastColor, ColorUtil.instance.pastPastColor,
                Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * 0.8f), 1f);
        }
        else
        {
            text.GetComponent<Text>().color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.futureColor, ColorUtil.instance.futureFutureColor,
                Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * 0.8f), 1f);
        }
    }
}
