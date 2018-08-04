using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Knife : MonoBehaviour {

    private int owner = -1;     // 칼을 던진 플레이어가 누구인지 (0: 휴먼, 1: 컴퓨터)
    private Vector3 start;      // 출발지입니다.
    private Vector3 dest;       // 목적지입니다. 날아갈 방향을 결정합니다.

    private Transform t;

    public float speed;

    // 칼이 생성될 때 자동으로, 한 번만 호출됩니다.
    private void Awake()
    {
        t = GetComponent<Transform>();  // 이 칼의 위치를 갖고 있습니다.
        start = t.position;
    }

    // 매 프레임마다 자동으로 호출됩니다.
    void FixedUpdate () {
		if (owner != -1)
        {
            Vector3 direction = (dest - start).normalized * speed;  // 속력은 항상 speed만큼입니다.
            t.SetPositionAndRotation(t.position + direction * Time.fixedDeltaTime, Quaternion.Euler(Quaternion.LookRotation(direction).eulerAngles + new Vector3(-90f, 0f, 90f)));
        }
        /*
        // 플레이어 캐릭터와의 Z좌표(시간축 좌표) 차이에 따라 투명도를 적용합니다.
        GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f,
            Mathf.Max(0, Mathf.Pow(Mathf.Abs(
                GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position.z - t.position.z) - 1, 2)));
        */
        if (Mathf.Abs(t.position.z) > 11f || Mathf.Abs(t.position.x) > 2.6f || Mathf.Abs(t.position.y) > 2f)
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
        else if (other.tag.Equals("Enemy") && owner == 0 && SceneManager.GetActiveScene().name.Equals("VagabondH") && other.GetComponent<Enemy>().Health > 0)
        {
            other.GetComponent<Enemy>().Damaged();
            Destroy(gameObject);
        }
        else if (other.tag.Equals("Enemy") && owner == 0 && SceneManager.GetActiveScene().name.Equals("Tutorial") && other.GetComponent<TutorialEnemy>().Health > 0)
        {
            other.GetComponent<TutorialEnemy>().Damaged();
            Destroy(gameObject);
        }
    }
}
