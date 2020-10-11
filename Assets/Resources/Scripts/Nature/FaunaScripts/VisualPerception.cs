using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VisualPerception : AdvancedMonoBehaviour
{
    [Header("Sight Range Settings")]
    [Range(0, 20)] public float sightRadius = 10f;
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
    [SerializeField] bool drawSightSphere = false;
    [Space(15)]
    [SerializeField] bool foodSightLines = true;
    [SerializeField] bool preySightLines = true;
    [SerializeField] bool predatorSightLines = true;



    // Private variables
    [HideInInspector] public int searchMasks;

    Metabolism metabolism;
    BodyReference body;


    void Start()
    {
        searchMasks = LayerMask.GetMask("Fauna", "FoodItem", "Vegetation", "Corpse");

        metabolism = GetComponent<Metabolism>();
        body = transform.root.GetComponent<BodyReference>();
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
        List<Collider> withinSightRange = Physics.OverlapSphere(body.eyes.position, sightRadius, searchMasks).ToList();
        withinSightRange.Remove(body.mainBodyCollider);


        //For every object in the area, check for line of sight and categorize into lists
        foreach (Collider col in withinSightRange)
        {
            //Check for clear Sight Line
            RaycastHit hit;
            Ray sightRay = new Ray(body.eyes.position, col.transform.position - body.eyes.position);
            if (col.Raycast(sightRay, out hit, sightRadius))
            {
                //Register nearby Mates
                if (col.transform.tag == body.mainBodyCollider.transform.tag)
                {
                    nearbyMates.Add(col);
                    continue;
                }

                if (metabolism != null)
                {
                    //Register nearby Food
                    if (metabolism.dietList.Contains(col.transform.tag))
                    {
                        nearbyFood.Add(col);

                        if (foodSightLines)
                            Debug.DrawRay(body.eyes.position, col.transform.position - body.eyes.position, Color.cyan, Time.deltaTime);
                        continue;
                    }

                    //Register nearby Prey
                    if (metabolism.preyList.Contains(col.transform.tag))
                    {
                        nearbyPrey.Add(col);

                        if (preySightLines)
                            Debug.DrawRay(body.eyes.position, col.transform.position - body.eyes.position, Color.magenta, Time.deltaTime);
                        continue;
                    }

                    //Register nearby Predators
                    Metabolism potentialPred = col.transform.root.GetComponentInChildren<Metabolism>();
                    if (potentialPred != null)
                    {
                        if (potentialPred.preyList.Contains(body.mainBodyCollider.transform.tag))
                        {
                            nearbyPredators.Add(col);

                            if (predatorSightLines)
                                Debug.DrawRay(body.eyes.position, col.transform.position - body.eyes.position, Color.red, Time.deltaTime);
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
        if (drawSightSphere && body.eyes)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, sightRadius);
        }
    }
}
