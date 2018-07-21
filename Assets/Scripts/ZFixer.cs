using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZFixer : MonoBehaviour {

    public GameObject player;
    private float initZ;

    private void Awake()
    {
        initZ = GetComponent<Transform>().position.z;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            GetComponent<Transform>().position = new Vector3(
                GetComponent<Transform>().position.x,
                GetComponent<Transform>().position.y,
                initZ + player.GetComponent<Transform>().position.z);
        }
    }
}
