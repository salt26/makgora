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

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipPanelClone = Instantiate(tooltipPanel, new Vector3(x, y, 0), Quaternion.identity, canvas);
        tooltipPanelClone.GetComponent<RectTransform>().sizeDelta = new Vector2(width,height);
        tooltipPanelClone.GetComponentInChildren<Text>().text = text;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tooltipPanelClone);
        tooltipPanelClone = null;
    }

    public void DestroyTooltip()
    {
        Destroy(tooltipPanelClone);
        tooltipPanelClone = null;
    }
}
