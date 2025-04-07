using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BallData : ScriptableObject
{
    public enum GoalType { totaldistance, bestDistance}
    public enum BallType { basic, orange, star}

    public BallType ballType;
    public GoalType goalType;
    //public GameObject ball;
    public bool basic;

    public string ballAblity;

    public int amount;

}
