using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEnemy : Enemy
{ 
    
    private Vector3 start;

    void FixedUpdate()
    {
        if (invincibleTime > 0f)
        {
            invincibleTime -= Time.fixedDeltaTime;
            t.SetPositionAndRotation(Vector3.Lerp(dest, start, invincibleTime / 3f), Quaternion.identity);
            //r.velocity = (dest - start) / 3f;
            //r.position = Vector3.Lerp(dest, start, invincibleTime / 3f);
        }
        if (invincibleTime < 0f)
        {
            invincibleTime = 0f;
        }
        if (invincibleTime <= 0f && myShield != null)
        {
            t.SetPositionAndRotation(dest, Quaternion.identity);
            //r.velocity = Vector3.zero;
            //r.MovePosition(dest);
            Destroy(myShield);
            myShield = null;
        }

        if (health <= 0) return;
    }

    public override void Damaged()
    {
        if (Health > 0 && invincibleTime <= 0f)
        {
            Debug.LogWarning("Enemy hit!");
            health--;
            if (hearts.Count > Health)
            {
                hearts[Health].SetActive(false);
            }
            if (Health > 0)
            {
                start = GetComponent<Transform>().position;
                if (Health == 2)
                {
                    dest = new Vector3(-1f, 0.05f, 2.89f);
                    GameObject.FindGameObjectWithTag("Player").GetComponent<TutorialManager>().tutorialText.text =
                        "마우스 왼쪽을 눌러 과거로, 또는 마우스 오른쪽을 눌러 미래로 칼을 던질 수 있습니다.\n" +
                        "마우스를 누르고 있으면 작은 시계가 나타납니다.\n이 시계의 초록색 침은 칼이 향할 시간을 가리킵니다.\n" +
                        "마우스를 오래 누를수록 더 먼 과거(미래)로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                        "마우스로 상대를 조준하고 초록색 침과 파란색 침이 겹칠 때까지 눌렀다가 떼세요.\n" +
                        "\"움직이지 않는 상대를 향해 칼을 던져서 2번 더 맞추세요.\"";
                }
                else if (Health == 1)
                {
                    dest = new Vector3(0.1f, 0.3f, -3.5f);
                    GameObject.FindGameObjectWithTag("Player").GetComponent<TutorialManager>().tutorialText.text =
                        "마우스 왼쪽을 눌러 과거로, 또는 마우스 오른쪽을 눌러 미래로 칼을 던질 수 있습니다.\n" +
                        "마우스를 누르고 있으면 작은 시계가 나타납니다.\n이 시계의 초록색 침은 칼이 향할 시간을 가리킵니다.\n" +
                        "마우스를 오래 누를수록 더 먼 과거(미래)로 칼을 던집니다. 칼의 속력은 일정합니다.\n" +
                        "마우스로 상대를 조준하고 초록색 침과 파란색 침이 겹칠 때까지 눌렀다가 떼세요.\n" +
                        "\"움직이지 않는 상대를 향해 칼을 던져서 1번 더 맞추세요.\"";
                }
                invincibleTime = 3f;
                myShield = Instantiate(divineShield, GetComponent<Transform>());
                GetComponent<AudioSource>().clip = damagedSound;
                GetComponent<AudioSource>().Play();
            }
        }
        else if (Health > 0 && invincibleTime > 0f)
        {
            GetComponent<AudioSource>().clip = guardSound;
            GetComponent<AudioSource>().Play();
        }

        if (Health <= 0 && GetComponentInChildren<CharacterModel>().gameObject.activeInHierarchy)
        {
            invincibleTime = 0f;
            GetComponentInChildren<CharacterModel>().gameObject.SetActive(false);
            r.velocity = Vector3.zero;
            GetComponent<AudioSource>().clip = killedSound;
            GetComponent<AudioSource>().Play();

            blowend = Instantiate(blow, GetComponent<Transform>().position, Quaternion.identity);

            StartCoroutine("Blow");
            Manager.instance.GraduateTutorial();

        }
    }

    
}
