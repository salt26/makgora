using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {

    private void FixedUpdate()
    {
        // 플레이어 캐릭터와의 Z좌표(시간축 좌표) 차이에 따라 투명도를 적용합니다.
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f,
            Mathf.Max(0, 1 - 1.2f * Mathf.Pow(Mathf.Abs(
                GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position.z - GetComponent<Transform>().position.z), 2)));
    }
}
