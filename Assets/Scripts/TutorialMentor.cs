using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMentor : MonoBehaviour {

    private Vector3 startPosition, destPosition;
    private float moveTime = 0f, temporalMoveCoolTime = 0f;
    private bool startMoving = false, endMoving = false;
    private bool isArrived = false; // 1페이즈에서 플레이어가 멘토에게 닿으면 true가 됩니다.

    public bool StartMoving
    {
        get
        {
            return startMoving;
        }
    }

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
            GetComponent<Rigidbody>().velocity = Vector3.zero;
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
        else if (startMoving && !endMoving)
        {
            // 목적지를 향해 XY평면을 따라 이동합니다.
            Rigidbody r = GetComponent<Rigidbody>();
            Vector3 movement = destPosition - GetComponent<Transform>().position;
            movement.z = 0f;
            r.velocity = movement.normalized * Manager.instance.MovingSpeed * 1.5f;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
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
        if (other.tag.Equals("Player") && other.GetComponent<TutorialManager>().Phase == 1 && !isArrived)
        {
            isArrived = true;
            other.GetComponent<TutorialManager>().NextProcess();
            //Destroy(gameObject);
        }
    }

    public void SetMoving(Vector3 dest)
    {
        startPosition = GetComponent<Transform>().position;
        destPosition = dest;
        startMoving = true;
        endMoving = false;
    }

    public void SetStop()
    {
        if (startMoving && endMoving)
        {
            startMoving = false;
            endMoving = false;
        }
    }
}
