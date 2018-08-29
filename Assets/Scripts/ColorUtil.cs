using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorUtil : MonoBehaviour {

    public static ColorUtil instance;
    
    [Header("Colors")]
    public Color pastPastColor;
    public Color pastColor;
    public Color presentEnemyColor;
    public Color futureColor;
    public Color futureFutureColor;
    public Color presentPlayerColor;
    public Color presentMentorColor;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public Color AlphaColor(Color c, float alpha)
    {
        return new Color(c.r, c.g, c.b, alpha);
    }
}
