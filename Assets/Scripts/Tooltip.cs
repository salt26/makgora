using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject tooltipPanel;
    public GameObject tooltipPanelClone;
    public Text tooltipText;
    public Transform canvas;
    public string text;
    public int x, y;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipText.text = text;
        tooltipPanelClone = Instantiate(tooltipPanel, new Vector3(x, y, 0), Quaternion.identity, canvas);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tooltipPanelClone);
    }
}
