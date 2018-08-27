using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDestination : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && other.GetComponent<TutorialManager>().Phase == 1)
        {
            other.GetComponent<TutorialManager>().NextProcess();
            Destroy(gameObject);
        }
    }
}
