using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeClockUI : ChargeUI {

    private Camera mainCamera;
    private Transform player;
    private Transform enemy;
    private float chargedZ = 0f;

    public float NewChargedZ
    {
        set
        {
            chargedZ = value;
        }
    }

    public Transform redHand;
    public Transform purpleHand;
    public Text purpleText;

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

    public override void SetRectTransform()
    {
        GetComponent<RectTransform>().position = mainCamera.WorldToScreenPoint(GetComponentInParent<MeshRenderer>().GetComponent<Transform>().position);
        if (player.GetComponent<Player>().Health > 0)
        {
            float deg = Mathf.Lerp(150f, -150f, ((player.position.z + Boundary.RoundZ(chargedZ)) - Boundary.zMin) / (Boundary.zMax - Boundary.zMin));
            purpleHand.SetPositionAndRotation(GetComponent<RectTransform>().position, Quaternion.Euler(0f, 0f, deg));
            purpleText.text = Boundary.ZToPage(player.position.z + Boundary.RoundZ(chargedZ)).ToString();
        }
        if (enemy.GetComponent<Enemy>().Health > 0)
        {
            float deg = Mathf.Lerp(150f, -150f, (enemy.position.z - Boundary.zMin) / (Boundary.zMax - Boundary.zMin));
            redHand.SetPositionAndRotation(GetComponent<RectTransform>().position, Quaternion.Euler(0f, 0f, deg));
            if (Boundary.ZToPage(player.position.z + Boundary.RoundZ(chargedZ)) - Boundary.ZToPage(enemy.position.z) == 0)
            {
                Color c = new Color(redHand.GetComponent<Image>().color.r, redHand.GetComponent<Image>().color.g, redHand.GetComponent<Image>().color.b, GetComponent<Image>().color.a);
                GetComponent<Image>().color = c;
            }
            else
            {
                GetComponent<Image>().color = new Color(0f, 0f, 0f, GetComponent<Image>().color.a);
            }
        }
    }

    public override void SetVisible()
    {
        GetComponent<Image>().enabled = true;
        purpleHand.GetComponent<Image>().enabled = true;
        redHand.GetComponent<Image>().enabled = true;
        purpleText.enabled = true;
    }
}
