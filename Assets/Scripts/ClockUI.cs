using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour {

    public Transform player;
    public Transform enemy;

    public Transform greenHand;
    public Transform redHand;

	void FixedUpdate () {
        /*
        float pz = player.GetComponent<Transform>().position.z + Time.fixedTime;
        float ez = enemy.GetComponent<Transform>().position.z + Time.fixedTime;
        GetComponent<Text>().text = "자신의 시간: ";
        if (pz < 0)
        {
            GetComponent<Text>().text += "-" + (int)(Mathf.Abs(pz)) + "." + (int)(Mathf.Abs(pz) * 1000) % 1000;
        }
        else
        {
            GetComponent<Text>().text += (int)(Mathf.Abs(pz)) + "." + (int)(Mathf.Abs(pz) * 1000) % 1000;
        }
        GetComponent<Text>().text += "\n상대의 시간: ";
        if (ez < 0)
        {
            GetComponent<Text>().text += "-" + (int)(Mathf.Abs(ez)) + "." + (int)(Mathf.Abs(ez) * 1000) % 1000;
        }
        else
        {
            GetComponent<Text>().text += (int)(Mathf.Abs(ez)) + "." + (int)(Mathf.Abs(ez) * 1000) % 1000;
        }
        */
        if (player.GetComponent<Player>().Health > 0)
        {
            greenHand.SetPositionAndRotation(greenHand.position, Quaternion.Euler(0f, 0f, -6f * Time.fixedTime - 18f * player.position.z));
        }
        if (enemy.GetComponent<Enemy>().Health > 0)
        {
            redHand.SetPositionAndRotation(redHand.position, Quaternion.Euler(0f, 0f, -6f * Time.fixedTime - 18f * enemy.position.z));
        }
    }
}
