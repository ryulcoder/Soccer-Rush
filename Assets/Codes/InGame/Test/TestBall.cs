using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall : MonoBehaviour
{
    public Transform target;

    public float speed = 1;

    Vector3 targetVec;
    Vector2 ratio;

    bool penalKickOn;

    void Start()
    {
        targetVec = target.position;

        Vector3 absVec = new(Mathf.Abs(targetVec.x), Mathf.Abs(targetVec.y), 0);

        float sum = absVec.x + absVec.y;

        ratio = new(absVec.x / sum, absVec.y / sum);

        //PenalKickOn();
    }

    void FixedUpdate()
    {
        if (penalKickOn)
        {
            if (targetVec == transform.position)
                penalKickOn = false;

            target.position = Vector3.MoveTowards(target.position, targetVec, Time.timeScale * speed);
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.timeScale * speed);
        }


    }

    public void PenalKickOn()
    {
        float dis = Vector3.Distance(transform.position, targetVec);

        float shape = dis * Mathf.Sin(45 * Mathf.Deg2Rad) * 2;

        Vector3 direction = (targetVec - transform.position).normalized; // πÊ«‚ ∫§≈Õ

        direction = new Vector3(direction.x / Mathf.Abs(direction.x) * shape * ratio.x, direction.y / Mathf.Abs(direction.y) * shape * ratio.y, 0);

        Debug.LogWarning(direction);

        target.position += direction;

        Debug.LogWarning(dis);
        Debug.LogWarning(Vector3.Distance(target.position, targetVec));

        penalKickOn = true;

    }
}
