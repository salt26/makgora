using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StalkerEnemy : Enemy {

    private Vector3 playerPosition;

    protected override void Move()
    {
        if (!isArrived && Vector3.Distance(t.position, dest) < 0.01f)
        {
            // 목적지에 도착했습니다.
            isArrived = true;
        }
        else if (!isArrived)
        {
            // 목적지를 향해 이동합니다.
            Vector3 movement = dest - t.position;
            r.velocity = movement.normalized * speed;

            r.position = new Vector3
            (
                Mathf.Clamp(r.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(r.position.y, Boundary.yMin, Boundary.yMax),
                Mathf.Clamp(r.position.z, Boundary.zMin, Boundary.zMax)
            );
        }

        if (isArrived)
        {
            // 새로 가려는 목적지를 정합니다.
            playerPosition = player.GetComponent<Transform>().position;
            float x = playerPosition.x + 0.3f * GaussianRandom();
            float y = playerPosition.y + 0.3f * GaussianRandom();
            float z = playerPosition.z + 0.5f * GaussianRandom();
            x = Mathf.Clamp(x, Boundary.xMin, Boundary.xMax);
            y = Mathf.Clamp(y, Boundary.yMin, Boundary.yMax);
            z = Mathf.Clamp(z, Boundary.zMin, Boundary.zMax);
            dest = new Vector3
            (
                x,
                y,
                z
            );
            isArrived = false;
        }
    }
}
