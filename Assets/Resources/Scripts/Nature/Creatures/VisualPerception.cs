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

    [HideInInspector] public List<Collider> nearbySiblings;
    [Header("Currently Visible")]
    public List<Collider> nearbyPredators;
    public List<Collider> nearbyPrey;
    public List<Collider> nearbyFood;
    

    private LayerMask creatureLayerMask;
    private LayerMask foodLayerMask;
    private LayerMask foliageLayerMask;
    private LayerMask corpseLayerMask;
    private int searchMasks;

    private Metabolism metabolism;
    private CreatureData cData;


    void Start()
    {
        creatureLayerMask = LayerMask.GetMask("Fauna");
        foodLayerMask = LayerMask.GetMask("FoodItem");
        foliageLayerMask = LayerMask.GetMask("Foliage");
        corpseLayerMask = LayerMask.GetMask("Corpse");

        searchMasks = creatureLayerMask + foodLayerMask + foliageLayerMask + corpseLayerMask;

        metabolism = GetComponent<Metabolism>();
        cData = GetComponent<CreatureData>();
    }



    void FixedUpdate()
    {
        //Clear list from last frame

        nearbySiblings = new List<Collider>();
        nearbyPredators = new List<Collider>();
        nearbyPrey = new List<Collider>();
        nearbyFood = new List<Collider>();


        // Check surroundings

        List<Collider> withinSightRange = Physics.OverlapSphere(transform.root.position, perceptionRadius, searchMasks).ToList();
        if (withinSightRange.Contains(selfCollider))
            withinSightRange.Remove(selfCollider);

        foreach (Collider col in withinSightRange)
        {
            //Check for clear Sight Line
            RaycastHit hit;
            Ray sightRay = new Ray(eyesTransform.position, col.transform.position - eyesTransform.position);
            if (Physics.Raycast(sightRay, out hit, perceptionRadius))
            {
                if (hit.collider == col)
                {
                    //Register nearby Siblings
                    if (hit.collider.transform.tag == selfCollider.transform.tag)
                    {
                        nearbySiblings.Add(col);
                    }

                    //Register nearby Predators
                    if (cData.predatorList.Contains(hit.collider.transform.tag))
                    {
                        if (predatorSightLines)
                            Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.red, Time.fixedDeltaTime);

                        nearbyPredators.Add(col);
                    }

                    //Register nearby Prey
                    if (cData.preyList.Contains(hit.collider.transform.tag))
                    {
                        if (preySightLines)
                            Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.magenta, Time.fixedDeltaTime);

                        nearbyPrey.Add(col);
                    }

                    //Register nearby Food
                    if (metabolism.dietList.Contains(hit.collider.transform.tag))
                    {
                        if (foodSightLines)
                            Debug.DrawRay(eyesTransform.position, col.transform.position - eyesTransform.position, Color.cyan, Time.fixedDeltaTime);

                        nearbyFood.Add(col);
                    }
                }
            }
        }
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
