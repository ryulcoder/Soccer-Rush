using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goalkeeper_Hands : MonoBehaviour
{
    public static Goalkeeper_Hands instance;

    Transform Ball;

    bool isCatch;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        isCatch = false;
    }

    public void DropBall()
    {
        Ball.SetParent(null, true);

        Penalty.Instance.goalEndMove = true;
    }

    void OnTrigerEnter(Collider collider)
    {
        if (collider.CompareTag("Ball") && !isCatch)
        {
            isCatch = true;

            Penalty.Instance.NoGoal();

            Ball = collider.GetComponent<Transform>();
            Ball.SetParent(transform);

            Debug.LogWarning("NoGoal");
        }
    }
}
