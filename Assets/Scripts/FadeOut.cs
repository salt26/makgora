using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {
    
    public Transform player;

    private Transform t;

    private void Awake()
    {
        t = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        float alpha;
        if (Mathf.Abs(player.position.z - t.position.z) > Boundary.OnePageToDeltaZ() * 1.5f)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            return;
        }
        else if (Mathf.Abs(player.position.z - t.position.z) < Boundary.OnePageToDeltaZ() * 0.5f)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            alpha = 1f;
        }
        else if (player.position.z - t.position.z > Boundary.OnePageToDeltaZ() * 0.5f &&
            player.position.z - t.position.z < Boundary.OnePageToDeltaZ() * 1.5f)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            alpha = 0.5f;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;
            alpha = 0.7f;
        }

        if (Mathf.Abs(player.position.z - t.position.z) < Boundary.OnePageToDeltaZ() * 0.8f)
        {
            GetComponent<SpriteRenderer>().color = ColorUtil.instance.AlphaColor(Color.black, alpha);
        }
        else if (t.position.z < player.position.z)
        {
            GetComponent<SpriteRenderer>().color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.pastColor, ColorUtil.instance.pastPastColor,
                Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * 0.8f), alpha);
        }
        else
        {
            GetComponent<SpriteRenderer>().color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.futureColor, ColorUtil.instance.futureFutureColor,
                Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * 0.8f), alpha);
        }
    }
}
