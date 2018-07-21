using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    private int phase;  // 0: XY평면 상 이동, 1: 시간축 상 이동, 2: 멈춘 적 맞추기

	// Use this for initialization
	void Start () {
        phase = 0;
	}
	
	void FixedUpdate () {
		
	}
}
