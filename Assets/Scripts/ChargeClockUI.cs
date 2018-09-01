using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeClockUI : MonoBehaviour {

    private Camera mainCamera;
    private Transform player;
    private Transform enemy;
    private float chargedZ = 0f;
    private float prepareTime = 0f;
    private bool isColored = false;

    public float ChargedZ
    {
        set
        {
            chargedZ = value;
        }
    }

    public float PrepareTime
    {
        set
        {
            prepareTime = value;
        }
    }

    public Transform redHand;
    public Transform purpleHand;
    public RectTransform background; 
    public Text purpleText;

    private void Awake()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
    }

    void FixedUpdate ()
    {
        SetRectTransform();
    }

    public void SetRectTransform()
    {
        isColored = false;
        GetComponent<RectTransform>().position = mainCamera.WorldToScreenPoint(GetComponentInParent<MeshRenderer>().GetComponent<Transform>().position);
        if (player.GetComponent<Player>().Health > 0)
        {
            float deg = Mathf.Lerp(150f, -150f, ((player.position.z + Boundary.RoundZ(chargedZ)) - Boundary.zMin) / (Boundary.zMax - Boundary.zMin));
            purpleHand.SetPositionAndRotation(GetComponent<RectTransform>().position, Quaternion.Euler(0f, 0f, deg));
            purpleText.text = Boundary.ZToPage(player.position.z + Boundary.RoundZ(chargedZ)).ToString();
            background.position = GetComponent<RectTransform>().position;
        }
        else
        {
            purpleHand.GetComponent<Image>().enabled = false;
            purpleText.enabled = false;
        }

        if (enemy.GetComponent<Enemy>().Health > 0 &&
            !(Manager.instance.GetCurrentGame()[0].Equals("Sniping") && enemy.GetComponent<Enemy>().Health == 1))
        {
            float deg = Mathf.Lerp(150f, -150f, (enemy.position.z - Boundary.zMin) / (Boundary.zMax - Boundary.zMin));
            redHand.SetPositionAndRotation(GetComponent<RectTransform>().position, Quaternion.Euler(0f, 0f, deg));
            if (prepareTime >= Manager.instance.PrepareChargeTime &&
                Boundary.ZToPage(player.position.z + Boundary.RoundZ(chargedZ)) - Boundary.ZToPage(enemy.position.z) == 0)
            {
                Color c = new Color(redHand.GetComponent<Image>().color.r, redHand.GetComponent<Image>().color.g, redHand.GetComponent<Image>().color.b, GetComponent<Image>().color.a);
                GetComponent<Image>().color = c;
                isColored = true;
            }
        }
        else
        {
            redHand.GetComponent<Image>().enabled = false;
        }

        if (prepareTime < Manager.instance.PrepareChargeTime)
        {
            background.sizeDelta = Vector2.Lerp(new Vector2(100f, 100f), Vector2.zero, prepareTime / Manager.instance.PrepareChargeTime);
            GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, GetComponent<Image>().color.a);
            isColored = true;
        }
        if (!isColored)
        {
            GetComponent<Image>().color = new Color(0f, 0f, 0f, GetComponent<Image>().color.a);
        }
    }

    public void SetVisible()
    {
        GetComponent<Image>().enabled = true;
        purpleHand.GetComponent<Image>().enabled = true;
        redHand.GetComponent<Image>().enabled = true;
        purpleText.enabled = true;
        background.GetComponent<Image>().enabled = true;
    }
}
