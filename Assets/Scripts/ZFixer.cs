using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZFixer : MonoBehaviour {

    public GameObject player;
    private float initZ;

    private void Awake()
    {
        initZ = GetComponent<Rigidbody>().position.z;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            GetComponent<Rigidbody>().MovePosition(
                new Vector3(
                GetComponent<Rigidbody>().position.x,
                GetComponent<Rigidbody>().position.y,
                initZ + player.GetComponent<Rigidbody>().position.z));
        }
    }
}
