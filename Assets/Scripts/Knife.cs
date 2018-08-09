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

    public float speed;

    // 칼이 생성될 때 자동으로, 한 번만 호출됩니다.
    private void Awake()
    {
        t = GetComponent<Transform>();  // 이 칼의 위치를 갖고 있습니다.
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
        start = t.position;
    }

    // 매 프레임마다 자동으로 호출됩니다.
    void FixedUpdate () {
        Vector3 direction = new Vector3();
        float otherZ = 0f;
		if (owner != -1)
        {
            direction = (dest - start).normalized * speed;  // 속력은 항상 speed만큼입니다.
            t.SetPositionAndRotation(t.position + direction * Time.fixedDeltaTime, Quaternion.Euler(Quaternion.LookRotation(direction).eulerAngles/* + new Vector3(-90f, 0f, 90f)*/));
        }

        float alpha = 1f;   // 대상을 지나 사라질 때 투명화됨
        if (owner == 0) otherZ = enemy.position.z;
        else if (owner == 1) otherZ = player.position.z;

        if (direction.z != 0f && (otherZ - t.position.z) * direction.z < 0)
        {
            alpha = Mathf.Pow(Mathf.Abs(otherZ - t.position.z) - 1, 2);
        }

        if (player.position.z - t.position.z < 0f)
        {
            // 플레이어 캐릭터와의 Z좌표(시간축 좌표) 차이에 따라 투명도를 적용합니다.
            GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, Mathf.Pow(Mathf.Abs(
                        player.position.z - t.position.z) - 1, 2), alpha);
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = new Color(Mathf.Pow(Mathf.Abs(
                        player.position.z - t.position.z) - 1, 2), 0f, 0f, alpha);
        }

        if (Mathf.Abs(t.position.z) > 6f || Mathf.Abs(t.position.x) > 2.6f || Mathf.Abs(t.position.y) > 2f)
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
            other.GetComponent<Enemy>().Damaged();
            Destroy(gameObject);
        }
    }
}
