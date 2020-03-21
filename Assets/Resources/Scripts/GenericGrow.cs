using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericGrow : MonoBehaviour
{
    private Vector3 scaleChange;
    private float growTime = 0;
    private float maxSize;
    

    //public Transform growObject;
    public enum GrowType { growOverTime, growByTrigger };
    public GrowType growType;
    [Tooltip("Only applies to Grow Over Time types")]
    public float growTimeSeconds = 3f;
    public float growRate = 0.1f;
    public float maxSizeMin, maxSizeMax;
    [Tooltip("Detach from Parent when FullyGrown")]
    public bool gainIndependence = false;
    [Header("Triggers")]
    public bool triggerGrowth = false;
    public bool fullyGrown = false;


    void Start()
    {
        scaleChange = new Vector3(growRate, growRate, growRate);
        maxSize = Random.Range(maxSizeMin, maxSizeMax);
    }

    void Update()
    {
        if (!fullyGrown)
        {
            if (growType == GrowType.growOverTime)
            {
                //Tick
                growTime += Time.deltaTime;
                if (growTime > growTimeSeconds)
                {
                    growTime = 0;
                    //Grow
                    transform.localScale += scaleChange;
                    CheckIfFullyGrown();
                }
            }
            if (growType == GrowType.growByTrigger && triggerGrowth)
            {
                //Grow
                transform.localScale += scaleChange;
                CheckIfFullyGrown();
                triggerGrowth = false;
            }
        }
    }

    void CheckIfFullyGrown()
    {
        if (transform.localScale.y >= maxSize)
        {
            fullyGrown = true;

            //Gain independence
            if (gainIndependence)
                transform.SetParent(null);
        }
    }
}