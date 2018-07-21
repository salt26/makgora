using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour {

    public GameObject player;

	void FixedUpdate () {
        float z = player.GetComponent<Transform>().position.z;
        if (z < 0)
        {
            GetComponent<Text>().text = "-" + (int)(Mathf.Abs(z)) + "." + (int)(Mathf.Abs(z) * 1000) % 1000;
        }
        else
        {
            GetComponent<Text>().text = (int)(Mathf.Abs(z)) + "." + (int)(Mathf.Abs(z) * 1000) % 1000;
        }
    }
}
