using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morphology : MonoBehaviour
{
    public GameObject carniMorph;
    public GameObject herbiMorph;
    
    CreatureData cData;

    private bool carnivore = false;



    private void Start()
    {
        cData = GetComponent<CreatureData>();
    }



    private void CalculateEvolutions()
    {
        if (cData.lifetimeDiet.Contains("Meat"))
            carnivore = true;
    }

    private GameObject NewMorph()
    {
        if (carnivore)
            return carniMorph;
        else
            return herbiMorph;
    }

    public void Evolve()
    {
        CalculateEvolutions();
        GameObject morphToForm = NewMorph();

        Debug.Log(transform.root.name + " is trying to morph into " + morphToForm.name);
    }
}
