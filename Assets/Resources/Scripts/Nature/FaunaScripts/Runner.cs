using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Runner : MonoBehaviour
{
    public float moveSpeed = 1;
    [SerializeField] float speedIncrement = 0.5f;

    AIPathAlignedToSurface pathing;
    CreatureStats cStats;

    void Start()
    {
        pathing = GetComponent<AIPathAlignedToSurface>();
        cStats = GetComponentInChildren<CreatureStats>();

        // Update runspeed
        moveSpeed = pathing.maxSpeed;

        cStats?.AddNewStat("Speed", moveSpeed, speedIncrement);
    }
    
    public void ChangeSpeed(float _amount)
    {
        if (pathing != null)
            pathing.maxSpeed += _amount;

        moveSpeed += _amount;   
    }
}
