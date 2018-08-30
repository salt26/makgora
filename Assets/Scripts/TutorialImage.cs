using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialImage : MonoBehaviour {

    private Transform t;
    private Transform player;
    private Camera mainCamera;
    private float alpha;

    private void Awake() {
        t = GetComponent<Transform>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        alpha = 1f;
    }

	private void FixedUpdate() {
        Vector3 v = mainCamera.WorldToScreenPoint(player.position);
        if (Mathf.Abs(t.position.x - v.x) < GetComponent<RectTransform>().rect.width / 2 + 30f && Mathf.Abs(t.position.y - v.y) < GetComponent<RectTransform>().rect.height / 2 + 55f)
        {
            alpha -= Time.fixedDeltaTime * 1.5f;
        }
        else
        {
            alpha += Time.fixedDeltaTime * 1.5f;
        }
        if (alpha < 0.3f) alpha = 0.3f;
        if (alpha > 1f) alpha = 1f;
        GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
;    }
}
