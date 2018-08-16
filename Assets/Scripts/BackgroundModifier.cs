using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundModifier : MonoBehaviour {

    public List<Sprite> backgrounds = new List<Sprite>();

    [SerializeField]
    private GameObject player;

    private void FixedUpdate()
    {
        if (player != null)
        {
            int magicNumber = Boundary.ZToPage(player.GetComponent<Rigidbody>().position.z).ToString().GetHashCode();
            Debug.Log(magicNumber);
            GetComponentInChildren<SpriteRenderer>().sprite = 
                backgrounds[magicNumber % backgrounds.Count];
        }
    }
}
