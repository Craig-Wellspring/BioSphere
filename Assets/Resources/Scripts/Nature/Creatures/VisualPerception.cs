using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VisualPerception : MonoBehaviour
{
    public Collider selfCollider;
    public Transform eyesTransform;

    [Header("Sight Range Settings")]
    public float creaturePerceptionRange = 10f;
    public float foodPerceptionRange = 10f;

    [Header("Draw Sight Lines")]
    public bool predatorSightLines = true;
    public bool preySightLines = true;
    public bool foodSightLines = true;

    [Header("Currently Visible")]
    [HideInInspector] public List<Collider> nearbySiblings;
    public List<Collider> nearbyPredators;
    public List<Collider> nearbyPrey;
    public List<Collider> nearbyFood;
    

    private LayerMask creatureLayerMask;
    private LayerMask eatingLayerMask;
    
    private Metabolism metabolism;
    private CreatureData cData;


    void Start()
    {
        creatureLayerMask = LayerMask.GetMask("Creature");
        eatingLayerMask = LayerMask.GetMask("Food");

        metabolism = GetComponent<Metabolism>();
        cData = GetComponent<CreatureData>();
    }


    void FixedUpdate()
    {
        
        nearbySiblings = new List<Collider>();
        nearbyPredators = new List<Collider>();
        nearbyPrey = new List<Collider>();
        nearbyFood = new List<Collider>();

        // Check surroundings

        List<Collider> withinSightRange = Physics.OverlapSphere(transform.root.position, creaturePerceptionRange, creatureLayerMask + eatingLayerMask).ToList();
        if (withinSightRange.Contains(selfCollider))
            withinSightRange.Remove(selfCollider);

        foreach (Collider col in withinSightRange)
        {
            //Check for clear Sight Line
            RaycastHit hit;
            Ray sightRay = new Ray(eyesTransform.position, col.transform.position - eyesTransform.position);
            if (Physics.Raycast(sightRay, out hit, creaturePerceptionRange))
            {
                //Register nearby Siblings
                if (hit.collider.transform.tag == transform.tag)
                    nearbySiblings.Add(col);

                //Register nearby Predators
                if (cData.predatorList.Contains(hit.collider.transform.tag))
                {
                    nearbyPredators.Add(col);

                    if (predatorSightLines)
                        Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.red, Time.fixedDeltaTime);
                }

                //Register nearby Prey
                if (cData.preyList.Contains(hit.collider.transform.tag))
                {
                    nearbyPrey.Add(col);

                    if (preySightLines)
                        Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.magenta, Time.fixedDeltaTime);
                }

                //Register nearby Food
                if (metabolism.dietList.Contains(hit.collider.transform.tag))
                {
                    nearbyFood.Add(col);

                    if (foodSightLines)
                        Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.cyan, Time.fixedDeltaTime);
                }
            }
        }
    }
}
