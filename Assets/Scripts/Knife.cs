using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Knife : MonoBehaviour {

    private int owner = -1;     // 칼을 던진 플레이어가 누구인지 (0: 휴먼, 1: 컴퓨터)
    private Vector3 start;      // 출발지입니다.
    private Vector3 dest;       // 목적지입니다. 날아갈 방향을 결정합니다.

    private Transform t;
    private Transform player;
    private Transform enemy;

    private bool isCracked;

    public float speed;

    public GameObject flare;

    // 칼이 생성될 때 자동으로, 한 번만 호출됩니다.
    private void Awake()
    {
        t = GetComponent<Transform>();  // 이 칼의 위치를 갖고 있습니다.
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
        start = t.position;
        isCracked = false;

        if (Mathf.Abs(player.position.z - t.position.z) > 1f)
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = false;
            }
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
            GetComponent<MeshRenderer>().material.color = new Color(0.3f, 0.3f, 0.3f, alpha);
            if (!isCracked && Mathf.Abs(otherZ - t.position.z) < 0.02f)
            {
                Instantiate(flare, t.position, Quaternion.identity);
                isCracked = true;
            }
        }
        else if (Mathf.Abs(otherZ - t.position.z) < 0.15f)
        {
            GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 0f, alpha);
        }
        else if (t.position.z < player.position.z)
        {
            GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 1f, alpha);
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f, alpha);
        }

        if (Mathf.Abs(t.position.z) > Boundary.zMax + 1f || Mathf.Abs(t.position.x) > Boundary.xMax + 1f || Mathf.Abs(t.position.y) > Boundary.yMax + 1f)
        {
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
            Destroy(gameObject);
        }
    }
}
