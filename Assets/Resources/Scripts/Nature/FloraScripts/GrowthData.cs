using UnityEngine;
using System.Collections.Generic;
using System;

public class GrowthData : MonoBehaviour
{
    [Header("State")]
    [SerializeField] bool halfGrown = false;
    [SerializeField] bool fullyGrown = false;

    [Header("Settings")]
    public float growthSpeed = 5;

    [Header("Half Grown")]
    [Tooltip("Change the Tag of Root transform to this string when Half Grown"), SerializeField]
    string halfGrownTag;
    [Tooltip("Detach from Parent when Half Grown")]
    [SerializeField] bool gainIndependenceHG = false;
    [Tooltip("Shade local terrain green by increment when Half Grown"), SerializeField]
    float addTerrainColor = 0;

    [Space(10)]
    public List<GameObject> activateObjectsHG;
    [SerializeField] bool spawnScale0HG = true;


    [Header("Fully Grown")]
    [Tooltip("Change the Tag of Root transform to this string when Fully Grown"), SerializeField]
    string fullyGrownTag;
    [Tooltip("Detach from Parent when FullyGrown")]
    [SerializeField] bool gainIndependenceFG = false;

    [Space(10)]
    public List<GameObject> activateObjectsFG;
    [SerializeField] bool spawnScale0FG = true;


    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        anim.SetFloat("GrowthSpeed", growthSpeed);


    }


    public Action halfGrownTrigger;
    void HalfGrown()
    {
        halfGrown = true;
        halfGrownTrigger?.Invoke();

        if (halfGrownTag.Length > 0)
            transform.tag = halfGrownTag;

        if (anim.parameters.ToString().Contains("HalfGrown"))
            anim.SetBool("HalfGrown", true);

        if (activateObjectsHG.Count > 0)
        {
            foreach (GameObject _obj in activateObjectsHG)
            {
                if (spawnScale0HG)
                    _obj.transform.localScale = Vector3.zero;

                _obj.SetActive(true);
            }
        }

        if (gainIndependenceHG)
            GainIndependence();

        if (addTerrainColor > 0)
            gameObject.AddComponent<TerrainColorizer>().ColorizeTerrain(addTerrainColor);
    }

    public Action fullyGrownTrigger;
    void FullyGrown()
    {
        fullyGrown = true;
        fullyGrownTrigger?.Invoke();

        if (fullyGrownTag.Length > 0)
            transform.tag = fullyGrownTag;

        if (anim.parameters.ToString().Contains("FullyGrown"))
            anim.SetBool("FullyGrown", true);

        if (activateObjectsFG.Count > 0)
        {
            foreach (GameObject _obj in activateObjectsFG)
            {
                if (spawnScale0FG)
                    _obj.transform.localScale = Vector3.zero;

                _obj.SetActive(true);
            }
        }

        if (gainIndependenceFG)
            GainIndependence();
    }


    public void ParentDies()
    {
        if (!halfGrown)
            Destroy(this.gameObject);
        else
            GainIndependence();
    }

    void GainIndependence()
    {
        transform.SetParent(null);

        if (TryGetComponent<GravityAttract>(out GravityAttract gBody))
            gBody.enabled = true;

        if (TryGetComponent<Rigidbody>(out Rigidbody rBody))
            rBody.constraints = RigidbodyConstraints.None;

    }
}