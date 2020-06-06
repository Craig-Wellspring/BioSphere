using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VisualPerception : MonoBehaviour
{
    public Collider selfCollider;

    [Header("Sight Range Settings")]
    public float predatorPerceptionRange = 10f;
    public float preyPerceptionRange = 10f;
    public float foodPerceptionRange = 10f;
    



    [HideInInspector] public List<Collider> nearbyPredators;
    [HideInInspector] public List<Collider> nearbyPrey;
    [HideInInspector] public List<Collider> nearbyFood;
    private LayerMask fleeLayerMask;
    private LayerMask aggroLayerMask;
    private LayerMask eatingLayerMask;

    private Metabolism metabolism;
    private CreatureData cData;


    void Start()
    {
        fleeLayerMask = LayerMask.GetMask("Creature");
        aggroLayerMask = LayerMask.GetMask("Creature");
        eatingLayerMask = LayerMask.GetMask("Food");

        metabolism = GetComponent<Metabolism>();
        cData = GetComponent<CreatureData>();
    }


    void FixedUpdate()
    {
        // Check surroundings

        //a) Look for Predators
        nearbyPredators = new List<Collider>();
        List<Collider> withinFleeRange = Physics.OverlapSphere(transform.root.position, predatorPerceptionRange, fleeLayerMask).ToList();
        if (withinFleeRange.Contains(selfCollider))
            withinFleeRange.Remove(selfCollider);
        foreach (Collider col in withinFleeRange)
            if (cData.predatorList.Contains(col.transform.root.tag))
                nearbyPredators.Add(col);


        //b) Look for Prey
        nearbyPrey = new List<Collider>();
        List<Collider> withinAggroRange = Physics.OverlapSphere(transform.root.position, preyPerceptionRange, aggroLayerMask).ToList();
        if (withinAggroRange.Contains(selfCollider))
            withinAggroRange.Remove(selfCollider);
        foreach (Collider col in withinAggroRange)
            if (cData.preyList.Contains(col.transform.root.tag))
                nearbyPrey.Add(col);


        //c) Look for Food
        nearbyFood = new List<Collider>();
        List<Collider> withinEatingRange = Physics.OverlapSphere(transform.root.position, foodPerceptionRange, eatingLayerMask).ToList();
        if (withinEatingRange.Contains(selfCollider))
            withinEatingRange.Remove(selfCollider);
        foreach (Collider col in withinEatingRange)
            if (metabolism.dietList.Contains(col.transform.tag))
                nearbyFood.Add(col);
    }
}
