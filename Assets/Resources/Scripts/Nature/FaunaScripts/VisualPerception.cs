using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VisualPerception : AdvancedMonoBehaviour
{
    [Header("Sight Settings")]
    public Transform eyes;
    [Range(0, 20)] public float sightRadius = 10f;
    [SerializeField] bool drawSightSphere = false;
    public LayerMask searchLayers;
    //public float viewAngle;


    [Header("Currently Visible")]
    public GameObject closestMate;
    public List<Collider> nearbyMates;
    [Space(10)]
    public GameObject closestFood;
    public List<Collider> nearbyFood;
    [Space(10)]
    public GameObject closestPrey;
    public List<Collider> nearbyPrey;
    [Space(10)]
    public GameObject closestPredator;
    public List<Collider> nearbyPredators;


    [Header("Debug")]
    [SerializeField] bool foodSightLines = true;
    [SerializeField] bool preySightLines = true;
    [SerializeField] bool predatorSightLines = true;



    // Private variables
    Metabolism metabolism;
    BodyReference body;


    void Start()
    {
        metabolism = GetComponent<Metabolism>();
        body = transform.root.GetComponent<BodyReference>();


        // Register Sight Radius in StatBlock
        GetComponent<CreatureStats>()?.AddNewStat("Perception", sightRadius);
    }



    void Update()
    {
        //Clear vision from last frame
        nearbyFood.Clear();
        closestFood = null;

        nearbyMates.Clear();
        closestMate = null;

        nearbyPrey.Clear();
        closestPrey = null;

        nearbyPredators.Clear();
        closestPredator = null;


        // Check surroundings, populate a list of everything in the area
        List<Collider> withinSightRange = Physics.OverlapSphere(eyes.position, sightRadius, searchLayers).ToList();
        withinSightRange.Remove(body.mainBodyCollider);


        //For every object in the area, check for line of sight and categorize into lists
        foreach (Collider col in withinSightRange)
        {
            // Check for clear Sight Line
            RaycastHit hit;
            Ray sightRay = new Ray(eyes.position, col.transform.position - eyes.position);
            if (col.Raycast(sightRay, out hit, sightRadius))
            {
                // Creatures with the same tag as me are my mates
                if (col.transform.tag == body.mainBodyCollider.transform.tag)
                {
                    nearbyMates.Add(col);
                    continue;
                }

                if (metabolism != null)
                {
                    // Register food if it is part of my diet
                    if (metabolism.dietList.Contains(col.transform.tag))
                    {
                        nearbyFood.Add(col);

                        if (foodSightLines)
                            Debug.DrawRay(eyes.position, col.transform.position - eyes.position, Color.cyan, Time.deltaTime);
                        continue;
                    }

                    // Register nearby Prey
                    if (metabolism.preyList.Contains(col.transform.tag))
                    {
                        nearbyPrey.Add(col);

                        if (preySightLines)
                            Debug.DrawRay(eyes.position, col.transform.position - eyes.position, Color.magenta, Time.deltaTime);
                        continue;
                    }

                    // Creatures who see me as Prey are my Predators
                    Metabolism potentialPred = col.transform.root.GetComponentInChildren<Metabolism>();
                    if (potentialPred != null)
                    {
                        if (potentialPred.preyList.Contains(body.mainBodyCollider.transform.tag))
                        {
                            nearbyPredators.Add(col);

                            if (predatorSightLines)
                                Debug.DrawRay(eyes.position, col.transform.position - eyes.position, Color.red, Time.deltaTime);
                            continue;
                        }
                    }
                }
            }
        }

        //Find the closest member of each type
        closestFood = ClosestObjInColliderList(nearbyFood, false);
        closestMate = ClosestObjInColliderList(nearbyMates, true);
        closestPredator = ClosestObjInColliderList(nearbyPredators, true);
        closestPrey = ClosestObjInColliderList(nearbyPrey, true);
    }

    private void OnDrawGizmosSelected()
    {
        if (drawSightSphere && eyes)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(eyes.position, sightRadius);
        }
    }
}
