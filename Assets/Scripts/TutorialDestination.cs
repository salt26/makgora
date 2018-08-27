using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDestination : MonoBehaviour {

    private Vector3 startPosition, destPosition;
    private float moveTime = 0f, temporalMoveCoolTime = 0f;
    private bool startMoving = false, endMoving = false;

    public bool EndMoving
    {
        get
        {
            return endMoving;
        }
    }

    void FixedUpdate()
    {
        if (Manager.instance.IsPaused) return;

        if (startMoving && !endMoving &&
            Vector3.Distance(GetComponent<Transform>().position, destPosition) < Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지에 도착했습니다.
            endMoving = true;
        }
        else if (startMoving && !endMoving && temporalMoveCoolTime > 0f)
        {
            temporalMoveCoolTime -= Time.fixedDeltaTime;
        }
        else if (startMoving && !endMoving &&
            Mathf.Abs(GetComponent<Transform>().position.z - destPosition.z) > Boundary.OnePageToDeltaZ() / 2f)
        {
            // 목적지를 향해 시간 축을 따라 이동합니다.
            Rigidbody r = GetComponent<Rigidbody>();
            r.velocity = Vector3.zero;
            float deltaZ;
            if (GetComponent<Transform>().position.z > destPosition.z) deltaZ = -Boundary.OnePageToDeltaZ();
            else deltaZ = Boundary.OnePageToDeltaZ();
            temporalMoveCoolTime = 1f / Manager.instance.TemporalSpeed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z + deltaZ, Boundary.zMin, Boundary.zMax)
            );
        }
        /*
        if (moveTime > 0f)
        {
            moveTime -= Time.fixedDeltaTime;
            Vector3 pos = Vector3.Lerp(destPosition, startPosition, moveTime / 2f);
            pos.z = Boundary.RoundZ(pos.z);
            GetComponent<Transform>().SetPositionAndRotation(pos, Quaternion.identity);
        }
        if (moveTime < 0f)
        {
            moveTime = 0f;
        }
        if (moveTime <= 0f && startMoving)
        {
            endMoving = true;
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && other.GetComponent<TutorialManager>().Phase == 1)
        {
            other.GetComponent<TutorialManager>().NextProcess();
            //Destroy(gameObject);
        }
    }

    public void SetMoving(Vector3 dest)
    {
        startPosition = GetComponent<Transform>().position;
        destPosition = dest;
        startMoving = true;
    }
}
