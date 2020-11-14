using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VisualPerception : MonoBehaviour
{
    [Header("Currently Visible")]
    public GameObject closestMate;
    public List<GameObject> nearbyMates;
    [Space(10)]
    public GameObject closestFood;
    public List<GameObject> nearbyFood;
    [Space(10)]
    public GameObject closestPrey;
    public List<GameObject> nearbyPrey;
    [Space(10)]
    public GameObject closestPredator;
    public List<GameObject> nearbyPredators;


    [Header("Sight Settings")]
    public Transform eyes;
    [Range(0, 20)] public float sightRadius = 10f;
    //[SerializeField] float radiusIncrement = 0.5f;
    [SerializeField] bool drawSightSphere = false;
    [Space(10)]
    [SerializeField] bool cullUnderwater = true;
    public LayerMask visibleLayers;
    //public float viewAngle;


    [Header("Debug")]
    [SerializeField] bool foodSightLines = true;
    [SerializeField] bool preySightLines = true;
    [SerializeField] bool predatorSightLines = true;



    // Private variables
    Metabolism metabolism;
    BodyReference body;

    float sightRefreshRate = 0.250f;


    void Start()
    {
        metabolism = GetComponent<Metabolism>();
        body = transform.root.GetComponent<BodyReference>();


        // Register Sight Radius in StatBlock
        // GetComponent<CreatureStats>()?.AddNewStat("Perception", sightRadius, radiusIncrement);

        // Start Seeing
        InvokeRepeating("Sight", sightRefreshRate, sightRefreshRate);
    }



    void Sight()
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
        List<Collider> withinSightRange = Physics.OverlapSphere(eyes.position, sightRadius, visibleLayers).ToList();
        withinSightRange.Remove(body.mainBodyCollider);


        //For every object in the area, check for line of sight and categorize into lists
        foreach (Collider _col in withinSightRange)
        {
            // Don't see objects that are under water
            if (cullUnderwater && !UtilityFunctions.AboveSeaLevel(_col.transform.position))
                continue;

            // Check for clear Sight Line
            RaycastHit hit;
            Ray sightRay = new Ray(eyes.position, _col.transform.position - eyes.position);
            if (_col.Raycast(sightRay, out hit, sightRadius))
            {
                // Creatures with the same tag as me are my mates
                if (_col.transform.tag == body.mainBodyCollider.transform.tag)
                {
                    nearbyMates.Add(_col.gameObject);
                    continue;
                }

                if (metabolism != null)
                {
                    // Register food if it is part of my diet
                    if (metabolism.dietList.Contains(_col.transform.tag))
                    {
                        nearbyFood.Add(_col.gameObject);

                        if (foodSightLines)
                            Debug.DrawRay(eyes.position, _col.transform.position - eyes.position, Color.cyan, Time.deltaTime);
                        continue;
                    }

                    // Register nearby Prey
                    if (metabolism.preyList.Contains(_col.transform.tag))
                    {
                        nearbyPrey.Add(_col.gameObject);

                        if (preySightLines)
                            Debug.DrawRay(eyes.position, _col.transform.position - eyes.position, Color.magenta, Time.deltaTime);
                        continue;
                    }

                    // Creatures who see me as Prey are my Predators
                    Metabolism potentialPred = _col.transform.root.GetComponentInChildren<Metabolism>();
                    if (potentialPred != null)
                    {
                        if (potentialPred.preyList.Contains(body.mainBodyCollider.transform.tag))
                        {
                            nearbyPredators.Add(_col.gameObject);

                            if (predatorSightLines)
                                Debug.DrawRay(eyes.position, _col.transform.position - eyes.position, Color.red, Time.deltaTime);
                            continue;
                        }
                    }
                }
            }
        }

        //Find the closest member of each type
        closestFood = UtilityFunctions.ClosestObjectToTransform(transform, nearbyFood, false);
        closestMate = UtilityFunctions.ClosestObjectToTransform(transform, nearbyMates, true);
        closestPredator = UtilityFunctions.ClosestObjectToTransform(transform, nearbyPredators, true);
        closestPrey = UtilityFunctions.ClosestObjectToTransform(transform, nearbyPrey, true);
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
