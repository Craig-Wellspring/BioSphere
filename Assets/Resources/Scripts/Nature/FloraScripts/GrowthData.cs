using UnityEngine;
using System.Collections.Generic;
using System;

public class GrowthData : MonoBehaviour
{
    [Header("Settings")]
    public float growthSpeed = 5;

    [Header("Half Grown")]
    //public bool halfGrown = false;
    [Tooltip("Change the Tag of Root transform to this string when Half Grown"), SerializeField]
    string halfGrownTag;
    public List<GameObject> activateObjectsHG;
    [Tooltip("Detach from Parent when Half Grown")]
    public bool gainIndependenceHG = false;
    [Tooltip("Shade local terrain green by increment when Half Grown"),SerializeField] 
    float addTerrainColor = 0;


    [Header("Fully Grown")]
    //public bool fullyGrown = false;
    [Tooltip("Change the Tag of Root transform to this string when Fully Grown"), SerializeField]
    string fullyGrownTag;
    public List<GameObject> activateObjectsFG;
    [Tooltip("Detach from Parent when FullyGrown")]
    public bool gainIndependenceFG = false;


    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        anim.SetFloat("GrowthSpeed", growthSpeed);
    }


    public Action halfGrownTrigger;
    void HalfGrown()
    {
        //halfGrown = true;
        halfGrownTrigger?.Invoke();

        if (halfGrownTag.Length > 0)
            transform.tag = halfGrownTag;

        if (anim.parameters.ToString().Contains("HalfGrown"))
            anim.SetBool("HalfGrown", true);

        if (activateObjectsHG.Count > 0)
            activateObjectsHG.ForEach(_obj => _obj.SetActive(true));

        if (gainIndependenceHG)
            GainIndependence();

        if (addTerrainColor > 0)
            gameObject.AddComponent<TerrainColorizer>().ColorizeTerrain(addTerrainColor);
    }

    public Action fullyGrownTrigger;
    void FullyGrown()
    {
        //fullyGrown = true;
        fullyGrownTrigger?.Invoke();

        if (fullyGrownTag.Length > 0)
            transform.tag = fullyGrownTag;

        if (anim.parameters.ToString().Contains("FullyGrown"))
            anim.SetBool("FullyGrown", true);

        if (activateObjectsFG.Count > 0)
            activateObjectsFG.ForEach(_obj => _obj.SetActive(true));

        if (gainIndependenceFG)
            GainIndependence();
    }

    void GainIndependence()
    {
        transform.SetParent(null);

        GravityAttract gBody = GetComponent<GravityAttract>();
        if (gBody)
            gBody.enabled = true;
    }
}