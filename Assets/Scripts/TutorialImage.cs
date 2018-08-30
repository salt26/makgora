using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialImage : MonoBehaviour {

    private Transform t;
    private Transform player;
    private Camera mainCamera;

    private void Awake() {
        t = GetComponent<Transform>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

	private void FixedUpdate() {
        Vector3 v = mainCamera.WorldToScreenPoint(player.position);
        if(Mathf.Abs(t.position.x-v.x)<330f && Mathf.Abs(t.position.y - v.y) < 230f)
        {
            GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
