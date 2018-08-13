using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour {

    public float duration;
    private float startTime;

	// Use this for initialization
	void Awake () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - startTime > duration)
        {
            Destroy(gameObject);
        }
	}
}
