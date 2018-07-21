using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[System.Serializable]
public static class Boundary
{
    public static float xMin = -1.19f, xMax = 1.19f, yMin = -0.88f, yMax = 0.88f, zMin = -10f, zMax = 10f;
}

public class Player : MonoBehaviour {

    public float speed;
    public float chargeSpeed;           // 마우스 클릭 시 Z좌표가 증가(감소)하는 속도입니다.
    public GameObject target;           // 마우스 클릭 지점 프리팹입니다.
    public GameObject knife;            // 칼 프리팹입니다.

    private int health = 3;
    private GameObject targetObject;    // 현재 화면에 나타난 마우스 클릭 지점 오브젝트를 관리합니다.
    private float chargedZ;             // 칼을 발사할 목적지 방향의 Z좌표(시간축 좌표)입니다.
    private Rigidbody r;

    private void Awake()
    {
        r = GetComponent<Rigidbody>();
        chargedZ = 0f;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (health <= 0) return;

        // 키보드의 A, D, W, S, 좌Shift, Space 키로부터 입력을 받습니다.
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        float moveTemporal = Input.GetAxisRaw("Temporal");

        // 플레이어를 움직입니다.
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, moveTemporal);
        r.velocity = movement.normalized * speed;

        // 플레이어가 화면 밖으로 나갈 수 없게 합니다.
        r.position = new Vector3
        (
            Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
            Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
            Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
        );

        if (!Input.GetMouseButton(1) && Input.GetMouseButton(0))
        {
            Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<9) && hit.collider.gameObject.tag.Equals("Present"))
            {
                //Debug.DrawLine(ray.origin, hit.point, Color.yellow, 3f);
                if (targetObject == null) targetObject = Instantiate(target, hit.point, Quaternion.identity);
                else targetObject.GetComponent<Transform>().SetPositionAndRotation(hit.point, Quaternion.identity);

                if (targetObject != null) {
                    chargedZ -= Time.deltaTime * chargeSpeed
                        * Vector2.Distance(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y),
                        new Vector2(hit.point.x, hit.point.y));
                    targetObject.GetComponentInChildren<Text>().text = "-";
                    targetObject.GetComponentInChildren<Text>().text += (int)(Mathf.Abs(chargedZ)) + "." + (int)(Mathf.Abs(chargedZ) * 100) % 100;
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
                if (targetObject == null) targetObject = Instantiate(target, hit.point, Quaternion.identity);
                else targetObject.GetComponent<Transform>().SetPositionAndRotation(hit.point, Quaternion.identity);

                if (targetObject != null)
                {
                    chargedZ += Time.deltaTime * chargeSpeed
                        * Vector2.Distance(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y),
                        new Vector2(hit.point.x, hit.point.y));
                    targetObject.GetComponentInChildren<Text>().text = "+";
                    targetObject.GetComponentInChildren<Text>().text += (int)(Mathf.Abs(chargedZ)) + "." + (int)(Mathf.Abs(chargedZ) * 100) % 100;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9) && hit.collider.gameObject.tag.Equals("Present"))
            {
                GameObject k = Instantiate(knife, GetComponent<Transform>().position, Quaternion.identity);
                k.GetComponent<Knife>().Initialize(0, new Vector3(hit.point.x, hit.point.y, GetComponent<Transform>().position.z + chargedZ));
                Destroy(targetObject);
                targetObject = null;
                chargedZ = 0f;
            }
        }
    }

    public void Damaged()
    {
        if (health > 0)
            health--;
        if (health <= 0 && GetComponent<MeshRenderer>().enabled)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
