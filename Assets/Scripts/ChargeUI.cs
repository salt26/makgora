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

    public RectTransform chargeCircle;
    public RectTransform enemyCircle;
    public Text pageText;
    public int minSize, maxSize;
    public Color black, red, darkPurple;

    private void Awake()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
    }

    void FixedUpdate () {
        SetRectTransform();
    }

    public void SetRectTransform()
    {
        GetComponent<RectTransform>().position = mainCamera.WorldToScreenPoint(GetComponentInParent<MeshRenderer>().GetComponent<Transform>().position);
        if (player.GetComponent<Player>().Health > 0)
        {
            /*
            blueHand.SetPositionAndRotation(GetComponent<RectTransform>().position,
                Quaternion.Euler(0f, 0f, -30f * (player.position.z + Boundary.RoundZ(chargedZ))));
                */
            chargeCircle.position = GetComponent<RectTransform>().position;
            chargeCircle.sizeDelta = CircleSize(Boundary.ZToPage(player.position.z + Boundary.RoundZ(chargedZ)));
            pageText.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
            pageText.text = Boundary.ZToPage(player.position.z + Boundary.RoundZ(chargedZ)).ToString();
        }
        if (enemy.GetComponent<Enemy>().Health > 0)
        {
            enemyCircle.position = GetComponent<RectTransform>().position;
            enemyCircle.sizeDelta = CircleSize(Boundary.ZToPage(enemy.position.z));
            if (Boundary.ZToPage(player.position.z + Boundary.RoundZ(chargedZ)) == Boundary.ZToPage(enemy.position.z))
            {
                pageText.color = red;
                GetComponent<Image>().color = red;
            }
            else
            {
                pageText.color = darkPurple;
                GetComponent<Image>().color = black;
            }
        }
    }

    public void SetVisible()
    {
        foreach (Image i in GetComponentsInChildren<Image>())
        {
            i.enabled = true;
        }
        pageText.enabled = true;
    }

    private Vector2 CircleSize(int page)
    {
        float s = Mathf.Lerp(minSize, maxSize, (page - Boundary.pageBase) / (float)Boundary.pageNum);
        return new Vector2(s, s);
    }
}
