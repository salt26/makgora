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

    public RectTransform chargePoint;
    public RectTransform enemyPoint;
    public RectTransform chargeBar;
    public RectTransform chargeCircle;
    public Text pageText;
    public int minPos, maxPos;
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
        Vector3 v = mainCamera.WorldToScreenPoint(GetComponentInParent<MeshRenderer>().GetComponent<Transform>().position);
        GetComponent<RectTransform>().position = v;
        chargeBar.position = v;
        chargeCircle.position = v;
        pageText.GetComponent<RectTransform>().position = v;

        if (player.GetComponent<Player>().Health > 0)
        {
            /*
            blueHand.SetPositionAndRotation(GetComponent<RectTransform>().position,
                Quaternion.Euler(0f, 0f, -30f * (player.position.z + Boundary.RoundZ(chargedZ))));
                */
            chargePoint.position = GetComponent<RectTransform>().position + PointPosition(Boundary.ZToPage(player.position.z + Boundary.RoundZ(chargedZ)));
            //chargePoint.sizeDelta = CircleSize(Boundary.ZToPage(player.position.z + Boundary.RoundZ(chargedZ)));
            //pageText.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
            pageText.text = Boundary.ZToPage(player.position.z + Boundary.RoundZ(chargedZ)).ToString();
        }
        if (enemy.GetComponent<Enemy>().Health > 0)
        {
            enemyPoint.position = GetComponent<RectTransform>().position + PointPosition(Boundary.ZToPage(enemy.position.z));
            //enemyPoint.sizeDelta = CircleSize(Boundary.ZToPage(enemy.position.z));
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
        foreach (Image i in GetComponentInParent<Canvas>().GetComponentsInChildren<Image>())
        {
            i.enabled = true;
        }
        pageText.enabled = true;
    }

    /*
    private Vector2 CircleSize(int page)
    {
        float s = Mathf.Lerp(maxSize, minSize, (page - Boundary.pageBase) / (float)Boundary.pageNum);
        return new Vector2(s, s);
    }
    */
    private Vector3 PointPosition(int page)
    {
        return new Vector3(Mathf.Lerp(minPos, maxPos, (page - Boundary.pageBase) / (float)Boundary.pageNum), 0f, 0f);
    }
}
