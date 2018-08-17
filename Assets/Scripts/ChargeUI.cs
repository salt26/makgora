using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUI : MonoBehaviour {

    private Camera mainCamera;
    private Transform player;
    private Transform enemy;
    private float chargedZ = 0f;

    public float ChargedZ
    {
        set
        {
            chargedZ = value;
        }
    }

    public Transform redHand;
    public Transform blueHand;

    private void Awake()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
    }

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
        SetRectTransform();
    }

    public void SetRectTransform()
    {
        GetComponent<RectTransform>().position = mainCamera.WorldToScreenPoint(GetComponentInParent<MeshRenderer>().GetComponent<Transform>().position);
        if (player.GetComponent<Player>().Health > 0)
        {
            blueHand.SetPositionAndRotation(GetComponent<RectTransform>().position,
                Quaternion.Euler(0f, 0f, 90f - 18f * (player.position.z + Boundary.RoundZ(chargedZ))));
        }
        if (enemy.GetComponent<Enemy>().Health > 0)
        {
            redHand.SetPositionAndRotation(GetComponent<RectTransform>().position,
                Quaternion.Euler(0f, 0f, 90f - 18f * enemy.position.z));
        }
    }

    public void SetVisible()
    {
        GetComponent<Image>().enabled = true;
        blueHand.GetComponent<Image>().enabled = true;
        redHand.GetComponent<Image>().enabled = true;
    }
}
