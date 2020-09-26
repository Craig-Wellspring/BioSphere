using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VisualPerception : AdvancedMonoBehaviour
{
    [Header("Sight Range Settings")]
    [SerializeField] Transform eyesTransform;
    public float sightRadius = 10f;
    //public float viewAngle;


    [Header("Currently Visible")]
    public List<Collider> nearbyMates;
    public GameObject closestMate;
    [Space(10)]
    public List<Collider> nearbyFood;
    public GameObject closestFood;
    [Space(10)]
    public List<Collider> nearbyPrey;
    public GameObject closestPrey;
    [Space(10)]
    public List<Collider> nearbyPredators;
    public GameObject closestPredator;


    [Header("Debug")]
    [SerializeField] bool drawSightSphere = false;
    [Space(15)]
    [SerializeField] bool foodSightLines = true;
    [SerializeField] bool preySightLines = true;
    [SerializeField] bool predatorSightLines = true;



    // Private variables
    [HideInInspector] public int searchMasks;

    Metabolism metabolism;
    CreatureData cData;


    void Start()
    {
        searchMasks = LayerMask.GetMask("Fauna", "FoodItem", "Foliage", "Corpse");

        metabolism = GetComponent<Metabolism>();
        cData = GetComponent<CreatureData>();
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
        List<Collider> withinSightRange = Physics.OverlapSphere(transform.root.position, sightRadius, searchMasks).ToList();
        withinSightRange.Remove(cData.mainBodyCollider);


        //For every object in the area, check for line of sight and categorize into lists
        foreach (Collider col in withinSightRange)
        {
            //Check for clear Sight Line
            RaycastHit hit;
            Ray sightRay = new Ray(eyesTransform.position, col.transform.position - eyesTransform.position);
            if (col.Raycast(sightRay, out hit, sightRadius))
            {
                //Register nearby Mates
                if (col.transform.tag == cData.mainBodyCollider.transform.tag)
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
                            Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.cyan, Time.deltaTime);
                        continue;
                    }

                    //Register nearby Prey
                    if (metabolism.preyList.Contains(col.transform.tag))
                    {
                        nearbyPrey.Add(col);

                        if (preySightLines)
                            Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.magenta, Time.deltaTime);
                        continue;
                    }

                    //Register nearby Predators
                    Metabolism potentialPred = col.transform.root.GetComponentInChildren<Metabolism>();
                    if (potentialPred != null)
                    {
                        if (potentialPred.preyList.Contains(cData.mainBodyCollider.transform.tag))
                        {
                            nearbyPredators.Add(col);

                            if (predatorSightLines)
                                Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.red, Time.deltaTime);
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
        if (drawSightSphere)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, sightRadius);
        }
    }
}
