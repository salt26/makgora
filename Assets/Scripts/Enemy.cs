using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float speed;
    public GameObject knife;

    private int health = 3;
    private Vector3 dest;
    private bool isArrived = true;
    private Rigidbody r;
    private Transform t;
    
    void Awake () {
        r = GetComponent<Rigidbody>(); 
        t = GetComponent<Transform>();
	}
	
	void FixedUpdate () {
        if (health <= 0) return;

        GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 1f,
            Mathf.Max(0, Mathf.Pow(Mathf.Abs(
                GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position.z - t.position.z) - 1, 2)));

        if (!isArrived && Vector3.Distance(t.position, dest) < 0.01f)
        {
            isArrived = true;
        }
        else if (!isArrived)
        {
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
