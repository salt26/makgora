using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {

    public Knife knife;    // Color 값을 얻기 위함입니다.
    public Transform player;

    private Transform t;

    private void Awake()
    {
        t = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        if (knife == null)
        {
            // 플레이어 캐릭터와의 Z좌표(시간축 좌표) 차이에 따라 투명도를 적용합니다.
            GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f,
                Mathf.Max(0, 1 - 2f * Mathf.Pow(Mathf.Abs(
                GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position.z - GetComponent<Transform>().position.z), 2)));
        }
        else
        {
            float alpha = 1f;
            if (Mathf.Abs(player.position.z - t.position.z) < Boundary.OnePageToDeltaZ() * 0.8f)
            {
                GetComponent<MeshRenderer>().material.color = knife.AlphaColor(knife.presentEnemyColor, alpha);
            }
            else if (t.position.z < player.position.z)
            {
                GetComponent<MeshRenderer>().material.color =
                    knife.AlphaColor(Color.Lerp(knife.pastColor, knife.pastPastColor,
                    Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * 0.8f), alpha);
            }
            else
            {
                GetComponent<MeshRenderer>().material.color =
                    knife.AlphaColor(Color.Lerp(knife.futureColor, knife.futureFutureColor,
                    Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * 0.8f), alpha);
            }
        }
    }
}
