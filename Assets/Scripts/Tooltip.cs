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

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipText.text = "튜토리얼입니다.";
        tooltipPanelClone = Instantiate(tooltipPanel, new Vector3(750, 514, 0), Quaternion.identity, canvas);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tooltipPanelClone);
    }
}
