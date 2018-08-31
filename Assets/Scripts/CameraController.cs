using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private Player player;
    private float initZ;

    private void Awake()
    {
        initZ = GetComponent<Rigidbody>().position.z;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            if (player.TemporalMoveCoolTime <= 0.5f / Manager.instance.TemporalSpeed)
            {
                GetComponent<Rigidbody>().MovePosition(
                    new Vector3(
                    GetComponent<Rigidbody>().position.x,
                    GetComponent<Rigidbody>().position.y,
                    initZ + player.GetComponent<Rigidbody>().position.z));
            }
            else
            {
                float z = Mathf.Lerp(initZ + player.GetComponent<Rigidbody>().position.z, GetComponent<Rigidbody>().position.z,
                    (player.TemporalMoveCoolTime - (0.5f / Manager.instance.TemporalSpeed)) / (0.5f / Manager.instance.TemporalSpeed));
                GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f,
                    Manager.instance.TemporalSpeed * Mathf.Sign((initZ + player.GetComponent<Rigidbody>().position.z) - GetComponent<Rigidbody>().position.z));
            }
        }
    }
}
