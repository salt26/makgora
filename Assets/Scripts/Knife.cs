using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Knife : MonoBehaviour {

    private int owner = -1;     // 칼을 던진 플레이어가 누구인지 (0: 휴먼, 1: 컴퓨터)
    private Vector3 start;      // 출발지입니다.
    private Vector3 dest;       // 목적지입니다. 날아갈 방향을 결정합니다.
    private Vector3 direction;

    private Transform t;
    private Transform player;
    private Transform enemy;
    private GameObject text;
    private GameObject sound;
    private Vector3 soundVector;
    private Camera mainCamera;

    private bool isCracked;

    private float speed;
    private float soundTime;
    private bool soundHasMade=false;
    private int soundNum;

    public GameObject flare;
    public GameObject knifeText;
    public GameObject soundEffect;
    public Sprite[] soundEffectImage;

    // 칼이 생성될 때 자동으로, 한 번만 호출됩니다.
    private void Awake()
    {
        t = GetComponent<Transform>();  // 이 칼의 위치를 갖고 있습니다.
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        start = t.position;
        isCracked = false;
        speed = Manager.instance.KnifeSpeed;

        if (Mathf.Abs(player.position.z - t.position.z) > Boundary.sight)
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = false;
            }
        }
        else
        {
            TextMover();
            SoundEffectMover();
        }
    }

    // 매 프레임마다 자동으로 호출됩니다.
    void FixedUpdate() {

        float ownZ = 0f;
        float otherZ = 0f;
		if (owner != -1)
        {
            t.SetPositionAndRotation(t.position + direction * Time.fixedDeltaTime,
                //Quaternion.Euler(Quaternion.LookRotation(direction).eulerAngles + new Vector3(0f, 0f, 0f)));
                Quaternion.LookRotation(direction, Vector3.forward) *
                Quaternion.FromToRotation(new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f)));
        }

        float alpha = 1f;   // 대상을 지나 사라질 때 투명화됨
        if (owner == 0)
        {
            ownZ = player.position.z;
            otherZ = enemy.position.z;
        }
        else if (owner == 1)
        {
            ownZ = enemy.position.z;
            otherZ = player.position.z;
        }

        if (Mathf.Abs(player.position.z - t.position.z) > Boundary.sight)
        {
            alpha = 0f;
            if (text != null) text.GetComponent<Text>().enabled = false;
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = false;
            }
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = false;
            }
        }
        else
        {
            if (text != null) text.GetComponent<Text>().enabled = true;
            TextMover();
            SoundEffectMover();
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = true;
            }
            
            // 투사체가 날아가다가 목표를 지난 후에
            if (direction.z != 0f && (otherZ - t.position.z) * direction.z < 0)
            {
                // 상대방 위치의 반대 방향으로 총알을 쏘면 자신과의 Z좌표(시간축 좌표) 차이에 따라 투명도를 적용합니다.
                if ((ownZ - otherZ) * (ownZ - t.position.z) < 0)
                {
                    alpha = Mathf.Pow(Mathf.Abs(ownZ - t.position.z) / Boundary.sight - 1, 2);
                }
                // 그 외의 경우 대상 캐릭터와의 Z좌표 차이에 따라 투명도를 적용합니다.
                else
                {
                    alpha = Mathf.Pow(Mathf.Abs(otherZ - t.position.z) / Boundary.sight - 1, 2);
                }

            }
        }
        
        if (owner == 0)
        {
            if (!isCracked && Mathf.Abs(otherZ - t.position.z) < 0.02f &&
                Mathf.Abs(dest.z - start.z) > Boundary.OnePageToDeltaZ() * 0.5f)
            {
                Instantiate(flare, t.position, Quaternion.identity);
                isCracked = true;
            }
        }

        if (Mathf.Abs(player.position.z - t.position.z) < Boundary.OnePageToDeltaZ() * Boundary.approach)
        {
            if (owner == 0)
                GetComponent<MeshRenderer>().material.color = ColorUtil.instance.AlphaColor(ColorUtil.instance.presentPlayerColor, alpha);
            else
                GetComponent<MeshRenderer>().material.color = ColorUtil.instance.AlphaColor(ColorUtil.instance.presentEnemyColor, alpha);
        }
        else if (t.position.z < player.position.z)
        {
            GetComponent<MeshRenderer>().material.color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.pastColor, ColorUtil.instance.pastPastColor, 
                Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * Boundary.approach), alpha);
        }
        else
        {
            GetComponent<MeshRenderer>().material.color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.futureColor, ColorUtil.instance.futureFutureColor,
                Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * Boundary.approach), alpha);
        }

        if (Mathf.Abs(t.position.z) > Boundary.zMax + Boundary.sight ||
            Mathf.Abs(t.position.x) > Boundary.xMax + Boundary.sight || 
            Mathf.Abs(t.position.y) > Boundary.yMax + Boundary.sight)
        {
            Destroy(text);
            Destroy(gameObject);
        }

    }
    
    /// <summary>
    /// 칼을 날리기 위해 외부에서 호출해줘야 하는 함수입니다.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="destination"></param>
    public void Initialize(int owner, int soundNum, Vector3 destination)
    {
        dest = destination;
        if (owner == 0 || owner == 1)
            this.owner = owner;
        
        if (owner != -1) direction = (dest - start).normalized * speed;  // 속력은 항상 speed만큼입니다.

        if (owner == 0)
        {
            MakeSoundEffect();
            StartCoroutine("SoundEffectMover");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && owner == 1 && other.GetComponent<Player>().Health > 0)
        {
            other.GetComponent<Player>().Damaged();
            Destroy(text);
            Destroy(gameObject);
        }
        else if (other.tag.Equals("Enemy") && owner == 0 && other.GetComponent<Enemy>().Health > 0)
        {
            if (!isCracked)
            {
                Instantiate(flare, t.position, Quaternion.identity);
                isCracked = true;
            }
            other.GetComponent<Enemy>().damaged();
            Destroy(text);
            Destroy(gameObject);
        }
    }

    private void TextMover()
    {
        if (Manager.instance.GetCurrentGame()[1].Equals("Hard"))
        {
            return;
        }
        Vector3 v = mainCamera.WorldToScreenPoint(t.position);
        Vector3 d = mainCamera.WorldToScreenPoint(t.position + direction) - v;
        // d.x * a.x + d.y * a.y = 0
        // a.x = d.y
        // a.y = -d.x
        Vector3 o = new Vector3(d.y, -d.x, 0f).normalized;
        if (owner == 0)
        {
            v -= o * 30f;
            v -= d.normalized * 10f;
        }
        else if (owner == 1)
        {
            v -= o * 20f;
            v -= d.normalized * 25f;
        }

        if (text != null)
        {
            text.GetComponent<Transform>().position = v;
        }
        else
        {
            text = Instantiate(knifeText, v, Quaternion.identity, Manager.instance.Canvas.GetComponent<Transform>());
        }
        int pageDiff = Boundary.ZToPage(t.position.z) - Boundary.ZToPage(player.position.z);
        if (pageDiff > 0)
        {
            text.GetComponent<Text>().text = "+" + pageDiff.ToString();
        }
        else
        {
            text.GetComponent<Text>().text = pageDiff.ToString();
        }

        float alpha = 1f;
        if (owner == 1 && direction.z != 0f && (player.position.z - t.position.z) * direction.z < 0)
        {
            alpha = Mathf.Pow(Mathf.Abs(player.position.z - t.position.z) / Boundary.sight - 1, 2);
        }

        if (Mathf.Abs(player.position.z - t.position.z) < Boundary.OnePageToDeltaZ() * Boundary.approach)
        {
            if (owner == 0)
                text.GetComponent<Text>().color = ColorUtil.instance.AlphaColor(ColorUtil.instance.presentPlayerColor, alpha);
            else
                text.GetComponent<Text>().color = ColorUtil.instance.AlphaColor(ColorUtil.instance.presentEnemyColor, alpha);
        }
        else if (t.position.z < player.position.z)
        {
            text.GetComponent<Text>().color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.pastColor, ColorUtil.instance.pastPastColor,
                Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * Boundary.approach), alpha);
        }
        else
        {
            text.GetComponent<Text>().color =
                ColorUtil.instance.AlphaColor(Color.Lerp(ColorUtil.instance.futureColor, ColorUtil.instance.futureFutureColor,
                Mathf.Abs(player.position.z - t.position.z) - Boundary.OnePageToDeltaZ() * Boundary.approach), alpha);
        }
    }

    private void MakeSoundEffect()
    {
        soundTime = 0.5f;
        Vector3 d = direction.normalized;
        Vector3 o = new Vector3(d.y, -d.x, d.z);
        soundVector = player.position + d * 0.3f + o * 0.18f;
        sound = Instantiate(soundEffect, soundVector, Quaternion.identity);
        sound.GetComponent<SpriteRenderer>().sprite = soundEffectImage[soundNum];
    }

    IEnumerator SoundEffectMover()
    {
        for (int i = 0; i <= 30; i++)
        {
            soundVector += direction.normalized * 0.01f;
            sound.GetComponent<Transform>().position = soundVector;
            sound.GetComponent<SpriteRenderer>().color = ColorUtil.instance.AlphaColor(new Color(1f, 1f, 1f), 1f - Mathf.Pow((float)i / 30f, 2f));
            yield return null;
        }
        Destroy(sound);
        sound = null;
    }
}
