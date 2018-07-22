using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMover : MonoBehaviour
{

    public GameObject player;
    private float chargedZ;

    public float ChargedZ
    {
        set
        {
            chargedZ = value;
        }
    }

    private void Awake()
    {
        chargedZ = 0f;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            GetComponent<Transform>().position = new Vector3(
                GetComponent<Transform>().position.x,
                GetComponent<Transform>().position.y,
                chargedZ + player.GetComponent<Transform>().position.z);
        }
    }
}
