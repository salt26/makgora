using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject tooltipPanel;
    private GameObject tooltipPanelClone;
    public Transform canvas;
    public string text;
    public int x, y;
    public int width, height;
    public float r, g, b;
    public int fontSize;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Button>().interactable)
        {
            GetComponent<Image>().color = new Color(r, g, b, 1);
            tooltipPanelClone = Instantiate(tooltipPanel, new Vector3(x, y, 0), Quaternion.identity, canvas);
            tooltipPanelClone.GetComponent<RectTransform>().sizeDelta = new Vector2(width,height);
            tooltipPanelClone.GetComponentInChildren<Text>().text = text;
            tooltipPanelClone.GetComponentInChildren<Text>().fontSize = fontSize;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyTooltip();
    }

    public void DestroyTooltip()
    {
        GetComponent<Image>().color = new Color(0, 0, 0, 1);
        Destroy(tooltipPanelClone);
        tooltipPanelClone = null;
    }
}
