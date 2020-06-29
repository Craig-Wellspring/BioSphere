using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VisualPerception : MonoBehaviour
{
    public Collider selfCollider;
    public Transform eyesTransform;

    [Header("Sight Range Settings")]
    public float perceptionRadius = 10f;
    [Space(10)]
    public bool drawSightSphere = false;

    [Header("Draw Sight Lines")]
    public bool predatorSightLines = true;
    public bool preySightLines = true;
    public bool foodSightLines = true;

    [Header("Currently Visible")]
    public List<Collider> nearbyMates;
    public GameObject closestMate;
    [Space(10)]
    public List<Collider> nearbyPredators;
    public GameObject closestPredator;
    [Space(10)]
    public List<Collider> nearbyPrey;
    public GameObject closestPrey;
    [Space(10)]
    public List<Collider> nearbyFood;
    public GameObject closestFood;

    #region LayerMasks
    private LayerMask creatureLayerMask;
    private LayerMask foodLayerMask;
    private LayerMask foliageLayerMask;
    private LayerMask corpseLayerMask;
    private int searchMasks;
    #endregion

    private Metabolism metabolism;


    void Start()
    {
        creatureLayerMask = LayerMask.GetMask("Fauna");
        foodLayerMask = LayerMask.GetMask("FoodItem");
        foliageLayerMask = LayerMask.GetMask("Foliage");
        corpseLayerMask = LayerMask.GetMask("Corpse");

        searchMasks = creatureLayerMask + foodLayerMask + foliageLayerMask + corpseLayerMask;

        metabolism = GetComponent<Metabolism>();
    }



    void Update()
    {
        //Clear vision from last frame
        nearbyFood.Clear();
        closestFood = null;
        nearbyMates.Clear();
        closestMate = null;
        nearbyPredators.Clear();
        closestPredator = null;
        nearbyPrey.Clear();
        closestPrey = null;


        // Check surroundings, populate a list of everything in the area
        List<Collider> withinSightRange = Physics.OverlapSphere(transform.root.position, perceptionRadius, searchMasks).ToList();
        withinSightRange.Remove(selfCollider);


        //For every object in the area, check for line of sight and categorize into lists
        foreach (Collider col in withinSightRange)
        {
            //Check for clear Sight Line
            RaycastHit hit;
            Ray sightRay = new Ray(eyesTransform.position, col.transform.position - eyesTransform.position);
            if (col.Raycast(sightRay, out hit, perceptionRadius))
            {
                //if (hit.collider == col)
                //{
                    //Register nearby Food
                    if (metabolism.dietList.Contains(col.transform.tag))
                    {
                        nearbyFood.Add(col);

                        if (foodSightLines)
                            Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.cyan, Time.deltaTime);
                        continue;
                    }

                    //Register nearby Mates
                    if (col.transform.tag == selfCollider.transform.tag)
                    {
                        nearbyMates.Add(col);
                        continue;
                    }

                    //Register nearby Predators
                    Metabolism potentialPred = col.transform.root.GetComponentInChildren<Metabolism>();
                    if (potentialPred != null)
                    {
                        if (potentialPred.preyList.Contains(selfCollider.transform.tag))
                        {
                            nearbyPredators.Add(col);

                            if (predatorSightLines)
                                Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.red, Time.deltaTime);
                            continue;
                        }
                    }

                    //Register nearby Prey
                    if (metabolism.preyList.Contains(col.transform.tag))
                    {
                        nearbyPrey.Add(col);

                        if (preySightLines)
                            Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.magenta, Time.deltaTime);
                        continue;
                    }
                //}
            }
        }

        //Find the closest member of each type
        closestFood = GetClosestTarget(nearbyFood);
        closestMate = GetClosestTarget(nearbyMates);
        closestPredator = GetClosestTarget(nearbyPredators);
        closestPrey = GetClosestTarget(nearbyPrey);
    }



    public GameObject GetClosestTarget(List<Collider> _targets)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Collider potentialTarget in _targets)
        {
            if (potentialTarget != null)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr && potentialTarget.gameObject != this.gameObject)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.gameObject;
                }
            }
        }
        return bestTarget;
    }


    private void OnDrawGizmosSelected()
    {
        if (drawSightSphere)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, perceptionRadius);
        }
    }
}
