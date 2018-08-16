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
            int pageNumber = Boundary.ZToPage(player.GetComponent<Rigidbody>().position.z);
            Debug.Log(pageNumber);
            GetComponentInChildren<SpriteRenderer>().sprite = 
                backgrounds[Mathf.Abs(pageNumber) % backgrounds.Count];

            switch ((pageNumber / backgrounds.Count) % 4)
            {
                case 0:
                    GetComponentInChildren<SpriteRenderer>().flipX = false;
                    GetComponentInChildren<SpriteRenderer>().flipY = false;
                    break;
                case 1:
                    GetComponentInChildren<SpriteRenderer>().flipX = false;
                    GetComponentInChildren<SpriteRenderer>().flipY = true;
                    break;
                case 2:
                    GetComponentInChildren<SpriteRenderer>().flipX = true;
                    GetComponentInChildren<SpriteRenderer>().flipY = false;
                    break;
                default:
                    GetComponentInChildren<SpriteRenderer>().flipX = true;
                    GetComponentInChildren<SpriteRenderer>().flipY = true;
                    break;
            }
        }
    }
}
