using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookUI : MonoBehaviour {

    public Transform player;
    public Transform enemy;

    public RectTransform greenPage;
    public Text greenText;
    public RectTransform redPage;
    public Text redText;

	void FixedUpdate () {
        /*
        float pz = player.GetComponent<Transform>().position.z + Time.fixedTime;
        float ez = enemy.GetComponent<Transform>().position.z + Time.fixedTime;
        GetComponent<Text>().text = "자신의 시간: ";
        if (pz < 0)
        {
            GetComponent<Text>().text += "-" + (int)(Mathf.Abs(pz)) + "." + (int)(Mathf.Abs(pz) * 1000) % 1000;
        }
        else
        {
            GetComponent<Text>().text += (int)(Mathf.Abs(pz)) + "." + (int)(Mathf.Abs(pz) * 1000) % 1000;
        }
        GetComponent<Text>().text += "\n상대의 시간: ";
        if (ez < 0)
        {
            GetComponent<Text>().text += "-" + (int)(Mathf.Abs(ez)) + "." + (int)(Mathf.Abs(ez) * 1000) % 1000;
        }
        else
        {
            GetComponent<Text>().text += (int)(Mathf.Abs(ez)) + "." + (int)(Mathf.Abs(ez) * 1000) % 1000;
        }
        */
        if (player.GetComponent<Player>().Health > 0)
        {
            greenPage.anchoredPosition = new Vector2(Mathf.Lerp(-240f, 240f,
                ((Boundary.ZToPage(player.position.z) - Boundary.pageBase) / (float)Boundary.pageNum)), greenPage.anchoredPosition.y);
            greenText.text = Boundary.ZToPage(player.position.z).ToString();
        }
        if (enemy.GetComponent<Enemy>().Health > 0)
        {
            redPage.anchoredPosition = new Vector2(Mathf.Lerp(-240f, 240f,
                ((Boundary.ZToPage(enemy.position.z) - Boundary.pageBase) / (float)Boundary.pageNum)), redPage.anchoredPosition.y);
            redText.text = Boundary.ZToPage(enemy.position.z).ToString();
        }
    }
}
