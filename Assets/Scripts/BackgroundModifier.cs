using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundModifier : MonoBehaviour {

    public SpriteRenderer pastSprite;
    public SpriteRenderer presentSprite;
    public SpriteRenderer futureSprite;
    public RectTransform backgroundCanvas;
    public List<Sprite> backgrounds = new List<Sprite>();

    [SerializeField]
    private GameObject player;

    private void Start()
    {
        pastSprite.GetComponent<Transform>().localPosition = new Vector3(pastSprite.GetComponent<Transform>().localPosition.x,
            pastSprite.GetComponent<Transform>().localPosition.y, -Boundary.OnePageToDeltaZ() * 10000f);
        presentSprite.GetComponent<Transform>().localPosition = new Vector3(presentSprite.GetComponent<Transform>().localPosition.x,
            presentSprite.GetComponent<Transform>().localPosition.y, 0f);
        futureSprite.GetComponent<Transform>().localPosition = new Vector3(futureSprite.GetComponent<Transform>().localPosition.x,
            futureSprite.GetComponent<Transform>().localPosition.y, Boundary.OnePageToDeltaZ() * 10000f);
        backgroundCanvas.localPosition = new Vector3(backgroundCanvas.localPosition.x,
            backgroundCanvas.localPosition.y, Boundary.OnePageToDeltaZ() * 20000f);
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            int pageNumber = Boundary.ZToPage(player.GetComponent<Rigidbody>().position.z);
            if (pageNumber > Boundary.pageBase)
                pastSprite.sprite =
                    backgrounds[Mathf.Abs(pageNumber - 1) % backgrounds.Count];
            else
                pastSprite.sprite = null;

            presentSprite.sprite =
                backgrounds[Mathf.Abs(pageNumber) % backgrounds.Count];

            if (pageNumber < Boundary.pageBase + Boundary.pageNum)
                futureSprite.sprite = 
                    backgrounds[Mathf.Abs(pageNumber + 1) % backgrounds.Count];
            else
                futureSprite.sprite = null;

            /*
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
            */
        }
    }
}
